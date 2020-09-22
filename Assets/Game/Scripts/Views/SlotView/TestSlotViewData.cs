using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GT.Backgammon.View
{
    public class TestSlotViewData : ISlotViewData
    {
        public UnityAction<BaseEventData> clickedAction;
        public int index;
        public bool top;

        public TestSlotViewData(UnityAction<BaseEventData> clickedAction, int index, bool top)
        {
            this.clickedAction = clickedAction;
            this.index = index;
            this.top = top;
        }
    }
}
