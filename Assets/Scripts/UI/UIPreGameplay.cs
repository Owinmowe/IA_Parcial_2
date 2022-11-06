using System;
using IA.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace IA.UI
{
    public class UIPreGameplay : MonoBehaviour
    {

        [SerializeField] private Button simulateButton;
        [SerializeField] private Slider simulationSpeedSlider;
        
        private void Awake()
        {
            simulateButton.onClick.AddListener(delegate
            {
                GameManager.Instance.Simulating = !GameManager.Instance.Simulating;
            });
            
            simulationSpeedSlider.onValueChanged.AddListener(delegate(float value)
            {
                GameManager.Instance.SimulationSpeed = value;
            });
        }

        private void Start()
        {
            GameManager.Instance.SimulationSpeed = 1f;
        }
    }
}

