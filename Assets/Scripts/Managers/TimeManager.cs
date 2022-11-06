using System;

namespace IA.Managers
{
    public class TimeManager
    {
        public Action onActionTimeReached;
        public Action onCycleEnd;
        public float TimeBetweenTicks { get; set; } = 1.0f;

        private float _currentTime = 0.0f;
        
        public void Update(float deltaTime)
        {
            _currentTime += deltaTime;
            while (_currentTime > TimeBetweenTicks)
            {
                onActionTimeReached?.Invoke();
                _currentTime -= TimeBetweenTicks;
            }
            onCycleEnd?.Invoke();
        }
    }
}
