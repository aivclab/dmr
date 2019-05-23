
using droid.Runtime.Utilities.Sampling;

namespace RandomFromDistributions.Examples.DistributionsExamples {
	public class ExponentialTestScript : DistributionTestScript {

		public Distributions.DirectionE direction = Distributions.DirectionE.Right_;

		public float exponent = 2.0f;
	
		protected override float GetRandomNumber(float min, float max) {
		
			return Distributions.RandomRangeExponential(min,max, this.exponent, this.direction);
		}

	}
}
