using UnityEngine;
using GT.Backgammon.Player;

namespace GT.Backgammon.View
{
    public abstract class BaseChatView : MonoBehaviour, IChatView
    {
        public PlayerChatView TopView;
        public PlayerChatView BottomView;

        protected bool isWhiteBottom;

        public virtual void InitView(bool isWhiteBottom)
        {
            this.isWhiteBottom = isWhiteBottom;
        }

        public virtual void ShowChat(PlayerColor color, string text)
        {

        }
    }
}
