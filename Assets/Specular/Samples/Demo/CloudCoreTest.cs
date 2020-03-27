using UnityEngine;

namespace Specular.Samples.Demo {
  public class CloudCoreTest : MonoBehaviour {
    public float falloffDstHorizontal = 3;
    public float falloffVertical = 1.5f;
    public float maxScale = 1;

    public Vector2 rotSpeedMinMax = new Vector2(10, 20);

    float _rot_speed;

    [HideInInspector] public Transform myTransform;

    void Start() {
      this._rot_speed = Random.Range(min : this.rotSpeedMinMax.x, max : this.rotSpeedMinMax.y);
      this.myTransform = this.transform;
    }

    void Update() {
      this.myTransform.RotateAround(point : this.transform.parent.position,
                                    axis : Vector3.up,
                                    angle : Time.deltaTime * this._rot_speed);
    }
  }
}
