using IA.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IA.UI
{
    public class UIGameplay : MonoBehaviour
    {
        
        
        [Header("Simulation Flow")]
        [SerializeField] private GameObject panelPreGameplay;
        [SerializeField] private Button playPauseButton;
        [SerializeField] private Button stopSimulationButton;
        
        [Header("Simulation Speed")]
        [SerializeField] private Slider simulationSpeedSlider;
        [SerializeField] private TextMeshProUGUI simulationSpeedText;
        
        private TextMeshProUGUI playPauseButtonText;
        private void Awake()
        {
            playPauseButtonText = playPauseButton.GetComponentInChildren<TextMeshProUGUI>();
            playPauseButtonText.text = "Pause Simulation";
            
            playPauseButton.onClick.AddListener(delegate
            {
                GameManager.Instance.Simulating = !GameManager.Instance.Simulating;
                playPauseButtonText.text = GameManager.Instance.Simulating ? "Pause Simulation" : "Resume Simulation";
            });
            
            stopSimulationButton.onClick.AddListener(delegate
            {
                GameManager.Instance.StopSimulation();
                panelPreGameplay.gameObject.SetActive(true);
                gameObject.SetActive(false);
            });
            
            simulationSpeedText.text = "Simulation Speed: " + GameManager.Instance.SimulationSpeed;
            
            simulationSpeedSlider.onValueChanged.AddListener(delegate(float value)
            {
                GameManager.Instance.SimulationSpeed = value;
                simulationSpeedText.text = "Simulation Speed: " + GameManager.Instance.SimulationSpeed;
            });
            
        }
    }
}
