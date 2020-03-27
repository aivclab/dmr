using Specular.Runtime.Scripts.Portal;
using UnityEngine;

namespace Specular.Runtime.Scripts {
  public static partial class CameraUtility {
    static readonly Vector3[] _cube_corner_offsets = {
                                                         new Vector3(1, 1, 1),
                                                         new Vector3(-1, 1, 1),
                                                         new Vector3(-1, -1, 1),
                                                         new Vector3(-1, -1, -1),
                                                         new Vector3(-1, 1, -1),
                                                         new Vector3(1, -1, -1),
                                                         new Vector3(1, 1, -1),
                                                         new Vector3(1, -1, 1),
                                                     };

    internal static bool VisibleFromCamera(PortalRenderer renderer, Camera camera) {
      // http://wiki.unity3d.com/index.php/IsVisibleFrom

      if (!renderer || !renderer.MeshRenderer)
        return false;

      var frustum_planes = GeometryUtility.CalculateFrustumPlanes(camera : camera);
      return GeometryUtility.TestPlanesAABB(planes : frustum_planes, bounds : renderer.MeshRenderer.bounds);
    }

    internal static bool BoundsOverlap(MeshFilter near_object, MeshFilter far_object, Camera camera) {
      var near = GetScreenRectFromBounds(renderer : near_object, main_camera : camera);
      var far = GetScreenRectFromBounds(renderer : far_object, main_camera : camera);

      // ensure far object is indeed further away than near object
      if (far._ZMax > near._ZMin) {
        // Doesn't overlap on x axis
        if (far._XMax < near._XMin || far._XMin > near._XMax) {
          return false;
        }

        // Doesn't overlap on y axis
        if (far._YMax < near._YMin || far._YMin > near._YMax) {
          return false;
        }

        // Overlaps
        return true;
      }

      return false;
    }

    internal static MinMax3D GetScreenRectFromBounds(MeshFilter renderer, Camera main_camera) {
      // With thanks to http://www.turiyaware.com/a-solution-to-unitys-camera-worldtoscreenpoint-causing-ui-elements-to-display-when-object-is-behind-the-camera/

      var min_max = new MinMax3D(min : float.MaxValue, max : float.MinValue);

      var screen_bounds_extents = new Vector3[8];
      var local_bounds = renderer.sharedMesh.bounds;
      var any_point_is_in_front_of_camera = false;

      for (var i = 0; i < 8; i++) {
        var local_space_corner = local_bounds.center
                                 + Vector3.Scale(a : local_bounds.extents, b : _cube_corner_offsets[i]);
        var world_space_corner = renderer.transform.TransformPoint(position : local_space_corner);
        var viewport_space_corner = main_camera.WorldToViewportPoint(position : world_space_corner);

        if (viewport_space_corner.z > 0) {
          any_point_is_in_front_of_camera = true;
        } else {
          // If point is behind camera, it gets flipped to the opposite side
          // So clamp to opposite edge to correct for this
          viewport_space_corner.x = (viewport_space_corner.x <= 0.5f) ? 1 : 0;
          viewport_space_corner.y = (viewport_space_corner.y <= 0.5f) ? 1 : 0;
        }

        // Update bounds with new corner point
        min_max.AddPoint(point : viewport_space_corner);
      }

      // All points are behind camera so just return empty bounds
      if (!any_point_is_in_front_of_camera) {
        return new MinMax3D();
      }

      return min_max;
    }

    internal struct MinMax3D {
      public float _XMin;
      public float _XMax;
      public float _YMin;
      public float _YMax;
      public float _ZMin;
      public float _ZMax;

      public MinMax3D(float min, float max) {
        this._XMin = min;
        this._XMax = max;
        this._YMin = min;
        this._YMax = max;
        this._ZMin = min;
        this._ZMax = max;
      }

      public void AddPoint(Vector3 point) {
        this._XMin = Mathf.Min(a : this._XMin, b : point.x);
        this._XMax = Mathf.Max(a : this._XMax, b : point.x);
        this._YMin = Mathf.Min(a : this._YMin, b : point.y);
        this._YMax = Mathf.Max(a : this._YMax, b : point.y);
        this._ZMin = Mathf.Min(a : this._ZMin, b : point.z);
        this._ZMax = Mathf.Max(a : this._ZMax, b : point.z);
      }
    }
  }
}
