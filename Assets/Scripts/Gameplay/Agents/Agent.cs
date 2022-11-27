using System;
using System.Collections;
using IA.Configurations;
using IA.Managers;
using UnityEngine;

namespace IA.Gameplay
{
    public class Agent : MonoBehaviour, IAgent
    {

        [Header("Agent Animation")] 
        [SerializeField] private float movementSpeed = 1;

        [Header("Fitness Strength")] 
        [SerializeField] private AnimationCurve fitnessCurve;
        
        public Action OnAgentStopMoving { get; set; } 
        public Action OnAgentStopActing { get; set; }
        public Vector2Int CurrentPosition { get; set; }

        public Genome AgentGenome { get; private set; }
        public NeuralNetwork AgentBrain { get; private set; }
        public int GenerationsLived { get; set; }
        public int FoodEaten { get; private set; }
        public float Fitness { get; private set; }
        public bool ActionPositive() => _actionInput > .5f;

        private float[] _inputs;

        private GameplayConfiguration _gameplayConfiguration;
        private Movement.MoveDirection _previousPositionDirection;
        private float _moveInput;
        private float _actionInput;
        private Team _team;

        public void SetPosition(Vector2Int startPosition)
        {
            CurrentPosition = startPosition;
        }

        public void SetTeam(Team team) => _team = team;
        
        private void Start()
        {
            GameManager.Instance.RegisterAgent(this);
            _gameplayConfiguration = GameManager.Instance.GameplayConfig;
        }

        private void OnDestroy()
        {
            GameManager.Instance.UnRegisterAgent(this);
        }

        public void StartMoving(bool instant)
        {
            Think();
            Move(_moveInput, instant);
        }

        public void StartActing(bool instant)
        {
            Act(_actionInput);
            OnAgentStopActing?.Invoke();
        }

        public void SetIntelligence(Genome genome, NeuralNetwork brain)
        {
            AgentGenome = genome;
            AgentBrain = brain;
            _inputs = new float[brain.InputsCount];
        }
        
        public void Think()
        {
            _inputs = GameManager.Instance.GetInputs(this);

            float[] output = AgentBrain.Synapsis(_inputs);

            _moveInput = output[0];
            _actionInput = output[1];
        }

        private void Move(float output, bool instant)
        {
            Vector3 newPosition = transform.position;
            if (output > .8f)
            {
                newPosition = _gameplayConfiguration.GetPostMovementPosition(this, Movement.MoveDirection.Down);
                _previousPositionDirection = Movement.MoveDirection.Up;
            }
            else if (output > .6f)
            {
                newPosition = _gameplayConfiguration.GetPostMovementPosition(this, Movement.MoveDirection.Right);
                _previousPositionDirection = Movement.MoveDirection.Left;
            }
            else if (output > .4f)
            {
                newPosition = _gameplayConfiguration.GetPostMovementPosition(this, Movement.MoveDirection.Left);
                _previousPositionDirection = Movement.MoveDirection.Right;
            }
            else if (output < .2f)
            {
                newPosition = _gameplayConfiguration.GetPostMovementPosition(this, Movement.MoveDirection.Up);
                _previousPositionDirection = Movement.MoveDirection.Down;
            }
            
            if (instant)
            {
                transform.position = newPosition;
                OnAgentStopMoving?.Invoke();
            }
            else
            {
                if (GameManager.Instance.AnimationsOn)
                {
                    StartCoroutine(MoveAnimationCoroutine(newPosition));
                }
                else
                {
                    transform.position = newPosition;
                    OnAgentStopMoving?.Invoke();
                }
            }
            
        }

        private IEnumerator MoveAnimationCoroutine(Vector3 endPosition)
        {
            Vector3 startPosition = transform.position;
            float t = 0;
            while (t < 1)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, t);
                t += Time.deltaTime * movementSpeed;
                yield return null;
            }
            transform.position = endPosition;
            OnAgentStopMoving?.Invoke();
        }
        
        private void Act(float output)
        {
            _gameplayConfiguration.AgentAct(this, _team);
        }

        public void ReturnToPreviousPosition()
        {
            transform.position = _gameplayConfiguration.GetPostMovementPosition(this, _previousPositionDirection);
            
            switch (_previousPositionDirection)
            {
                case Movement.MoveDirection.Down:
                    _previousPositionDirection = Movement.MoveDirection.Up;
                    break;
                
                case Movement.MoveDirection.Left:
                    _previousPositionDirection = Movement.MoveDirection.Right;
                    break;
                
                case Movement.MoveDirection.Right:
                    _previousPositionDirection = Movement.MoveDirection.Left;
                    break;
                
                case Movement.MoveDirection.Up:
                    _previousPositionDirection = Movement.MoveDirection.Down;
                    break;
            }
        }
        
        public void Eat(int bonusFitness)
        {
            FoodEaten++;
            Fitness += bonusFitness * fitnessCurve.Evaluate(FoodEaten);
        }
        
        public enum Team
        {
            Red,
            Green
        }
        
    }
}
