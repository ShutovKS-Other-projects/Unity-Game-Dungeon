//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Ticks every few milli-seconds
    /// </summary>
    public class Timer
    {
        private float hertz;
        /// <summary>
        /// Ticks per second
        /// </summary>
        public float Hertz
        {
            get
            {
                return hertz;
            }
            set
            {
                hertz = value;
                if (hertz == 0)
                {
                    hertz = 1e-6f;
                }
                frameTime = 1.0f / hertz;
            }
        }

        float frameTime;
        float timeSinceFrameStart = 0;
        public delegate void OnTick(float elapsedTime);
        public event OnTick Tick;

        public Timer()
        {
            Hertz = 30;
        }

        /// <summary>
        /// Update should be called once per frame
        /// </summary>
        /// <param name="deltaSeconds">The frame time between calls</param>
        public void Update(float deltaSeconds)
        {
            timeSinceFrameStart += deltaSeconds;
            if (timeSinceFrameStart >= frameTime)
            {
                timeSinceFrameStart = 0;
                if (Tick != null)
                {
                    Tick(frameTime);
                }
            }
        }
    }
}
