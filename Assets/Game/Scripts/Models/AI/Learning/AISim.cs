//using System.Collections;
//using System;
//using System.Linq;
//using System.Collections.Generic;

//using AForge.Neuro;
//using AForge.Neuro.Learning;
//using System.IO;
//using System.Text;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEngine;

//public class AISim
//{


//    public static IEnumerator SimulateTurn(Match curMatch, AIPlayer player, Action rollCallback, Action<string> callback)
//    {
//        Match match = new Match(curMatch, true);

//        if (AIWaitTime != 0)
//        {
//            float rollDelay = UnityEngine.Random.Range(AIMinRollDiceTime, AIRollDiceTime);
//            yield return new UnityEngine.WaitForSeconds(rollDelay);
//            if (rollCallback != null)
//                rollCallback();
//            float playDelay = UnityEngine.Random.Range(AIMinWaitTime, AIWaitTime);
//            yield return new UnityEngine.WaitForSeconds(playDelay);
//        }
//        if (callback != null)
//            callback(player.FindBestTurn(match));
//    }

//    #region TD Gammon AI
//    static string PATH = UnityEngine.Application.streamingAssetsPath + "/Networks/";
//    static string TDGAMMON_FILE_NAME = PATH + "TDGammonNet_";
//    static string TDGAMMON_ANDROID_FILE_NAME = UnityEngine.Application.persistentDataPath + "/TDGammonNet_";

//    const int inputNeuronCount = 198;
//    const int outputNeuronCount = 2;
//    const int hiddenLayerNeuronCount = 60;
//    const double learningRate = 0.2;
//    const double momentum = 0;
//    const double sigmoidAlphaValue = 0.0714;

//    static ActivationNetwork InitNewNetwork()
//    {
//        return new ActivationNetwork(new SigmoidFunction(sigmoidAlphaValue), inputNeuronCount, hiddenLayerNeuronCount, outputNeuronCount);
//    }

//    public static ActivationNetwork InitNetworkStream(int strength = 0)
//    {
//        if (strength == 0)
//            return InitNewNetwork();

//        ActivationNetwork network;
//        if (TDGAMMON_FILE_NAME.Contains("://"))
//        {
//            string realPath = TDGAMMON_ANDROID_FILE_NAME + strength + ".bin";
//            if (File.Exists(realPath) && new FileInfo(realPath).Length == 0)
//                File.Delete(realPath);
//            if (File.Exists(realPath) == false)
//            {
//                UnityEngine.WWW www = new UnityEngine.WWW(TDGAMMON_FILE_NAME + strength + ".bin");
//                while (!www.isDone) { }
//                try
//                {
//                    File.WriteAllBytes(realPath, www.bytes);
//                }
//                catch (Exception e)
//                {
//                    Debug.LogError("Writing to file failed. " + e.Message);
//                }
//            }
//            network = (ActivationNetwork)SC_DataManager.LoadFromFile(realPath);
//        }
//        else
//        {
//            network = (ActivationNetwork)SC_DataManager.LoadFromFile(TDGAMMON_FILE_NAME + strength + ".bin");
//        }
//        return network;
//    }

//    public static ActivationNetwork InitNetwork(int strength = 0)
//    {
//        return strength == 0 ? InitNewNetwork() : LoadNetworkFromFile(TDGAMMON_FILE_NAME + strength + ".bin");
//    }

//    static BackPropagationLearning InitTeacher(ActivationNetwork network)
//    {
//        BackPropagationLearning teacher = new BackPropagationLearning(network);
//        teacher.LearningRate = learningRate;
//        teacher.Momentum = momentum;
//        return teacher;
//    }

//    static void SaveNetworkToFile(int strength, Network network)
//    {
//        GTDataManagementKit.SaveSerializableClassToFile(TDGAMMON_FILE_NAME + strength + ".bin", network);
//    }

//    static ActivationNetwork LoadNetworkFromFile(string fileName)
//    {
//        return (ActivationNetwork)GTDataManagementKit.LoadSerializableClassFromFile(fileName);
//    }
//    #endregion TD Gammon AI

