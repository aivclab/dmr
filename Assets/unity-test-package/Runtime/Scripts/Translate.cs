using UnityEngine;

namespace Runtime.Scripts {
  /// <summary>
  ///
  /// </summary>
  public class Translate : MonoBehaviour {
    /// <summary>
    ///
    /// </summary>
    [SerializeField]
   float Speed = 1.0f;


    [SerializeField] Vector3 _translation = Vector3.left;

    Vector3 _base_position;
    float _total_time = 0.0f;


    void Start() {
      this._base_position = this.transform.position;
      this._total_time = 0;
    }

    void Update() {
      this._total_time += Time.deltaTime * this.Speed;
      this.transform.position =
          this._base_position + Mathf.Sin(this._total_time) * this._translation;
    }
  }
}
