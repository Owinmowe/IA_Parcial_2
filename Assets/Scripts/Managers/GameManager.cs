using System;
using System.Collections.Generic;
using UnityEngine;
using IA.Gameplay;
using IA.Configurations;
using Unity.VisualScripting;

namespace IA.Managers
{
    public class GameManager : MonoBehaviour
    {

        public Action<int> OnTurnEnd;
        public Action<int> OnGenerationEnd;

        public static GameManager Instance { get; private set; } = null;
        
        [SerializeField] private GameplayConfiguration gameplayConfiguration = default;
        
        public GameplayConfiguration GameplayConfig => gameplayConfiguration;
        public GenomeData RedGenomeData { get; private set; } = new GenomeData();
        public GenomeData GreenGenomeData { get; private set; } = new GenomeData();
        public bool AnimationsOn { get; set; } = false;
        public bool Started { get; private set; } = false;
        public bool Paused { get; set; } = true;
        
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
        private bool _agentsTime = false;
        
        private TimeManager _timeManager;
        private AgentsManager _agentsManager;
        private GenerationManager _generationManager;
        
        private int _currentTurn = 0;
        private int _currentGeneration = 0;

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
            GameObject terrainParent = new GameObject();
            terrainParent.transform.SetParent(transform);
            terrainParent.name = "Terrain Parent";
            
            gameplayConfiguration.CreateTerrain(terrainParent.transform);
            
            _timeManager.onActionTimeReached += StartAllAgentsMovingTime;
            _timeManager.SetCyclesAmount(gameplayConfiguration.TurnsPerGeneration);
            
            _agentsManager.onAllAgentsStoppedMoving += StartAllAgentsActionTime;
            _agentsManager.onAllAgentsStoppedActing += AllAgentsStoppedActing;
        }
        private void Update()
        {
            if (!Paused && !_agentsTime && Started) _timeManager.Update(Time.deltaTime);
        }
        private void OnDestroy()
        {
            _timeManager.onActionTimeReached -= StartAllAgentsMovingTime;
        }

        
        public void StartSimulation()
        {
            Started = true;
            Paused = false;
            _agentsTime = false;
            
            gameplayConfiguration.CreateAgents(GreenGenomeData.populationCount, RedGenomeData.populationCount);
            gameplayConfiguration.CreateFood();
            
            CreateBrainsWithSavedData();
        }

        private void CreateBrainsWithSavedData()
        {
            var botAgents = gameplayConfiguration.GreenAgentsList;
            foreach (var agent in botAgents)
            {
                _generationManager.CreateAgentBrains(agent, GreenGenomeData);
            }

            var topAgents = gameplayConfiguration.RedAgentsList;
            foreach (var agent in topAgents)
            {
                _generationManager.CreateAgentBrains(agent, RedGenomeData);
            }
        }
        public void StopSimulation()
        {
            Started = false;
            Paused = true;
            gameplayConfiguration.ClearAllAgentsAndFood();
            
            _currentTurn = 0;
            _currentGeneration = 0;
        }

        public float[] GetInputs(Agent agent)
        {
            var inputs = new float[4];

            var closestFoodPosition = gameplayConfiguration.GetClosestFood(agent.CurrentPosition);

            inputs[0] = agent.CurrentPosition.x;
            inputs[1] = agent.CurrentPosition.y;
            inputs[2] = closestFoodPosition.x;
            inputs[3] = closestFoodPosition.y;
            
            return inputs;
        }
        
        private void StartAllAgentsMovingTime(bool instant)
        {
            _agentsTime = true;
            _agentsManager.StartAllAgentsMoving(instant);
        }
        
        private void StartAllAgentsActionTime()
        {
            _agentsManager.StartAllAgentsActing();
        }
        
        private void AllAgentsStoppedActing()
        {
            _currentTurn++;
            if (_currentTurn > gameplayConfiguration.TurnsPerGeneration)
            {
                _currentTurn = 0;
                _currentGeneration++;

                List<GenerationManager.AgentGenerationData> greenGenerationData =
                    _generationManager.GetNewGenerationData(GreenGenomeData, gameplayConfiguration.GreenAgentsList);
                
                List<GenerationManager.AgentGenerationData> redGenerationData =
                    _generationManager.GetNewGenerationData(RedGenomeData, gameplayConfiguration.RedAgentsList);
                    
                gameplayConfiguration.ClearAllAgentsAndFood();
                    
                gameplayConfiguration.CreateAgents(greenGenerationData, redGenerationData);
                gameplayConfiguration.CreateFood();
                
                OnGenerationEnd?.Invoke(_currentGeneration);
            }

            _agentsTime = false;
            OnTurnEnd?.Invoke(_currentTurn);
        }
        
        public void RegisterAgent(IAgent agent) => _agentsManager.RegisterAgent(agent);
        public void UnRegisterAgent(IAgent agent) => _agentsManager.UnRegisterAgent(agent);

    }
}
