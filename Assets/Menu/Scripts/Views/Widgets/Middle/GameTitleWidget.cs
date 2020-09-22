using UnityEngine.UI;

public class GameTitleWidget : Widget
{
    public Image TitleImage;

    public override void EnableWidget()
    {
        base.EnableWidget();

        TitleImage.sprite = AssetController.Instance.GetGameSpecific().logo;

    }
}
