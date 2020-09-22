using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GT.Websocket;

public class Rank {

    public delegate void Changed<T>(T newValue);

    public event Changed<int> OnXPChanged;
    public event Changed<int> OnLevelChanged;
    public event Changed<int> OnStarsChanged;
    public event Changed<MedalData> OnMedalChanged;

    private static List<MedalData> MedalList;

    private int m_XP;
    private int m_level;
    private int m_stars;
    public float LevelProgress { get; private set; }
    public int PointsForNextLevel { get; private set; }
    private MedalData m_Medal;


    public int XP
    {
        get { return m_XP; }
        protected set
        {
            if (Utils.SetProperty(ref m_XP, value))
            {
                if(Medal == null || m_XP >= PointsForNextLevel)
                {
                    int newLevel;
                    int newStars;
                    MedalData newMedal;
                    int pointsForNextLevel;
                    GetRank(m_XP, out newLevel, out newStars, out newMedal, out pointsForNextLevel);
                    Level = newLevel;
                    Stars = newStars;
                    Medal = newMedal;
                    PointsForNextLevel = pointsForNextLevel;
                    LevelProgress = (float)m_XP / (float)PointsForNextLevel;
                }

                if (OnXPChanged != null)
                    OnXPChanged(m_XP);
            }
        }
    }

    public int Level
    {
        get { return m_level; }
        protected set
        {
            if (Utils.SetProperty(ref m_level, value) && OnLevelChanged != null)
                OnLevelChanged(m_level);
        }
    }

    public int Stars
    {
        get { return m_stars; }
        protected set
        { 
            if (Utils.SetProperty(ref m_stars, value) && OnStarsChanged != null)
                OnStarsChanged(m_stars);
        }
    }

    public MedalData Medal
    {
        get { return m_Medal; }
        protected set
        {
            if (Utils.SetProperty(ref m_Medal, value) && OnMedalChanged != null)
                OnMedalChanged(m_Medal);
        }
    }

    public string MedalName
    {
        get { return Medal.MedalName; }
    }

    public Texture2D MedalTexture
    {
        get { return Medal.MedalTexture; }
    }

    public Rank(int xp)
    {
        MedalList = AssetController.Instance.MedalList;
        SetXP(xp);
    }

    public void SetXP(object o)
    {
        XP = o.ParseInt();
    }

    public static void GetRank(int xp, out int level, out int stars, out MedalData medal, out int pointsForNextLevel)
    {
        level = 0;
        stars = 0;
        int medalIndex = 0;
        pointsForNextLevel = 0;
        do
        {
            pointsForNextLevel += MedalList[medalIndex].PointsPerLevel;
            ++stars;
            ++level;
            if (stars == 3)
            {
                ++medalIndex;
                if(medalIndex < MedalList.Count)
                    stars = 0;
            }

        } while (xp > pointsForNextLevel && medalIndex < MedalList.Count);

        medal = MedalList[medalIndex];
    }

}
