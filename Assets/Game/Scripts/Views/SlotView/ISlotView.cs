using UnityEngine;
using GT.Backgammon.Logic;
using UnityEngine.Events;

namespace GT.Backgammon.View
{
    public interface ISlotView 
    {
        int LogicIndex { get; }
        SlotColor SlotColor { get; set; }
        int Quantity { get; set; }

        void InitSlot(ISlotViewData slotViewData);

        void SetSlotView(int logicIndex, SlotColor color, int quantity);

        void AddChecker(SlotColor color);

        void RemoveChecker();

        void ResetSlotView();

        void SetIndicator(Color color);

        void ResetIndicator();
    }
}
