using System.Collections.Generic;
using droid.Runtime.Utilities.Sampling;
using UnityEngine;

namespace RandomFromDistributions.Examples.DistributionsExamples {
	public class CustomPDFTestScript : MonoBehaviour {

		public bool update = false;
	
		public int repetitions = 1000;
		public int width = 25;
	
		private GameObject graph;
	
		// Use this for initialization
		void Start() {
			this.graph = this.CreateGraph();
		}
	
		// Update is called once per frame
		void Update () {
			if (this.update) {
				Destroy(this.graph);
				this.graph = this.CreateGraph();
			}
		}
	
	
		private GameObject CreateGraph () {
			int[] buckets = new int[this.distribution.Length]; // add one, because RandomRangeNormalDistribution is inclusive.
			for (int i = 0; i < buckets.Length; ++i) {
				buckets[i] = 0;
			}
		
			for (int i = 0; i < this.repetitions; ++i) {
				float bucket = this.GetRandomNumber();
			
				buckets[Mathf.RoundToInt(bucket)] ++;
			}
		
			// Display how many times each bucket was drawn by creating a bunch of dots in the scene. 
			GameObject graph = new GameObject("Graph");
			graph.transform.parent = this.transform;
			graph.transform.localPosition = new Vector3(0,0,0);
			for (int i = 0; i < this.distribution.Length; ++i) {
			
				float height = buckets[i]+1;
				GameObject new_dot = GameObject.CreatePrimitive(PrimitiveType.Cube);
				new_dot.transform.parent = graph.transform;
				new_dot.transform.localPosition = new Vector3(i* this.width, height/2.0f, 0);
				new_dot.transform.localScale = new Vector3(this.width, height, 1);
			}
			return graph;	
		}


		public float[] distribution;
	
		protected float GetRandomNumber() {
		
			return Distributions.RandomChoiceFollowingDistribution(new List<float>(this.distribution));
		}

	}
}
