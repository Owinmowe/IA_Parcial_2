using System;
using IA.Configurations;
using IA.Managers;
using UnityEngine;

namespace IA.Gameplay
{
    public class Agent : MonoBehaviour, IAgent
    {
        public Action OnAgentStopMoving { get; set; } 
        public Action OnAgentStopActing { get; set; }
        public Vector2Int CurrentPosition { get; set; }
        
        private Genome _genome;
        private NeuralNetwork _brain;
        private float[] _inputs;

        private float _moveInput;
        private float _actionInput;
        
        public void SetTerrainConfiguration(Vector2Int startPosition, TerrainConfiguration configuration)
        {
            _terrainConfiguration = configuration;
            CurrentPosition = startPosition;
        }

        private TerrainConfiguration _terrainConfiguration;
        
        private void Start()
        {
            GameManager.Instance.RegisterAgent(this);
        }

        private void OnDestroy()
        {
            GameManager.Instance.UnRegisterAgent(this);
        }

        public void StartMoving()
        {
            //Think();
            Move(_moveInput);
            OnAgentStopMoving?.Invoke();
        }

        public void StartActing()
        {
            Act(_actionInput);
            OnAgentStopActing?.Invoke();
        }

        public void SetIntelligence(Genome genome, NeuralNetwork brain)
        {
            _genome = genome;
            _brain = brain;
            _inputs = new float[brain.InputsCount];
        }
        
        public void Think()
        {

            _inputs = GameManager.Instance.GetInputs(this);

            float[] output = _brain.Synapsis(_inputs);

            _moveInput = output[0];
            _actionInput = output[1];
        }

        private void Move(float output)
        {
            if (output < -.6f)
            {
                transform.position = _terrainConfiguration.GetPostMovementPosition(this, Movement.MoveDirection.Down);
            }
            else if (output < -.2f)
            {
                transform.position = _terrainConfiguration.GetPostMovementPosition(this, Movement.MoveDirection.Up);
            }
            else if (output < .2f)
            {
                transform.position = _terrainConfiguration.GetPostMovementPosition(this, Movement.MoveDirection.Left);
            }
            else if (output < .6f)
            {
                transform.position = _terrainConfiguration.GetPostMovementPosition(this, Movement.MoveDirection.Right);
            }
        }
        
        private void Act(float output)
        {
            if (output < 0)
            {
                
            }
            else
            {
                
            }
        }
        
    }
}