//    #region TD Gammon AI Learning
//    public static IEnumerator LearnPlayGames(int gamesAmount, int saveInterval, int continueFrom = 0)
//    {
//        DateTime startTime = DateTime.Now;
//        ActivationNetwork network = InitNetwork(continueFrom);
//        int gamesCount = continueFrom;
//        int toGamesCount = continueFrom + gamesAmount;
//        int whiteWonCount = 0, blackWonCount = 0;
//        BackPropagationLearning teacher = InitTeacher(network);
//        while (gamesCount++ < toGamesCount)
//        {
//            yield return null;
//            Match match = StartGame();
//            Enums.MatchResult result = RunOneGame(match, network, teacher);
//            if (result == Enums.MatchResult.White)
//                whiteWonCount++;
//            else if (result == Enums.MatchResult.Black)
//                blackWonCount++;

//            if (saveInterval != 0 && gamesCount % saveInterval == 0)
//                SaveNetworkToFile(gamesCount, network);

//            Debug.Log("[" + gamesCount + "] {" + (DateTime.Now - startTime) + "} - Scores: (" + whiteWonCount + "," + blackWonCount +
//                ") Dice: (" + match.WhiteDice.GetDiceData() + "," + match.BlackDice.GetDiceData() + ")");
//            //Debug.Log("Game Errors: " + gameErrors.Display());
//        }
//    }

//    public static IEnumerator PlayGames(AIPlayer p1, AIPlayer p2, int gamesAmount)
//    {
//        DateTime startTime = DateTime.Now;
//        int gamesCount = 0;
//        int whiteWonCount = 0, blackWonCount = 0;
//        while (gamesCount++ < gamesAmount)
//        {
//            yield return null;
//            Match match = StartGame();
//            Enums.MatchResult result = RunOneGame(match, p1, p2);
//            if (result == Enums.MatchResult.White)
//                whiteWonCount++;
//            else if (result == Enums.MatchResult.Black)
//                blackWonCount++;
//            Debug.Log("[" + gamesCount + "] {" + (DateTime.Now - startTime) + "} - Scores: (" + whiteWonCount + "," + blackWonCount +
//                ") Dice: (" + match.WhiteDice.GetDiceData() + "," + match.BlackDice.GetDiceData() + ")");
//            Debug.Log("Game Errors: " + gameErrors.Display());
//        }
//    }

//    public static IEnumerator PlayGames(int AI1Str, int AI2Str, int gamesAmount)
//    {
//        DateTime startTime = DateTime.Now;
//        int gamesCount = 0;
//        ActivationNetwork AI1Network = InitNetwork(AI1Str);
//        BackPropagationLearning AI1Teacher = InitTeacher(AI1Network);
//        Enums.PlayerColor AI1Color = Enums.PlayerColor.White;
//        ActivationNetwork AI2Network = InitNetwork(AI2Str);
//        BackPropagationLearning AI2Teacher = InitTeacher(AI2Network);
//        Enums.PlayerColor AI2Color = Enums.PlayerColor.Black;
//        int whiteWonCount = 0, blackWonCount = 0;
//        while (gamesCount++ < gamesAmount)
//        {
//            yield return null;
//            Match match = StartGame();
//            Enums.MatchResult result = RunOneGame(match, AI1Color, AI1Network, AI1Teacher, AI2Color, AI2Network, AI2Teacher);
//            if (result == Enums.MatchResult.White)
//                whiteWonCount++;
//            else if (result == Enums.MatchResult.Black)
//                blackWonCount++;
//            Debug.Log("[" + gamesCount + "] {" + (DateTime.Now - startTime) + "} - Scores: (AI" + AI1Str + ":" + whiteWonCount + "," + "AI" + AI2Str + ":" + blackWonCount +
//                ") Dice: (" + match.WhiteDice.GetDiceData() + "," + match.BlackDice.GetDiceData() + ")");
//            Debug.Log("Game Errors: " + gameErrors.Display());
//        }
//    }

