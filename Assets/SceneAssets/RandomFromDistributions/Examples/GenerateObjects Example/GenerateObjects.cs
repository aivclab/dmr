// C# example:

using droid.Runtime.Utilities.Sampling;
using UnityEngine;

namespace RandomFromDistributions.Examples.GenerateObjects_Example {
	[ExecuteInEditMode]
	public class GenerateObjects : MonoBehaviour {

		public GameObject object_to_instantiate;

	
		public float pos_range = 1000f;
	
		public float min_size = 10f;
		public float max_size = 100f;

		public int count = 100;


		public enum SizeRangeType_e { Uniform, LinearRight, LinearLeft, Normal, CurveRight, CurveLeft }
		public SizeRangeType_e size_range_type;

		// Add an option to context menu to run the script!
		[ContextMenu("Generate Objects!")]
		public GameObject Generate () {

			// Create objects grouped by "Generated Objects" GameObject.
			GameObject parent_object = new GameObject("Generated Objects");

			// Create the object under same parent as this script.
			parent_object.transform.parent = this.gameObject.transform.parent;


			// Create the objects with randomized position and scale.
			for (int i = 0; i <= this.count; ++i) {
				GameObject instantiated_obj = Instantiate(this.object_to_instantiate);

				instantiated_obj.transform.position = new Vector3(Random.Range(-this.pos_range, this.pos_range),
				                                                  Random.Range(-this.pos_range, this.pos_range),
				                                                  Random.Range(-this.pos_range, this.pos_range));

				float scale;
				switch (this.size_range_type) {
					case SizeRangeType_e.Uniform :
						scale = Random.Range(this.min_size, this.max_size);
						break;
					case SizeRangeType_e.LinearRight :
						scale = Distributions.RandomRangeLinear(this.min_size, this.max_size, 1.0f);
						break;
					case SizeRangeType_e.LinearLeft :
						scale = Distributions.RandomRangeLinear(this.min_size, this.max_size, -1.0f);
						break;
					case SizeRangeType_e.Normal :
						scale = Distributions.RandomRangeNormalDistribution(this.min_size, this.max_size, Distributions.ConfidenceLevel._999);
						break;
					case SizeRangeType_e.CurveRight :
						scale = Distributions.RandomRangeSlope(this.min_size, this.max_size, 10.0f, Distributions.DirectionE.Right_);
						break;
					case SizeRangeType_e.CurveLeft :
						scale = Distributions.RandomRangeSlope(this.min_size, this.max_size, 10.0f, Distributions.DirectionE.Left_);
						break;
					default :
						scale = Random.Range(this.min_size, this.max_size);
						break;
				}
				instantiated_obj.transform.localScale = new Vector3(scale, scale, scale);

				instantiated_obj.transform.parent = parent_object.transform;
			}

			return parent_object;
		}

	}
}







