using GT.User;
using UnityEngine.UI;

public class FullMatchHistoryWidget : ListContainerWidget<HistoryMatches>
{
    public ScrollRect scrollRect;

    public override void EnableWidget()
    {
        base.EnableWidget();
#if UNITY_STANDALONE || UNITY_WEBGL
        MiddleWidgetScrollBar.Instance.SetNewScrollRect(scrollRect);
#endif

        UserController.Instance.gtUser.OnMatchHistoryChanged += GtUser_OnMatchHistoryChanged;
        InitList(UserController.Instance.gtUser.MatchHistoryData);
    }

    public override void DisableWidget()
    {
        UserController.Instance.gtUser.OnMatchHistoryChanged -= GtUser_OnMatchHistoryChanged;
        base.DisableWidget();
    }

    protected override void UpdateNextNeededElement()
    {
        UserController.Instance.GetMissingMatchHistory();
    }

    #region Events
    private void GtUser_OnMatchHistoryChanged(HistoryMatches newValue)
    {
        UpdateList(newValue);
    }
    #endregion Events
}
