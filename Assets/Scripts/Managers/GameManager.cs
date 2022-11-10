using System;
using System.Collections.Generic;
using UnityEngine;
using IA.Gameplay;
using IA.Configurations;

namespace IA.Managers
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private TerrainConfiguration terrainConfiguration = default;
        [SerializeField] private GenomeData genomeData = default;
        
        public static GameManager Instance { get; private set; } = null;
        
        public bool Started { get; set; }
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

        public int TurnsPerGeneration { get; set; } = 30;
        
        private float _simulationSpeed = 1f;
        
        private Action _onMoveAllGrid;
        private Action _onActionAllGrid;

        private TimeManager _timeManager;
        private AgentsManager _agentsManager;
        private GenerationManager _generationManager;
        
        private int _currentTurn = 0;

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
            _agentsManager = new AgentsManager();
            _generationManager = new GenerationManager();
        }

        private void Start()
        {
            _timeManager.onActionTimeReached += StartAllAgentsMovingTime;
            
            _agentsManager.onAllAgentsStoppedMoving += StartAllAgentsActionTime;
            _agentsManager.onAllAgentsStoppedActing += AllAgentsStoppedActing;
        }
        private void Update()
        {
            if (Simulating && Started) _timeManager.Update(Time.deltaTime);
        }
        private void OnDestroy()
        {
            _timeManager.onActionTimeReached -= StartAllAgentsMovingTime;
        }

        
        public void StartSimulation()
        {
            Started = true;
            Simulating = true;
            
            terrainConfiguration.CreateTerrain();
            terrainConfiguration.CreateAgents();
            terrainConfiguration.CreateFood();
            
            var botAgents = terrainConfiguration.BotAgentList;
            foreach (var agent in botAgents)
            {
                _generationManager.CreateAgentBrains(agent, genomeData);
            }
            
            var topAgents = terrainConfiguration.TopAgentList;
            foreach (var agent in topAgents)
            {
                _generationManager.CreateAgentBrains(agent, genomeData);
            }
        }
        
        public void StopSimulation()
        {
            Started = false;
            Simulating = false;
            terrainConfiguration.ClearAllAgentsAndFood();
        }

        public float[] GetInputs(Agent agent)
        {
            var inputs = new float[4];

            var closestFoodPosition = terrainConfiguration.GetClosestFood(agent.CurrentPosition);

            inputs[0] = agent.CurrentPosition.x;
            inputs[1] = agent.CurrentPosition.y;
            inputs[2] = closestFoodPosition.x;
            inputs[3] = closestFoodPosition.y;
            
            return inputs;
        }
        
        private void StartAllAgentsMovingTime()
        {
            _onMoveAllGrid?.Invoke();
            _agentsManager.StartAllAgentsMoving();
        }
        
        private void StartAllAgentsActionTime()
        {
            _onActionAllGrid?.Invoke();
            _agentsManager.StartAllAgentsActing();
        }
        
        private void AllAgentsStoppedActing()
        {
            _currentTurn++;
            if (_currentTurn == TurnsPerGeneration)
            {
                Debug.Log(_currentTurn);
                _currentTurn = 0;
                    
                terrainConfiguration.ClearAllAgentsAndFood();
                    
                terrainConfiguration.CreateAgents();
                terrainConfiguration.CreateFood();
            }
        }
        
        public void RegisterAgent(IAgent agent) => _agentsManager.RegisterAgent(agent);
        public void UnRegisterAgent(IAgent agent) => _agentsManager.UnRegisterAgent(agent);

    }
}
