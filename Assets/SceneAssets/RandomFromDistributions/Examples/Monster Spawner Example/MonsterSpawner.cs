﻿using System.Collections.Generic;
using droid.Runtime.Sampling;
using UnityEngine;

namespace RandomFromDistributions.Examples.Monster_Spawner_Example {
	public class MonsterSpawner : MonoBehaviour {

		public GameObject[] monsters;    // These need to have the same size in order to function correctly.
		public List<float> frequencies;  // Think of them together as being like a Map<GameObect,float> (but maps don't work in editor).

		public int maxNumMonsters;
		private int currNumMonsters = 0;

		// Update is called once per frame
		void Update () {
			if (this.currNumMonsters < this.maxNumMonsters) {
				var index = Distributions.RandomChoiceFollowingDistribution(this.frequencies);

				var monster = Instantiate(this.monsters[index]);
				this.currNumMonsters++;

				// Randomize position
				var x = Distributions.RandomRangeNormalDistribution(-5,5, Distributions.ConfidenceLevel._90);
				var y = Distributions.RandomRangeNormalDistribution(-5,5, Distributions.ConfidenceLevel._90);

				monster.transform.position = new Vector3(x,y,0);
			}
		}

		public void RemoveMonster() {
			this.currNumMonsters--;
		}
	}
}
