using UnityEngine;

namespace RandomFromDistributions.Examples.DistributionsExamples {
	public abstract class DistributionTestScript : MonoBehaviour {

		public bool update = false;

		public int repetitions = 1000;
		public int min = -25;
		public int max = 25;

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
			var buckets = new int[this.max+1 - this.min]; // add one, because RandomRangeNormalDistribution is inclusive.
			for (var i = 0; i < buckets.Length; ++i) {
				buckets[i] = 0;
			}
		
			for (var i = 0; i < this.repetitions; ++i) {
				var bucket = this.GetRandomNumber(this.min, this.max);
			
				buckets[Mathf.RoundToInt(bucket) - this.min] ++;
			}
		
			// Display how many times each bucket was drawn by creating a bunch of dots in the scene. 
			var graph = new GameObject("Graph");
			graph.transform.parent = this.transform;
			graph.transform.localPosition = new Vector3(0,0,0);
			for (var i = this.min; i < this.max; ++i) {

				float height = buckets[i- this.min]+1;
				var new_dot = GameObject.CreatePrimitive(PrimitiveType.Cube);
				new_dot.transform.parent = graph.transform;
				new_dot.transform.localPosition = new Vector3(i, height/2.0f, 0);
				new_dot.transform.localScale = new Vector3(1, height, 1);
			}
			return graph;	
		}

		protected abstract float GetRandomNumber(float min, float max);
	}
}
