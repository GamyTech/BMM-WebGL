
namespace GT.Backgammon.AI
{
    public interface IBackgammonAI 
    {
        int InputNeuronCount { get; }
        int OutputNeuronCount { get; }

        IPath[] GetAllPaths();

        double[] GetInputAfterSimulation(IPath path);

        int CompareOutputs(double[] currentOutput, double[] bestOutput);
    }
}
