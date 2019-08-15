
using droid.Runtime.Sampling;

namespace RandomFromDistributions.Examples.DistributionsExamples {
	public class CurveTestScript : DistributionTestScript {

		public Distributions.DirectionE direction = Distributions.DirectionE.Right_;

		public float skew = 5.0f;
	
		protected override float GetRandomNumber(float min, float max) {
		
			return Distributions.RandomRangeSlope(min, max, this.skew, this.direction);
		}

	}
}
