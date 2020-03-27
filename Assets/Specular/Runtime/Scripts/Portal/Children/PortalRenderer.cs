using System;
using UnityEngine;
using Object = System.Object;

namespace Specular.Runtime.Scripts.Portal {
  [RequireComponent(requiredComponent : typeof(MeshRenderer), requiredComponent2 : typeof(MeshFilter))]
  [DisallowMultipleComponent]
  public class PortalRenderer : MonoBehaviour {
    MeshRenderer _mesh_renderer;
    MeshFilter _screen_mesh_filter;
    RenderTexture _view_texture;
    static readonly int _main_tex = Shader.PropertyToID("_MainTex");

    void Awake() {
      this._mesh_renderer = this.GetComponent<MeshRenderer>();
      this._screen_mesh_filter = this.GetComponent<MeshFilter>();
      this._mesh_renderer.material.SetInt(nameID : Portal._Active, 1);
    }

    internal void CreateViewTexture(Portal this_portal) {
      if (this._view_texture == null
          || this._view_texture.width != Screen.width
          || this._view_texture.height != Screen.height) {
        if (this._view_texture != null) {
          this._view_texture.Release();
        }

        this._view_texture = new RenderTexture(width : Screen.width, height : Screen.height, 0);
        // Render the view from the portal camera to the view texture
        this_portal._Specular_Camera.targetTexture = this._view_texture;
        // Display the view texture on the screen of the linked portal
        this_portal.linkedPortal.PortalRenderer.MeshRenderer.material.SetTexture(nameID : _main_tex,
                                                                                 value : this._view_texture);
      }
    }

    internal MeshRenderer MeshRenderer { get { return this._mesh_renderer; } }

    internal MeshFilter ScreenMeshFilter { get { return this._screen_mesh_filter; } }
    public float Thickness { get { return this.transform.localScale.z; } }
  }
}
