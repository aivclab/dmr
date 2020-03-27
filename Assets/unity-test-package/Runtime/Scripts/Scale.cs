using UnityEngine;

namespace Runtime.Scripts {
  /// <summary>
  ///
  /// </summary>
  public class Scale : MonoBehaviour {
    /// <summary>
    ///
    /// </summary>
    [SerializeField]
   float Speed = 1.0f;

    [SerializeField] Vector3 _scalar = Vector3.left;

    Vector3 _base_scale;
    float _total_time = 0.0f;


    void Start() {
      this._base_scale = this.transform.position;
      this._total_time = 0;
    }

    void Update() {
      this._total_time += Time.deltaTime * this.Speed;
      this.transform.localScale =
          this._base_scale + Mathf.Sin(f : this._total_time) * this._scalar;
    }
  }
}
