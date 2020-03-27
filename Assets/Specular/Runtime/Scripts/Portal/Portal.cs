using System;
using System.Collections.Generic;
using UnityEngine;

namespace Specular.Runtime.Scripts.Portal {
  public class Portal : SpecularCamera {
    [Header("Main Settings")]
    [SerializeField]
    internal Portal linkedPortal;

    [SerializeField] internal int recursionLimit = 5;

    [Header("Advanced Settings")]
    [SerializeField]
    internal float nearClipOffset = 0.05f;

    [SerializeField] internal float nearClipLimit = 0.2f;

    // Private variables
    PortalRenderer _portal_renderer;

    Material _first_recursion_mat;
    List<PortalTraveller> _tracked_travellers;

    internal static readonly Int32 _Active = Shader.PropertyToID("_IsActive");
    internal static readonly Int32 _Slice_Centre = Shader.PropertyToID("_SliceCentre");
    internal static readonly Int32 _Slice_Normal = Shader.PropertyToID("_SliceNormal");
    internal static readonly Int32 _Slice_Offset_Dst = Shader.PropertyToID("_SliceOffsetDst");

    void Awake() {
      this._specular_camera = this.GetComponentInChildren<PortalCamera>().CameraComponent;
      this._portal_renderer = this.GetComponentInChildren<PortalRenderer>();
      this._tracked_travellers = new List<PortalTraveller>();
    }

    void LateUpdate() {
      Teleportation.HandleTravellers(_tracked_travellers : ref this._tracked_travellers,
                                     linkedPortal : this.linkedPortal,
                                     transform : this.transform);
    }

    protected internal override void PreSpecularRender(Camera receiver) {
      // Called before any portal cameras are rendered for the current frame
      Teleportation.SliceTravellers(_tracked_travellers : ref this._tracked_travellers,
                                    portal : this,
                                    receiver : receiver);
    }

    protected internal override void SpecularRender(Camera receiver) {
      // Manually render the camera attached to this portal
      // Called after PrePortalRender, and before PostPortalRender

      // Skip rendering the view from this portal if player is not looking at the linked portal
      if (!CameraUtility.VisibleFromCamera(renderer : this.linkedPortal._portal_renderer, camera : receiver)
      ) {
        return;
      }

      this._portal_renderer.CreateViewTexture(this_portal : this);

      var local_to_world_matrix = receiver.transform.localToWorldMatrix;
      var render_positions = new Vector3[this.recursionLimit];
      var render_rotations = new Quaternion[this.recursionLimit];

      var start_index = 0;
      this._Specular_Camera.projectionMatrix = receiver.projectionMatrix;
      for (var i = 0; i < this.recursionLimit; i++) {
        if (i > 0) {
          // No need for recursive rendering if linked portal is not visible through this portal
          if (!CameraUtility.BoundsOverlap(near_object : this._portal_renderer.ScreenMeshFilter,
                                           far_object : this.linkedPortal._portal_renderer.ScreenMeshFilter,
                                           camera : this._Specular_Camera)) {
            break;
          }
        }

        local_to_world_matrix = this.transform.localToWorldMatrix
                                * this.linkedPortal.transform.worldToLocalMatrix
                                * local_to_world_matrix;
        var render_order_index = this.recursionLimit - i - 1;
        render_positions[render_order_index] = local_to_world_matrix.GetColumn(3);
        render_rotations[render_order_index] = local_to_world_matrix.rotation;

        this._Specular_Camera.transform.SetPositionAndRotation(position : render_positions
                                                                   [render_order_index],
                                                               rotation : render_rotations
                                                                   [render_order_index]);
        start_index = render_order_index;
      }

      // Hide screen so that camera can see through portal
      this._portal_renderer.MeshRenderer.shadowCastingMode =
          UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
      this.linkedPortal._portal_renderer.MeshRenderer.material.SetInt(nameID : _Active, 0);

      for (var i = start_index; i < this.recursionLimit; i++) {
        this._Specular_Camera.transform.SetPositionAndRotation(position : render_positions[i],
                                                               rotation : render_rotations[i]);
        Rendering.SetNearClipPlane(this_portal : this, receiver : receiver);
        Rendering.HandleClipping(this_portal : this, receiver : receiver);
        this._Specular_Camera.Render();

        if (i == start_index) {
          this.linkedPortal._portal_renderer.MeshRenderer.material.SetInt(nameID : _Active, 1);
        }
      }

      // Unhide objects hidden at start of render
      this._portal_renderer.MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    protected internal override void PostSpecularRender(Camera receiver) {
      // Called once all portals have been rendered, but before the player camera renders
      Teleportation.SliceTravellers(_tracked_travellers : ref this._tracked_travellers,
                                    portal : this,
                                    receiver : receiver);

      Rendering.ProtectScreenFromClipping(view_point : receiver.transform.position,
                                          this_portal : this,
                                          receiver : receiver);
    }

    void OnTriggerEnter(Collider other) {
      var traveller = other.GetComponent<PortalTraveller>();
      if (traveller) {
        Teleportation.OnTravellerEnterPortal(traveller : traveller, this_portal : this);
      }
    }

    void OnTriggerExit(Collider other) {
      var traveller = other.GetComponent<PortalTraveller>();
      if (traveller && this._tracked_travellers.Contains(item : traveller)) {
        traveller.ExitPortalThreshold();
        this._tracked_travellers.Remove(item : traveller);
      }
    }

    void OnValidate() {
      if (this.linkedPortal != null) {
        this.linkedPortal.linkedPortal = this;
      }
    }

    public Camera _specular_camera;

    public List<PortalTraveller> TrackedTravellers { get { return this._tracked_travellers; } }

    internal PortalRenderer PortalRenderer { get { return this._portal_renderer; } }

    public Camera _Specular_Camera { get { return this._specular_camera; } }
  }
}
