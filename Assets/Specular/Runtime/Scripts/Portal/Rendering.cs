using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Specular.Runtime.Scripts.Portal {
  public static class Rendering {
    internal static void SetNearClipPlane(Portal this_portal, Camera receiver) {
      // Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
      // Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO

      // Learning resource:
      // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
      var clip_plane = this_portal.transform;
      var dot = Math.Sign(value : Vector3.Dot(lhs : clip_plane.forward,
                                              rhs : this_portal.transform.position
                                                    - this_portal._Specular_Camera.transform.position));

      var cam_space_pos =
          this_portal._Specular_Camera.worldToCameraMatrix.MultiplyPoint(point : clip_plane.position);
      var cam_space_normal =
          this_portal._Specular_Camera.worldToCameraMatrix.MultiplyVector(vector : clip_plane.forward) * dot;
      var cam_space_dst = -Vector3.Dot(lhs : cam_space_pos, rhs : cam_space_normal)
                          + this_portal.nearClipOffset;

      // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
      if (Mathf.Abs(f : cam_space_dst) > this_portal.nearClipLimit) {
        var clip_plane_camera_space = new Vector4(x : cam_space_normal.x,
                                                  y : cam_space_normal.y,
                                                  z : cam_space_normal.z,
                                                  w : cam_space_dst);

        // Update projection based on new clip plane
        // Calculate matrix with player cam so that player camera settings (fov, etc) are used
        this_portal._Specular_Camera.projectionMatrix =
            receiver.CalculateObliqueMatrix(clipPlane : clip_plane_camera_space);
      } else {
        this_portal._Specular_Camera.projectionMatrix = receiver.projectionMatrix;
      }
    }

    internal static float ProtectScreenFromClipping(Vector3 view_point, Portal this_portal, Camera receiver) {
      // Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
      var half_height = receiver.nearClipPlane * Mathf.Tan(f : receiver.fieldOfView * 0.5f * Mathf.Deg2Rad);
      var half_width = half_height * receiver.aspect;
      var dst_to_near_clip_plane_corner =
          new Vector3(x : half_width, y : half_height, z : receiver.nearClipPlane).magnitude;
      var screen_thickness = dst_to_near_clip_plane_corner;

      if (this_portal.PortalRenderer) {
        var portal_renderer_thickness = this_portal.PortalRenderer.transform;
        var cam_facing_same_dir_as_portal = Vector3.Dot(lhs : this_portal.transform.forward,
                                                        rhs : this_portal.transform.position - view_point)
                                            > 0;
        portal_renderer_thickness.localScale = new Vector3(x : portal_renderer_thickness.localScale.x,
                                                           y : portal_renderer_thickness.localScale.y,
                                                           z : screen_thickness);
        portal_renderer_thickness.localPosition =
            Vector3.forward * (screen_thickness * (cam_facing_same_dir_as_portal ? 0.5f : -0.5f));
      }

      return screen_thickness;
    }

    internal static void HandleClipping(Portal this_portal, Camera receiver) {
      // There are two main graphical issues when slicing travellers
      // 1. Tiny sliver of mesh drawn on backside of portal
      //    Ideally the oblique clip plane would sort this out, but even with 0 offset, tiny sliver still visible
      // 2. Tiny seam between the sliced mesh, and the rest of the model drawn onto the portal screen
      // This function tries to address these issues by modifying the slice parameters when rendering the view from the portal
      // Would be great if this could be fixed more elegantly, but this is the best I can figure out for now
      const float hide_dst = -1000;
      const float show_dst = 1000;
      var screen_thickness =
          ProtectScreenFromClipping(view_point : this_portal._Specular_Camera.transform.position,
                                    this_portal : this_portal.linkedPortal,
                                    receiver : receiver);

      for (var index = 0; index < this_portal.TrackedTravellers.Count; index++) {
        var traveller = this_portal.TrackedTravellers[index : index];
        if (Teleportation.SameSideOfPortal(pos_a : traveller.transform.position,
                                           pos_b : this_portal._Specular_Camera.transform.position,
                                           portal : this_portal)) {
          // Addresses issue 1
          traveller.SetSliceOffsetDst(dst : hide_dst, false);
        } else {
          // Addresses issue 2
          traveller.SetSliceOffsetDst(dst : show_dst, false);
        }

        // Ensure clone is properly sliced, in case it's visible through this portal:
        var clone_side_of_linked_portal =
            -Teleportation.SideOfPortal(pos : traveller.transform.position, portal : this_portal);
        var cam_same_side_as_clone =
            Teleportation.SideOfPortal(pos : this_portal._Specular_Camera.transform.position,
                                       portal : this_portal.linkedPortal)
            == clone_side_of_linked_portal;
        if (cam_same_side_as_clone) {
          traveller.SetSliceOffsetDst(dst : screen_thickness, true);
        } else {
          traveller.SetSliceOffsetDst(dst : -screen_thickness, true);
        }
      }

      //var offset_from_portal_to_cam = this.PortalCamPos - this.transform.position;
      for (var index = 0; index < this_portal.linkedPortal.TrackedTravellers.Count; index++) {
        var linked_traveller = this_portal.linkedPortal.TrackedTravellers[index : index];
        var traveller_pos = linked_traveller.graphicsObject.transform.position;
        var clone_pos = linked_traveller.GraphicsClone.transform.position;
        // Handle clone of linked portal coming through this portal:
        var clone_on_same_side_as_cam =
            Teleportation.SideOfPortal(pos : traveller_pos, portal : this_portal.linkedPortal)
            != Teleportation.SideOfPortal(pos : this_portal._Specular_Camera.transform.position,
                                          portal : this_portal);
        if (clone_on_same_side_as_cam) {
          // Addresses issue 1
          linked_traveller.SetSliceOffsetDst(dst : hide_dst, true);
        } else {
          // Addresses issue 2
          linked_traveller.SetSliceOffsetDst(dst : show_dst, true);
        }

        // Ensure traveller of linked portal is properly sliced, in case it's visible through this portal:
        var cam_same_side_as_traveller =
            Teleportation.SameSideOfPortal(pos_a : linked_traveller.transform.position,
                                           pos_b : this_portal._Specular_Camera.transform.position,
                                           portal : this_portal.linkedPortal);
        if (cam_same_side_as_traveller) {
          linked_traveller.SetSliceOffsetDst(dst : screen_thickness, false);
        } else {
          linked_traveller.SetSliceOffsetDst(dst : -screen_thickness, false);
        }
      }
    }
  }
}
