//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;

namespace DungeonArchitect.Utils
{
    /// <summary>
    /// A random stream based on normal distribution. Also support uniform distsribution
    /// </summary>
    public class PMRandom
    {
        const int A = 16807;
        const int M = 2147483647;
        
        uint seed = 0;
        Random random = new Random();
		public Random UniformRandom {
			get {
				return random;
			}
		}


        /// <summary>
        /// Creates a new random stream with seed 0
        /// </summary>
		public PMRandom() 
		{ 
			Initialize(0);
		}

        /// <summary>
        /// Creates a new random stream with the specified seed
        /// </summary>
        /// <param name="seed">The seed to initialize the random stream</param>
        public PMRandom(uint seed)
        {
			Initialize(seed);
        }

        /// <summary>
        /// Initializes the stream with the given seed
        /// </summary>
        /// <param name="seed"></param>
		public void Initialize(uint seed) {
			this.seed = seed;
			random = new Random((int)this.seed);
		}

        // http://stackoverflow.com/a/218600
        /// <summary>
        /// Gets the next random number from a uniform distribution
        /// </summary>
        /// <returns>Random number from a uniform stream</returns>
        public float NextGaussianFloat()
        {
            double u1 = random.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)

            return (float)randStdNormal;
        }

        /// <summary>
        /// Gets the next random number from a uniform distribution
        /// </summary>
        /// <param name="mean">The mean used for the normal distribution</param>
        /// <param name="stdDev">The standard deviation used for the normal distribution</param>
        /// <returns>The resulting random number from the normal distributed random stream</returns>
        public float NextGaussianFloat(float mean, float stdDev)
        {
            return mean + stdDev * NextGaussianFloat(); 
        }

        public UnityEngine.Vector2 RandomPointOnCircle()
        {
            float angle = GetNextUniformFloat() * UnityEngine.Mathf.PI * 2;
            return new UnityEngine.Vector2(UnityEngine.Mathf.Cos(angle), UnityEngine.Mathf.Sin(angle));
        }

        /// <summary>
        /// Gets a random number from the uniformly distributed stream
        /// </summary>
        /// <returns></returns>
        public float GetNextUniformFloat()
        {
			return (float)random.NextDouble();
        }
    }
}
