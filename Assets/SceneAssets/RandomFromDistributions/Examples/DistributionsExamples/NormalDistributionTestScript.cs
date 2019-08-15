
using droid.Runtime.Sampling;

namespace RandomFromDistributions.Examples.DistributionsExamples {
	public class NormalDistributionTestScript : DistributionTestScript {

		public Distributions.ConfidenceLevel conf_level = Distributions.ConfidenceLevel._95;
	

		protected override float GetRandomNumber(float min, float max) {

			return Distributions.RandomRangeNormalDistribution(min, max, this.conf_level);
		}
	
	}
}
