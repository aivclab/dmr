using Specular.Runtime.Scripts.Portal;
using UnityEngine;

namespace Specular.Samples.Demo {
  [RequireComponent(requiredComponent : typeof(Rigidbody))]
  public class PortalPhysicsObject : PortalTraveller {
    public float force = 10;
    new Rigidbody _rigidbody;
    public Color[] colors;
    static int _i;

    void Awake() {
      this._rigidbody = this.GetComponent<Rigidbody>();
      this.graphicsObject.GetComponent<MeshRenderer>().material.color = this.colors[_i];
      _i++;
      if (_i > this.colors.Length - 1) {
        _i = 0;
      }
    }

    public override void Teleport(Transform from_portal, Transform to_portal, Vector3 pos, Quaternion rot) {
      base.Teleport(from_portal : from_portal,
                    to_portal : to_portal,
                    pos : pos,
                    rot : rot);
      this._rigidbody.velocity =
          to_portal.TransformVector(vector : from_portal.InverseTransformVector(vector : this
                                                                                         ._rigidbody
                                                                                         .velocity));
      this._rigidbody.angularVelocity =
          to_portal.TransformVector(vector : from_portal.InverseTransformVector(vector : this
                                                                                         ._rigidbody
                                                                                         .angularVelocity));
    }
  }
}
