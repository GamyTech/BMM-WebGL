using UnityEngine;
using System.Collections;
using GT.Backgammon.Player;

namespace GT.Backgammon.View
{
    public interface IChatView
    {
        void InitView(bool isWhiteBottom);

        void ShowChat(PlayerColor color, string text);

    }
}
