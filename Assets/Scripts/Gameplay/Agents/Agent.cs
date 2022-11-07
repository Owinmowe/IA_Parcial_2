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
            var randomDirection = Movement.GetRandomDirection();

            transform.position = _terrainConfiguration.GetPostMovementPosition(this, randomDirection);
            
            OnAgentStopMoving?.Invoke();
        }

        public void StartActing()
        {
            Debug.Log(gameObject.name + ": Act Start");
            OnAgentStopActing?.Invoke();
        }

    }
}
