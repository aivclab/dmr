using UnityEngine;

namespace Specular.Samples.Demo {
  public class CloudTest : MonoBehaviour {
    public int numViewDirections = 100;
    public int numClouds = 10;
    public int cloudSpawnSeed;
    public bool randomizeCloudSeed;

    public float spawnRadius = 10;
    [Range(0, 1)] public float startHeight;
    public GameObject cloudPrefab;
    public GameObject cloudCorePrefab;

    void Start() {
      var golden_ratio = (1 + Mathf.Sqrt(5)) / 2;
      var angle_increment = Mathf.PI * 2 * golden_ratio;

      for (var i = 0; i < this.numViewDirections; i++) {
        var t = (float)i / this.numViewDirections;
        var inclination = Mathf.Acos(f : 1 - (1 - this.startHeight) * t);
        var azimuth = angle_increment * i;

        var x = Mathf.Sin(f : inclination) * Mathf.Sin(f : azimuth);
        var y = Mathf.Cos(f : inclination);
        var z = Mathf.Sin(f : inclination) * Mathf.Cos(f : azimuth);

        var g = Instantiate(original : this.cloudPrefab,
                            position : this.transform.position
                                       + new Vector3(x : x, y : y, z : z) * this.spawnRadius,
                            rotation : Quaternion.identity,
                            parent : this.transform);
      }

      if (this.randomizeCloudSeed) {
        this.cloudSpawnSeed = Random.Range(-10000, 10000);
      }

      var prng = new System.Random(Seed : this.cloudSpawnSeed);

      for (var i = 0; i < this.numClouds; i++) {
        var t = (float)prng.NextDouble();
        var inclination = Mathf.Acos(f : 1 - (1 - this.startHeight) * t);
        var azimuth = angle_increment * i;

        var x = Mathf.Sin(f : inclination) * Mathf.Sin(f : azimuth);
        var y = Mathf.Cos(f : inclination);
        var z = Mathf.Sin(f : inclination) * Mathf.Cos(f : azimuth);

        var g = Instantiate(original : this.cloudCorePrefab,
                            position : this.transform.position
                                       + new Vector3(x : x, y : y, z : z) * this.spawnRadius,
                            rotation : Quaternion.identity,
                            parent : this.transform);
      }
    }
  }
}
