
using droid.Runtime.Sampling;

namespace RandomFromDistributions.Examples.DistributionsExamples {
	public class LinearTestScript : DistributionTestScript {
	
		public float slope = 1.0f;
	
		protected override float GetRandomNumber(float min, float max) {
		
			return Distributions.RandomRangeLinear(min, max, this.slope);
		}

	}
}
