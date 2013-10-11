using System;

namespace CastleMaster.Graphics
{
    public class TimerHelper
    {
        private int currentFrame, minFrame, maxFrame, currentTicks, ticksPerFrame;
        private bool isRunning = false, runOnce;

        public event Action RoundEnded;
        public event Action AnimationEnded;

        public TimerHelper(int ticksPerFrame, int maxFrame, bool runOnce = false, int minFrame = 0, int currentFrame = 0)
        {
            this.ticksPerFrame = ticksPerFrame;
            this.maxFrame = maxFrame;
            this.minFrame = minFrame;
            this.currentFrame = currentFrame < minFrame ? minFrame : currentFrame;
            this.runOnce = runOnce;
            currentTicks = 0;
        }

        public bool IsRunning { get { return isRunning; } }

        public int CurrentFrame { get { return currentFrame; } }

        public TimerHelper Start()
        {
            if (!isRunning)
                isRunning = true;
            return this;
        }

        public void Stop()
        {
            isRunning = false;
        }

        public void Reset()
        {
            currentFrame = minFrame;
            currentTicks = 0;
        }

        public void Restart()
        {
            isRunning = true;
            currentFrame = minFrame;
            currentTicks = 0;
        }

        public void UpdateStep()
        {
            if (isRunning)
            {
                currentTicks++;

                if (currentTicks > ticksPerFrame)
                {
                    currentFrame++;
                    if (currentFrame > maxFrame)
                    {
                        if (runOnce)
                        {
                            Stop();
                            if (AnimationEnded != null)
                                AnimationEnded();
                        }
                        else
                        {
                            currentFrame = minFrame;
                            if (RoundEnded != null)
                                RoundEnded();
                        }
                    }

                    currentTicks = 0;
                }
            }
        }
    }
}
