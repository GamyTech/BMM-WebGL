using GT.Backgammon.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GT.Backgammon.View
{
    public abstract class BasePlayerDiceView : MonoBehaviour
    {
        public abstract IEnumerator ShowDice(int[] dice, IPlayer player);

        public abstract void HideDice();
    }
}
