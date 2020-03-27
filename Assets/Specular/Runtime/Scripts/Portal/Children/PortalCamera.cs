using UnityEngine;

namespace Specular.Runtime.Scripts.Portal {
  [RequireComponent(requiredComponent : typeof(Camera))]
  [DisallowMultipleComponent]
  public class PortalCamera : MonoBehaviour {
    [SerializeField] Camera _camera;

    void Awake() {
      this._camera = this.GetComponent<Camera>();
      this._camera.enabled = false;
    }

    public Camera CameraComponent { get { return this._camera; } }
  }
}
