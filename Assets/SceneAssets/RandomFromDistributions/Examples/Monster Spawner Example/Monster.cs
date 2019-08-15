using UnityEngine;

namespace RandomFromDistributions.Examples.Monster_Spawner_Example {
	/// <summary>
	///
	/// </summary>
	public class Monster : MonoBehaviour {

		float lifetime = 0;
		public float SecondsToLive = 3;

		// Update is called once per frame
		void Update () {
			this.lifetime += Time.deltaTime;
			if (this.lifetime >= this.SecondsToLive) {
				FindObjectOfType<MonsterSpawner>().RemoveMonster();
				Destroy(this.gameObject);
			}
		}
	}
}
