using System.Collections.Generic;
using UnityEngine;
using GT.Websocket;
using GT.Backgammon.Logic;
using GT.Backgammon.Player;
using MiniJSON;

public class RequestInGameController : MonoBehaviour {

    private static RequestInGameController m_instance;
    public static RequestInGameController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<RequestInGameController>()); } private set { m_instance = value; } }

    private void OnDestroy()
    {
        Instance = null;
        StopAllCoroutines();
    }

    public void SendReady()
    {
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.ReadyToPlayTourneyMatch : RequestId.ReadyToPlay;
        Dictionary<string, object> dict = new Dictionary<string, object>() { { "TempMatchId", UserController.Instance.TempMatchId.ToString() } };
        WebSocketKit.Instance.SendRequest(request, dict);
    }

    public void SendRoll(Dictionary<string, string> rollingItems)
    {
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.TourneyRolled : RequestId.Rolled;
        WebSocketKit.Instance.SendRequest(request, new Dictionary<string, object>() { { "Data", Json.Serialize(rollingItems) } });
    }

    public void SendItemChat(string message)
    {
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.SendTourneyChat : RequestId.SendChat;
        WebSocketKit.Instance.SendRequest(request, new Dictionary<string, object>() { { "Message", message } });
    }

    public void SendItemChanged(Dictionary<string, object> storeList)
    {
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.ChangeTourneyRoomItem : RequestId.ChangeRoomItem;
        WebSocketKit.Instance.SendRequest(request, new Dictionary<string, object>() { { "Items", Json.Serialize(storeList) } });
    }

    public void SendRematch(bool isRematch)
    {
        WebSocketKit.Instance.SendRequest(RequestId.Rematch, new Dictionary<string, object>() { { "IsRematch", isRematch } });
    }

    public void SendMove(Move[] moves, int eaten, Board board, bool isBot = false)
    {
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.SendTourneyMove : RequestId.SendMove;
        Dictionary<string, object> moveData = new Dictionary<string, object>();
        moveData.Add("Data", Move.SerializeMoves(moves));
        moveData.Add("NewBoard", Board.Serialize(board));
        if(eaten > 0)
            moveData.Add("Eaten", eaten);
        if (isBot)
            moveData.Add("IsBot", isBot);

        Dictionary<string, object> dict = new Dictionary<string, object>() { { "MoveData", moveData } };
        WebSocketKit.Instance.SendRequest(request, dict);
    }

    public void SendDoubleRequest(string color, bool isBot = false)
    {
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.TourneyDoubleCube : RequestId.DoubleCube;
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("Color", color);
        if(isBot)
            dict.Add("IsBot", isBot);

        WebSocketKit.Instance.SendRequest(request, dict);
    }

    public void SendDoubleResponse(DoubleResponse answer, PlayerColor color, Board board, bool isBot = false)
    {
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.TourneyDoubleCubeAnswer : RequestId.DoubleCubeAnswer;
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("Answer", answer.ToString());
        dict.Add("OpponentColor", color.ToString());
        dict.Add("Board", Board.Serialize(board));
        if(isBot)
            dict.Add("IsBot", isBot);

        WebSocketKit.Instance.SendRequest(request, dict);
    }

    public void SendStopGame(string winnerId, string winnerColor, string board)
    {        
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.StopTourneyMatch : RequestId.StopGame;
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("Winner", winnerId);
        dict.Add("Board", board);
        dict.Add("Color", winnerColor);
        WebSocketKit.Instance.SendRequest(request, dict);
    }

    public void GetCurrentMoveState(int moveId, bool isDoubleCube)
    {
        RequestId request = TourneyController.Instance.IsInTourney() ? RequestId.GetTourneyCurrentMoveState : RequestId.GetCurrentMoveState;
        WebSocketKit.Instance.SendRequest(request, new Dictionary<string, object>() { { "MoveId", moveId }, { "isDoubleCube", isDoubleCube } });
    }

    public void RequestCurrentMatches(int maxCount)
    {
        //Dictionary<string, object> dict = new Dictionary<string, object>() { { "Count", maxCount } };
        //WebSocketKit.Instance.SendRequest(RequestId.GetCurrentMatches, dict);
    }

    public void ConnectToMatch(int matchId)
    {
        //Dictionary<string, object> dict = new Dictionary<string, object>() { { "MatchId", matchId } };
       // WebSocketKit.Instance.SendRequest(RequestId.ConnectToMatch, dict);
    }
}
