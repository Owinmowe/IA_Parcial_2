using System;
using IA.Configurations;
using UnityEngine;

namespace IA.Gameplay
{
    public interface IAgent
    {
        void StartMoving();
        Action OnAgentStopMoving { get; set; }
        void StartActing();
        Action OnAgentStopActing { get; set; }
    }
}
