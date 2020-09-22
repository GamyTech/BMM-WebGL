using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankWidget : Widget
{

    public Text Level;
    public Text points;
    public Text PointsForNext;
    public Slider ProgressSlider;
    public GameObject RowPanel;
    public HorizontalLayoutGroup RowPrefab;
    public MedalView RankViewPrefab;

    public override void EnableWidget()
    {
        Level.text = UserController.Instance.gtUser.rank.Level.ToString();
        points.text = UserController.Instance.gtUser.rank.XP.ToString();
        PointsForNext.text = UserController.Instance.gtUser.rank.PointsForNextLevel.ToString();
        ProgressSlider.value = UserController.Instance.gtUser.rank.LevelProgress;

        int Slots = 0;
        bool beforeCurrentRank = true;
        HorizontalLayoutGroup lastRow = null;
        for (int x = 0; x < AssetController.Instance.MedalList.Count; ++x)
        {
            int MaxRankPerRow = AssetController.Instance.MedalList.Count - x < 4 ? 2 : 3;
            if (Slots <= x)
            {
                lastRow = Instantiate(RowPrefab, RowPanel.transform, false);
                Slots += MaxRankPerRow;
            }

            MedalView rv = Instantiate(RankViewPrefab, lastRow.transform, false);

            int stars = beforeCurrentRank ? 3 : 0;
            string medalName = null;
            if (AssetController.Instance.MedalList[x].MedalName == UserController.Instance.gtUser.rank.MedalName)
            {
                beforeCurrentRank = false;
                stars = UserController.Instance.gtUser.rank.Stars;
                medalName = AssetController.Instance.MedalList[x].MedalName;
            }
            rv.Init(AssetController.Instance.MedalList[x].MedalTexture, stars, medalName);
        }

        base.EnableWidget();
    }

    public override void DisableWidget()
    {
        base.DisableWidget();
    }
}
