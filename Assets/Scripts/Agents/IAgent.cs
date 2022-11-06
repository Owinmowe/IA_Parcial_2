using System;

namespace IA.Agents
{
    public interface IAgent
    {
        void StartMoving();
        Action OnAgentStopMoving { get; set; }
        void StartActing();
        Action OnAgentStopActing { get; set; }
    }
}