//    static Match StartGame()
//    {
//        Enums.PlayerColor startingColor = UnityEngine.Random.Range(0, 2) == 0 ? Enums.PlayerColor.White : Enums.PlayerColor.Black;
//        Match currentMatch = new Match(startingColor, 100, 100, true);
//        currentMatch.SetStartDice(startingColor);
//        currentMatch.StartMatch();
//        return currentMatch;
//    }

//    static Enums.MatchResult RunOneGame(Match match, AIPlayer p1, AIPlayer p2)
//    {
//        gameErrors = new List<double>();
//        while (match.IsMatchOver == false)
//        {
//            AIPlayer currentPlayer = match.CurrentTurn == p1.PColor ? p1 : p2;
//            List<CheckerMove> chosenTurn = CheckerMove.DeserializeList(currentPlayer.FindBestTurn(match));
//            if (chosenTurn.Count > 0)
//                match.ApplyTurn(chosenTurn);
//            match.EndTurn();
//            match.SetDice(match.CurrentTurn);
//        }
//        return match.Result;
//    }

//    static Enums.MatchResult RunOneGame(Match match, ActivationNetwork network, BackPropagationLearning teacher)
//    {
//        gameErrors = new List<double>();
//        while (match.IsMatchOver == false)
//        {
//            MakeTurn(match, network, teacher);
//        }
//        return match.Result;
//    }

//    static Enums.MatchResult RunOneGame(Match match, Enums.PlayerColor AI1Color, ActivationNetwork AI1, BackPropagationLearning AI1Teacher,
//        Enums.PlayerColor AI2Color, ActivationNetwork AI2, BackPropagationLearning AI2Teacher)
//    {
//        gameErrors = new List<double>();
//        while (match.IsMatchOver == false)
//        {
//            if (match.CurrentTurn == AI1Color)
//            {
//                MakeTurn(match, AI1, AI1Teacher);
//            }
//            else
//            {
//                MakeTurn(match, AI2, AI2Teacher);
//            }
//        }
//        return match.Result;
//    }

//    static List<double> gameErrors;

//    // Given vector of weights D eligibility trace vector e(s).
//    // 1.) Evaluate board positions using the neural network. Choose the move with the highest (lowest as black) evaluation. Move.
//    // 2.) If this is the end of the game:
//    //      Backprop, with reward of 1 or 0 depending on whether white won or lost.
//    // 3.) Else if this was not the first move, then:
//    //      a) Evaluate board.
//    //      b) Calculate error between current evaluation and previous evaluation.
//    //      c) Backprop, using the current evaluation as desired output and the board position previous to the current move as the input.
//    // End.
//    static string MakeTurn(Match match, Network network, BackPropagationLearning teacher)
//    {
//        string turnDone = "";
//        Enums.PlayerColor currentTurn = match.CurrentTurn;
//        List<List<CheckerMove>> turns = match.GetAllPossibleTurns(currentTurn);
//        if (turns.Count > 0)
//        {
//            double[] previousBoardInput = GetTDGammonInput(match);
//            List<CheckerMove> chosenTurn = FindBestTDGammonTurn(match, network, currentTurn, turns);

//            match.ApplyTurn(chosenTurn);
//            turnDone = CheckerMove.SerializeList(chosenTurn);
//            match.EndTurn();
//            match.SetDice(match.CurrentTurn);
//            if (match.IsMatchOver)
//            {
//                double whiteWonOutput = match.Result == Enums.MatchResult.White ? 1 : 0;
//                double blackWonOutput = match.Result == Enums.MatchResult.Black ? 1 : 0;
//                double[] output = new double[outputNeuronCount] { whiteWonOutput, blackWonOutput };
//                double error = teacher.Run(previousBoardInput, output);
//                gameErrors.Add(error);
//            }
//            else if (match.TurnCount > 1)
//            {
//                double[] output = new double[outputNeuronCount];
//                Array.Copy(network.Compute(GetTDGammonInput(match)), output, output.Length);
//                double error = teacher.Run(previousBoardInput, output);
//                gameErrors.Add(error);
//            }
//        }
//        else
//        {
//            // No moves possible
//            match.EndTurn();
//            match.SetDice(match.CurrentTurn);
//        }
//        return turnDone;
//    }

