using System;

namespace IA.Gameplay
{
    public interface IAgent
    {
        void StartMoving(bool instant = false);
        Action OnAgentStopMoving { get; set; }
        void StartActing(bool instant = false);
        Action OnAgentStopActing { get; set; }
        void Think();
    }
}
