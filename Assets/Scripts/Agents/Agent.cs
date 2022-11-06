using System;
using IA.Managers;
using UnityEngine;

namespace IA.Agents
{
    public class Agent : MonoBehaviour, IAgent
    {
        public Action OnAgentStopMoving { get; set; } 
        public Action OnAgentStopActing { get; set; }
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
            Debug.Log( gameObject.name + ": Move Start");
            OnAgentStopMoving?.Invoke();
        }

        public void StartActing()
        {
            Debug.Log(gameObject.name + ": Act Start");
            OnAgentStopActing?.Invoke();
        }

    }
}
