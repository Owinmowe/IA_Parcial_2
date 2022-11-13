using System;
using IA.Managers;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IA.UI
{
    public class UIPanelSide : MonoBehaviour
    {
        
        [Space(10)]
        [SerializeField] private GenomeTeam team;
        [Space(10)]
        
        [SerializeField] private IntValueConfigurationSlider populationConfiguration;
        [SerializeField] private IntValueConfigurationSlider inputConfiguration;
        [SerializeField] private IntValueConfigurationSlider hiddenLayerConfiguration;
        [SerializeField] private IntValueConfigurationSlider outputsConfiguration;
        [SerializeField] private IntValueConfigurationSlider neuronsPerHiddenLayerConfiguration;
        [SerializeField] private FloatValueConfigurationSlider biasConfiguration;
        [SerializeField] private FloatValueConfigurationSlider sigmoidConfiguration;
        [SerializeField] private FloatValueConfigurationSlider mutationRateConfiguration;
        [SerializeField] private FloatValueConfigurationSlider mutationChanceConfiguration;

        private GenomeData _data;
        private void Start()
        {
            _data = team == GenomeTeam.Green ? GameManager.Instance.GreenGenomeData : GameManager.Instance.RedGenomeData;

            SetPopulationCountSlider();
            SetInputCountSlider();
            SetHiddenLayerCountSlider();
            SetOutputsCountSlider();
            SetNeuronsPerHiddenLayerCountSlider();
            SetBiasValueSlider();
            SetSigmoidSlopeSlider();
            SetMutationRateSlider();
            SetMutationChanceSlider();
        }

        private void SetPopulationCountSlider()
        {
            populationConfiguration.SetSlider(null, null, GameManager.Instance.GameplayConfig.TerrainCount.x);

            populationConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.populationCount = (int)value;
            });
            _data.populationCount = populationConfiguration.defaultValue;
        }

        private void SetInputCountSlider()
        {
            inputConfiguration.SetSlider(null);

            inputConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.inputsCount = (int)value;
            });
            _data.inputsCount = inputConfiguration.defaultValue;
        }
        
        private void SetHiddenLayerCountSlider()
        {
            hiddenLayerConfiguration.SetSlider(null);

            hiddenLayerConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.hiddenLayers = (int)value;
            });
            _data.hiddenLayers = hiddenLayerConfiguration.defaultValue;
        }
        
        private void SetOutputsCountSlider()
        {
            outputsConfiguration.SetSlider(null);

            outputsConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.outputsCount = (int)value;
            });
            _data.outputsCount = outputsConfiguration.defaultValue;
        }
        
        private void SetNeuronsPerHiddenLayerCountSlider()
        {
            neuronsPerHiddenLayerConfiguration.SetSlider(null);

            neuronsPerHiddenLayerConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.neuronCountPerHiddenLayer = (int)value;
            });
            _data.neuronCountPerHiddenLayer = neuronsPerHiddenLayerConfiguration.defaultValue;
        }
        
        private void SetBiasValueSlider()
        {
            biasConfiguration.SetSlider(null);

            biasConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.bias = value;
            });
            _data.bias = biasConfiguration.defaultValue;
        }
        
        private void SetSigmoidSlopeSlider()
        {
            sigmoidConfiguration.SetSlider(null);

            sigmoidConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.sigmoid = value;
            });
            _data.sigmoid = sigmoidConfiguration.defaultValue;
        }
        
        private void SetMutationRateSlider()
        {
            mutationRateConfiguration.SetSlider(null);

            mutationRateConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.mutationRate = value;
            });
            _data.mutationRate = mutationRateConfiguration.defaultValue;
        }
        
        private void SetMutationChanceSlider()
        {
            mutationChanceConfiguration.SetSlider(null);

            mutationChanceConfiguration.slider.onValueChanged.AddListener(delegate(float value)
            {
                _data.mutationChance = value;
            });
            _data.mutationChance = mutationChanceConfiguration.defaultValue;
        }
        
        [Serializable]
        private enum GenomeTeam
        {
            Red,
            Green
        }

        [Serializable]
        private class IntValueConfigurationSlider
        {
            public Slider slider;
            public TextMeshProUGUI textComponent;
            public string textBeforeValue;
            public int defaultValue;
            public int minValue;
            public int maxValue;
            
            public void SetSlider(int? newDefaultValue = null, int? newMinValue = null, int? newMaxValue = null)
            {
                defaultValue = newDefaultValue ?? defaultValue;
                slider.minValue = newMinValue ?? (float)minValue;
                slider.maxValue = newMaxValue ?? (float)maxValue;

                textComponent.text = textBeforeValue + defaultValue;

                slider.SetValueWithoutNotify(defaultValue);
                
                slider.onValueChanged.AddListener(delegate(float value)
                {
                    textComponent.text = textBeforeValue + slider.value;
                });
            }
        }
        
        [Serializable]
        private class FloatValueConfigurationSlider
        {
            public Slider slider;
            public TextMeshProUGUI textComponent;
            public string textBeforeValue;
            public float defaultValue;
            public float minValue;
            public float maxValue;

            public void SetSlider(float? newDefaultValue = null, float? newMinValue = null, float? newMaxValue = null)
            {
                defaultValue = newDefaultValue ?? defaultValue;
                slider.minValue = newMinValue ?? minValue;
                slider.maxValue = newMaxValue ?? maxValue;
                
                textComponent.text = textBeforeValue + defaultValue;

                slider.SetValueWithoutNotify(defaultValue);

                slider.onValueChanged.AddListener(delegate(float value)
                {
                    textComponent.text = textBeforeValue + slider.value;
                });
            }
        }
        
    }
}
