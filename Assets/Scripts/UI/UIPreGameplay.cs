using IA.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IA.UI
{
    public class UIPreGameplay : MonoBehaviour
    {

        [SerializeField] private GameObject panelGameplay;
        
        [Header("Pre Gameplay UI")]
        [SerializeField] private Button simulateButton;
        [SerializeField] private TextMeshProUGUI turnsPerGenerationText;
        [SerializeField] private Slider turnsPerGenerationSlider;

        private void Awake()
        {
            simulateButton.onClick.AddListener(delegate
            {
                GameManager.Instance.StartSimulation();
                panelGameplay.SetActive(true);
                gameObject.SetActive(false);
            });
        }

        private void Start()
        {
            GameManager.Instance.SimulationSpeed = 1f;
            SetTurnsPerGenerationUI();
        }

        private void SetTurnsPerGenerationUI()
        {
            turnsPerGenerationSlider.wholeNumbers = true;
            
            int currentTurnsPerGeneration = GameManager.Instance.GameplayConfig.TurnsPerGeneration;
            turnsPerGenerationSlider.SetValueWithoutNotify(currentTurnsPerGeneration);
            turnsPerGenerationText.text = "Turns Per Generation: " + currentTurnsPerGeneration;
            
            turnsPerGenerationSlider.onValueChanged.AddListener(delegate(float value)
            {
                GameManager.Instance.GameplayConfig.TurnsPerGeneration = (int)value;
                turnsPerGenerationText.text = "Turns Per Generation: " + value;
            });
        }
        
    }
}

