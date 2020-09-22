using UnityEngine;
using GT.Backgammon.Logic;
using GT.Backgammon.AI;
using AForge.Neuro;
using System.Collections;

namespace GT.Backgammon.Player
{
    public class TDGammonPlayer : BaseAIPlayer
    {
        ActivationNetwork network;

        private bool tutorialMode;
        private string m_description;
        public override string AIDescription
        {
            get
            {
                return m_description;
            }
        }

        public TDGammonPlayer(string id, PlayerColor color, int strength) : base(id, color)
        {
            InitAI(strength);
        }

        public TDGammonPlayer(string id, PlayerColor color, PlayerData data, float minWaitTime, float maxWaitTime, int strength, bool isTutorialMode = false) 
            : base(id, color, data, minWaitTime, maxWaitTime)
        {
            InitAI(strength);
            tutorialMode = isTutorialMode;
        }

        private void InitAI(int strength)
        {
            m_description = "TDGammon" + strength;
            network = TDGammon.InitNetworkStream(strength);
        }

        protected override Move[] GetBestTurn(Board board, out int eaten)
        {
            IBackgammonAI adapter = new TreeAIAdapter(TDGammon.inputNeuronCount, TDGammon.outputNeuronCount, possibleMoves, board, playerColor);

            TreePath<Move> path = TDGammon.FindBestTDGammonPath(network, adapter) as TreePath<Move>;
            eaten = GetEatenOnPath(path);
            return path.GetItemsFromPath();
        }


        protected override bool ShouldDouble(Board board)
        {
            return CalcShouldDouble(board);
        }

        public override void RequestedDouble(Board board)
        {
            base.RequestedDouble(board);
            if (ContentController.Instance != null)
                ContentController.Instance.StartCoroutine(RequestedDoubleRoutine(board));
            else
                RequestedDoubleInstant(board);
        }

        protected void RequestedDoubleInstant(Board board)
        {
            DoubleResponse(CalcResponse(board));
        }

        protected IEnumerator RequestedDoubleRoutine(Board board)
        {
            yield return WaitRandomTime();
            DoubleResponse(CalcResponse(board));
        }

        private bool CalcShouldDouble(Board board)
        {
            int whitePip, blackPip;
            board.GetPip(out whitePip, out blackPip);

            int score = whitePip - blackPip;
            if (playerColor == PlayerColor.White)
                score = -score;
            if (score > 20)
                return true;
            else
                return false;
        }

        private DoubleResponse CalcResponse(Board board)
        {
            if(tutorialMode) return Logic.DoubleResponse.Yes;

            int whitePip, blackPip;
            board.GetPip(out whitePip, out blackPip);

            int score = whitePip - blackPip;
            if (playerColor == PlayerColor.White)
                score = -score;
            //if (score > 20)
            //    return Logic.DoubleResponse.DoubleAgain;
            if (score < -10)
                return Logic.DoubleResponse.GiveUp;
            else
                return Logic.DoubleResponse.Yes;
        }
    }
}
