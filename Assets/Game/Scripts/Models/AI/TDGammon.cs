using AForge.Neuro;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;


namespace GT.Backgammon.AI
{
    public static class TDGammon
    {
        public const int inputNeuronCount = 198;
        public const int outputNeuronCount = 2;


        public static ActivationNetwork InitNetworkStream(int strength = 0)
        {
            ActivationNetwork network = null;

            // binary files are not compatible with resources.load
            TextAsset asset = Resources.Load("TDGammonNet_" + strength) as TextAsset;

            if (asset == null)
                Debug.LogError("TDGammonNet_" + strength + " is not in Ressources");

            MemoryStream ms = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (ms = new MemoryStream(asset.bytes))
                {
                    object obj = bf.Deserialize(ms);
                    network = (ActivationNetwork)obj;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Writing to file failed. " + e.Message);
            }
            finally
            {
                if (ms != null)
                    ms.Close();
            }

            return network;
        }

        public static IPath FindBestTDGammonPath(AForge.Neuro.Network network, IBackgammonAI agent)
        {
            // get all paths/turns
            IPath[] allPaths = agent.GetAllPaths();
            // create best paths list
            List<IPath> bestPaths = new List<IPath>();

            double[] tempInput, tempOutput, bestOutput = new double[agent.OutputNeuronCount];

            // for each path in paths
            for (int i = 0; i < allPaths.Length; i++)
            {
                // get input of the state after simulation of path
                tempInput = agent.GetInputAfterSimulation(allPaths[i]);
                // compute output using input and save it in temp output
                tempOutput = network.Compute(tempInput);

                // if its the first iteration
                if (i == 0)
                {
                    // save as best output and add the current path to best paths
                    Array.Copy(tempOutput, bestOutput, tempOutput.Length);
                    bestPaths.Add(allPaths[i]);
                }
                // if not first iteration
                else
                {
                    // check if current output is better then best output
                    int compareValue = agent.CompareOutputs(tempOutput, bestOutput);
                    if (compareValue > 0)
                    {
                        // clear best paths and set it as best output and add current path to best paths
                        bestPaths.Clear();
                        Array.Copy(tempOutput, bestOutput, tempOutput.Length);
                        bestPaths.Add(allPaths[i]);
                    }
                    else if (compareValue == 0)
                    {
                        // if best output equals to current output just add the path to best paths
                        bestPaths.Add(allPaths[i]);
                    }
                }
            }

            // return a random path in best paths
            return bestPaths[UnityEngine.Random.Range(0, bestPaths.Count)];
        }
    }
}
