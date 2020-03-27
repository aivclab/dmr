using UnityEngine;

namespace Specular.Samples.Demo {
  public class CloudBehaviorTest : MonoBehaviour {
    public Vector2 rotSpeedMinMax = new Vector2(10, 20);

    float _rot_speed;

    CloudCoreTest[] _cloud_centres;
    Transform _my_transform;

    void Start() {
      this._rot_speed = Random.Range(min : this.rotSpeedMinMax.x, max : this.rotSpeedMinMax.y);
      this._cloud_centres = FindObjectsOfType<CloudCoreTest>();
      this._my_transform = this.transform;
    }

    void Update() {
      this._my_transform.RotateAround(point : this.transform.parent.position,
                                      axis : Vector3.up,
                                      angle : Time.deltaTime * this._rot_speed);
      float max_scale = 0;
      for (var i = 0; i < this._cloud_centres.Length; i++) {
        var cloud_centre = this._cloud_centres[i];
        var offset = (this._my_transform.position - cloud_centre.transform.position);
        var sqr_dst_horizontal = offset.x * offset.x + offset.z * offset.z;
        var sqr_dst_vertical = offset.y * offset.y;
        var t_h = 1
                  - Mathf.Min(1,
                              b : sqr_dst_horizontal
                                  / (cloud_centre.falloffDstHorizontal * cloud_centre.falloffDstHorizontal));
        var t_v = 1
                  - Mathf.Min(1,
                              b : sqr_dst_vertical
                                  / (cloud_centre.falloffVertical * cloud_centre.falloffVertical));
        //float t = 1 - Mathf.Min (1, sqrDst / (falloffDst * falloffDst));
        max_scale = Mathf.Max(a : max_scale, b : t_v * t_h * cloud_centre.maxScale);
      }

      this._my_transform.localScale = Vector3.one * max_scale;
    }
  }
}
