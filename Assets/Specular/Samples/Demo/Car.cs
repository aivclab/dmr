using Specular.Runtime.Scripts.Portal;
using UnityEngine;

namespace Specular.Samples.Demo {
  public class Car : PortalTraveller {
    public float maxSpeed = 1;
    float _speed;
    float _target_speed;
    float _smooth_v;

    void Start() {
      Debug.Log("Press C to stop/start car");
      this._target_speed = this.maxSpeed;
    }

    void Update() {
      var move_dst = Time.deltaTime * this._speed;
      this.transform.position += this.transform.forward * Time.deltaTime * this._speed;

      if (Input.GetKeyDown(key : KeyCode.C)) {
        this._target_speed = (this._target_speed == 0) ? this.maxSpeed : 0;
      }

      this._speed = Mathf.SmoothDamp(current : this._speed,
                                     target : this._target_speed,
                                     currentVelocity : ref this._smooth_v,
                                     .5f);
    }
  }
}
