using IA.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace IA.UI
{
    public class UIPreGameplay : MonoBehaviour
    {

        [SerializeField] private GameObject panelGameplay;
        [SerializeField] private Button simulateButton;

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
        }
    }
}

