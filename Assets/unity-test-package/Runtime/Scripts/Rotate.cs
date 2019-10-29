using UnityEngine;

namespace Runtime.Scripts {
  /// <inheritdoc />
  ///  <summary>
  ///  </summary>
  class Rotate : MonoBehaviour {
    [SerializeField] float angularVelocity = 10;
    [SerializeField] Vector3 axis = Vector3.up;

    void Update() {
      var rot = Quaternion.AngleAxis(this.angularVelocity * Time.deltaTime, this.axis);
      this.transform.localRotation *= rot;
    }
  }
}
