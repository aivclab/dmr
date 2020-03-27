using UnityEngine;

namespace Specular.Samples.Demo {
  public class Spawner : MonoBehaviour {
    public bool spawnAtStart;
    public GameObject prefab;

    void Start() {
      Debug.Log("Press Space to spawn cubes");
      if (this.spawnAtStart) {
        this.Spawn();
      }
    }

    void Update() {
      if (Input.GetKeyDown(key : KeyCode.Space)) {
        this.Spawn();
      }
    }

    void Spawn() {
      Instantiate(original : this.prefab,
                  position : this.transform.position,
                  rotation : this.transform.rotation);
    }
  }
}
