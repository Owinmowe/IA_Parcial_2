using System;
using System.Collections.Generic;
using UnityEngine;
using IA.Agents;

namespace IA.Managers
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; private set; } = null;

        public bool Simulating { get; set; } = false;

        public float SimulationSpeed
        {
            get => _simulationSpeed;
            set
            {
                _simulationSpeed = value;
                _timeManager.TimeBetweenTicks = 1.0f / _simulationSpeed;
            }
        }
        private float _simulationSpeed = 1f;
        
        private Action _onMoveAllGrid;
        private Action _onActionAllGrid;

        private TimeManager _timeManager;

        private List<IAgent> _agentsList = new List<IAgent>();
        private int _movedAgentsCount = 0;
        private int _actedAgentsCount = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            _timeManager = new TimeManager();
        }

        private void Start()
        {
            _timeManager.onActionTimeReached += StartAllAgentsMovingTime;
        }

        private void OnDestroy()
        {
            _timeManager.onActionTimeReached -= StartAllAgentsMovingTime;
        }

        private void Update()
        {
            if (Simulating) _timeManager.Update(Time.deltaTime);
        }
        
        private void StartAllAgentsMovingTime()
        {
            _onMoveAllGrid?.Invoke();

            _movedAgentsCount = 0;
            foreach (var agent in _agentsList)
            {
                agent.StartMoving();
            }
        }
        
        private void AgentStoppedMoving()
        {
            _movedAgentsCount++;
            if (_movedAgentsCount < _agentsList.Count) return;
            StartAllAgentsActionTime();
        }

        private void StartAllAgentsActionTime()
        {
            _onActionAllGrid?.Invoke();

            _actedAgentsCount = 0;
            foreach (var agent in _agentsList)
            {
                agent.StartActing();
            }
        }
        
        private void AgentStoppedActing()
        {
            _actedAgentsCount++;
            if (_actedAgentsCount < _agentsList.Count) return;
        }
        
        
        public void RegisterAgent(IAgent agent)
        {
            _agentsList.Add(agent);
            agent.OnAgentStopMoving += AgentStoppedMoving;
            agent.OnAgentStopActing += AgentStoppedActing;
        }
        
        public void UnRegisterAgent(IAgent agent)
        {
            agent.OnAgentStopMoving -= AgentStoppedMoving;
            agent.OnAgentStopActing -= AgentStoppedActing;
            _agentsList.Remove(agent);
        }
        
    }
}
