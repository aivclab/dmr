using UnityEngine;

namespace Specular.Runtime.Scripts {
  public class SpecularReceiver : MonoBehaviour {
    SpecularCamera[] _speculars;
    protected Camera _Specular_Receiver;

    void Awake() {
      this._Specular_Receiver = Camera.main;
      this._speculars = FindObjectsOfType<SpecularCamera>();
    }

    void OnPreCull() {
      for (var i = 0; i < this._speculars.Length; i++) {
        this._speculars[i].PreSpecularRender(receiver : this._Specular_Receiver);
      }

      for (var i = 0; i < this._speculars.Length; i++) {
        this._speculars[i].SpecularRender(receiver : this._Specular_Receiver);
      }

      for (var i = 0; i < this._speculars.Length; i++) {
        this._speculars[i].PostSpecularRender(receiver : this._Specular_Receiver);
      }
    }
  }
}
