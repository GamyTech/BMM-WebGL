using UnityEngine;
using UnityEngine.Events;

namespace GT.Backgammon.View
{
    public class ImpSlotViewData : ISlotViewData
    {
        public UnityAction<int> ClickedAction;
        public UnityAction<int> CheckerSelectedAction;
        public UnityAction<int, int, Checker> CheckerDroppedAction;
        public int Index;
        public Texture2D[] Checkers;

        public ImpSlotViewData(UnityAction<int> clickedAction, UnityAction<int> checkerSelectedAction, UnityAction<int, int, Checker> checkerDroppedAction, int index, Texture2D[] checkers)
        {
            ClickedAction = clickedAction;
            CheckerSelectedAction = checkerSelectedAction;
            CheckerDroppedAction = checkerDroppedAction;
            Index = index;
            Checkers = checkers;
        }
    }
}
