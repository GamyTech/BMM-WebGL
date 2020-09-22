public class TourneyRulesPopupWdget : PopupWidget
{
    public void Close()
    {
        if (string.IsNullOrEmpty(TourneyController.Instance.activeTourneyId))
        {
            HidePopup();
            return;
        }

        if (TourneyController.Instance.IsInStartedTourney())
            WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyInfoPopup);
        else
            WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyMenuPopup, null, true, false);
    }
}
