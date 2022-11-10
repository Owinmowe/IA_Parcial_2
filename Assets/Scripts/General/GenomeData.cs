[System.Serializable]
public class GenomeData
{
    public float mutationChance = 0.10f;
    public float mutationRate = 0.01f;
    
    public float[] genome;
    public int inputsCount;
    public int hiddenLayers;
    public int outputsCount;
    public int neuronCountPerHiddenLayer;
    public float bias;
    public float sigmoid;
}