//    public static List<CheckerMove> FindBestTDGammonTurn(Match match, Network network, Enums.PlayerColor currentTurn, List<List<CheckerMove>> turns)
//    {
//        List<List<CheckerMove>> bestTurns = new List<List<CheckerMove>>();
//        double[] bestOutput = new double[outputNeuronCount];
//        for (int i = 0; i < turns.Count; i++)
//        {
//            List<CheckerMove> tempTurn = turns[i];
//            Match simulatedMatch = new Match(match, true);
//            simulatedMatch.ApplyTurn(tempTurn);
//            simulatedMatch.EndTurn();
//            double[] input = GetTDGammonInput(simulatedMatch);
//            double[] tempOutput = network.Compute(input);
//            if (i == 0)
//            {
//                Array.Copy(tempOutput, bestOutput, tempOutput.Length);
//                bestTurns.Add(tempTurn);
//            }
//            else if (currentTurn == Enums.PlayerColor.White)
//            {
//                if (bestOutput[0] - bestOutput[1] < tempOutput[0] - tempOutput[1])
//                {
//                    Array.Copy(tempOutput, bestOutput, tempOutput.Length);
//                    bestTurns.Clear();
//                    bestTurns.Add(tempTurn);
//                }
//                else if (bestOutput[0] - bestOutput[1] == tempOutput[0] - tempOutput[1])
//                {
//                    bestTurns.Add(tempTurn);
//                }
//            }
//            else if (currentTurn == Enums.PlayerColor.Black)
//            {
//                if (bestOutput[1] - bestOutput[0] < tempOutput[1] - tempOutput[0])
//                {
//                    Array.Copy(tempOutput, bestOutput, tempOutput.Length);
//                    bestTurns.Clear();
//                    bestTurns.Add(tempTurn);
//                }
//                else if (bestOutput[1] - bestOutput[0] == tempOutput[1] - tempOutput[0])
//                {
//                    bestTurns.Add(tempTurn);
//                }
//            }
//        }

//        return bestTurns[UnityEngine.Random.Range(0, bestTurns.Count)];
//    }

//    public static double[] GetTDGammonInput(Match match)
//    {
//        double[] input = new double[inputNeuronCount];
//        for (int i = 0; i < GameBoard.BOARD_SIZE; i++)
//        {
//            Enums.SlotColor slot = match.board.GetSlot(i).SlotColor;
//            int count = match.board.GetSlot(i).Quantity;

//            if (slot == Enums.SlotColor.White)
//            {
//                input[i * 8 + 0] = count >= 1 ? 1 : 0;
//                input[i * 8 + 1] = count >= 2 ? 1 : 0;
//                input[i * 8 + 2] = count >= 3 ? 1 : 0;
//                input[i * 8 + 3] = count > 3 ? (double)(count - 3) / 2 : 0;
//            }
//            else if (slot == Enums.SlotColor.Black)
//            {
//                input[i * 8 + 4] = count >= 1 ? 1 : 0;
//                input[i * 8 + 5] = count >= 2 ? 1 : 0;
//                input[i * 8 + 6] = count >= 3 ? 1 : 0;
//                input[i * 8 + 7] = count > 3 ? (double)(count - 3) / 2 : 0;
//            }
//        }

//        // Current moving player
//        Enums.PlayerColor currentTurn = match.CurrentTurn;
//        input[192] = currentTurn == Enums.PlayerColor.White ? 1 : 0;
//        input[193] = currentTurn == Enums.PlayerColor.Black ? 1 : 0;

//        // Chackers on the bar (eaten)
//        input[194] = (double)match.board.GetEatenSlot(Enums.PlayerColor.White).Quantity / 2;
//        input[195] = (double)match.board.GetEatenSlot(Enums.PlayerColor.Black).Quantity / 2;

//        // Checkers borne off
//        input[196] = (double)match.board.GetBearoffSlot(Enums.PlayerColor.White).Quantity / 15;
//        input[197] = (double)match.board.GetBearoffSlot(Enums.PlayerColor.Black).Quantity / 15;

//        return input;
//    }
//    #endregion TD Gammon AI Learning
//}
//
