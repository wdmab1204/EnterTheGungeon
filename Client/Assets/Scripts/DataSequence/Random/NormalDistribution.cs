using System;

namespace GameEngine.DataSequence.Random
{
    public class NormalDistribution
    {
        private System.Random random;
        public double mean, stdev;

        public NormalDistribution(System.Random random, double mean, double stdev)
        {
            this.random = random;
            this.mean = mean;
            this.stdev = stdev;
        }

        public double NextDouble()
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdev * randStdNormal;
        }
    }
}