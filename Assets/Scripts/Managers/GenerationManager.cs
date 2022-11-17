using System.Collections.Generic;
using IA.Gameplay;

namespace IA.Managers
{
    public class GenerationManager
    {
        
        private const int MaxGenerationsPerAgent = 3;
        private const int MinAgentsPerGroup = 2;

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

        public List<AgentGenerationData> GetNewGenerationData(GenomeData genomeData, List<Agent> agentsList)
        {
            List<AgentGenerationData> generationDataList = new List<AgentGenerationData>();

            if (agentsList.Count > MinAgentsPerGroup)
            {
                foreach (var agent in agentsList)
                {
                    if (agent.GenerationsLived < MaxGenerationsPerAgent && agent.FoodEaten > 0)
                    {
                        AgentGenerationData data = new AgentGenerationData();
                        data.brain = agent.AgentBrain;
                        data.genome = agent.AgentGenome;
                        data.generationsLived = agent.GenerationsLived;
                        generationDataList.Add(data);
                    }
                }
            }
            
            return generationDataList;
        }
        
        public class AgentGenerationData
        {
            public Genome genome;
            public NeuralNetwork brain;
            public int generationsLived;
        }
        
    }
}
