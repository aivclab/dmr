using System;
using System.Collections.Generic;
using UnityEngine;

namespace Specular.Runtime.Scripts.Portal {
  public static class Teleportation {
    internal static void HandleTravellers(ref List<PortalTraveller> _tracked_travellers,
                                          Portal linkedPortal,
                                          Transform transform) {
      for (var i = 0; i < _tracked_travellers.Count; i++) {
        var traveller = _tracked_travellers[index : i];
        var traveller_t = traveller.transform;
        var m = linkedPortal.transform.localToWorldMatrix
                * transform.worldToLocalMatrix
                * traveller_t.localToWorldMatrix;

        var offset_from_portal = traveller_t.position - transform.position;
        var portal_side = Math.Sign(value : Vector3.Dot(lhs : offset_from_portal, rhs : transform.forward));
        var portal_side_old =
            Math.Sign(value : Vector3.Dot(lhs : traveller.PreviousOffsetFromPortal, rhs : transform.forward));
        // Teleport the traveller if it has crossed from one side of the portal to the other
        if (portal_side != portal_side_old) {
          var position_old = traveller_t.position;
          var rot_old = traveller_t.rotation;
          traveller.Teleport(from_portal : transform,
                             to_portal : linkedPortal.transform,
                             pos : m.GetColumn(3),
                             rot : m.rotation);
          traveller.GraphicsClone.transform.SetPositionAndRotation(position : position_old,
                                                                   rotation : rot_old);
          // Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
          OnTravellerEnterPortal(traveller : traveller, this_portal : linkedPortal);
          _tracked_travellers.RemoveAt(index : i);
          i--;
        } else {
          traveller.GraphicsClone.transform.SetPositionAndRotation(position : m.GetColumn(3),
                                                                   rotation : m.rotation);
          //UpdateSliceParams (traveller);
          traveller.PreviousOffsetFromPortal = offset_from_portal;
        }
      }
    }

    internal static int SideOfPortal(Vector3 pos, Portal portal) {
      return Math.Sign(value : Vector3.Dot(lhs : pos - portal.transform.position,
                                           rhs : portal.transform.forward));
    }

    internal static bool SameSideOfPortal(Vector3 pos_a, Vector3 pos_b, Portal portal) {
      return SideOfPortal(pos : pos_a, portal : portal) == SideOfPortal(pos : pos_b, portal : portal);
    }

    public static void SliceTravellers(ref List<PortalTraveller> _tracked_travellers,
                                       Portal portal,
                                       Camera receiver) {
      for (var index = 0; index < _tracked_travellers.Count; index++) {
        var traveller = _tracked_travellers[index : index];
        UpdateSliceParams(traveller : traveller, portal : portal, receiver : receiver);
      }
    }

    static void UpdateSliceParams(PortalTraveller traveller, Portal portal, Camera receiver) {
      // Calculate slice normal
      var side = SideOfPortal(pos : traveller.transform.position, portal : portal);
      var slice_normal = portal.transform.forward * -side;
      var clone_slice_normal = portal.linkedPortal.transform.forward * side;

      // Calculate slice centre
      var slice_pos = portal.transform.position;
      var clone_slice_pos = portal.linkedPortal.transform.position;

      // Adjust slice offset so that when player standing on other side of portal to the object, the slice doesn't clip through
      float slice_offset_dst = 0;
      float clone_slice_offset_dst = 0;
      var screen_thickness = portal.PortalRenderer.Thickness;

      var player_same_side_as_traveller = SameSideOfPortal(pos_a : receiver.transform.position,
                                                           pos_b : traveller.transform.position,
                                                           portal : portal);
      if (!player_same_side_as_traveller) {
        slice_offset_dst = -screen_thickness;
      }

      var player_same_side_as_clone_appearing =
          side != SideOfPortal(pos : receiver.transform.position, portal : portal.linkedPortal);
      if (!player_same_side_as_clone_appearing) {
        clone_slice_offset_dst = -screen_thickness;
      }

      // Apply parameters
      for (var i = 0; i < traveller.OriginalMaterials.Length; i++) {
        traveller.OriginalMaterials[i].SetVector(nameID : Portal._Slice_Centre, value : slice_pos);
        traveller.OriginalMaterials[i].SetVector(nameID : Portal._Slice_Normal, value : slice_normal);
        traveller.OriginalMaterials[i].SetFloat(nameID : Portal._Slice_Offset_Dst, value : slice_offset_dst);

        traveller.CloneMaterials[i].SetVector(nameID : Portal._Slice_Centre, value : clone_slice_pos);
        traveller.CloneMaterials[i].SetVector(nameID : Portal._Slice_Normal, value : clone_slice_normal);
        traveller.CloneMaterials[i]
                 .SetFloat(nameID : Portal._Slice_Offset_Dst, value : clone_slice_offset_dst);
      }
    }

    internal static void OnTravellerEnterPortal(PortalTraveller traveller, Portal this_portal) {
      if (!this_portal.TrackedTravellers.Contains(item : traveller)) {
        traveller.EnterPortalThreshold();
        traveller.PreviousOffsetFromPortal = traveller.transform.position - this_portal.transform.position;
        this_portal.TrackedTravellers.Add(item : traveller);
      }
    }
  }
}
