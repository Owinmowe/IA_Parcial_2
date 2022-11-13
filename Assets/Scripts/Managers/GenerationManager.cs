using System.Collections.Generic;
using IA.Gameplay;

namespace IA.Managers
{
    public class GenerationManager
    {
        public void CreateAgentBrains(Agent agent, GenomeData data)
        {
            NeuralNetwork brain = new NeuralNetwork();
            brain.AddFirstNeuronLayer(data.inputsCount, data.bias, data.sigmoid);
            for (int i = 0; i < data.hiddenLayers; i++)
            {
                brain.AddNeuronLayer(data.neuronCountPerHiddenLayer, data.bias, data.sigmoid);
            }
            brain.AddNeuronLayer(data.outputsCount, data.bias, data.sigmoid);

            Genome genome = new Genome(brain.GetTotalWeightsCount());
            if (data.genome is { Length: > 0 })
            {
                genome.genome = data.genome;
            }
            
            agent.SetIntelligence(genome, brain);
        }
    }
}
