using UnityEngine;
using System.Collections.Generic;
using GT.Websocket;

public class Wallet
{
    public delegate void floatAmountChanged(float newMoney);
    public delegate void intAmountChanged(int newPoints);

    public event floatAmountChanged OnCashChanged;
    public event floatAmountChanged OnBonusCashChanged;
    public event floatAmountChanged OnTotalCashChanged;

    public event intAmountChanged OnVirtualChanged;

    public event intAmountChanged OnLoyaltyChanged;
    public event intAmountChanged OnTotalLoyaltyChanged;

    public static float[] MULTIPLIER_LEVELS = new float[] { 0, 75000f, 200000f }; //75000f, 200000f

    public const string CashPostfix = "$"; // $,€
    public const string SpecialCashPostfix = "Ś"; // Ś,€
    public const string VirtualPostfix = "Ž ";
    public const string GPTPostfix = "‡ ";
    public const string LoyaltyPointsPostfix = "Ť";
    public const string PercentPostfix = "%";

    public const float DeductionPartCash = .05f;
    public const float DeductionPartVirtual = .1f;

    private float m_cash;
    private float m_bonusCash;
    private float m_totalCash;

    private int m_virtualCoins;

    private int m_loyaltyPoints;
    private int m_totalLoyaltyPoints;
    private int m_loyaltyMultiplier;


    public float Cash
    {
        get { return m_cash; }
        protected set
        {
            if (Utils.SetProperty(ref m_cash, value))
            {
                if (OnCashChanged != null)
                    OnCashChanged(m_cash);
                TotalCash = m_cash + m_bonusCash;
            }
        }
    }
    public float BonusCash
    {
        get { return m_bonusCash; }
        protected set
        {
            if (Utils.SetProperty(ref m_bonusCash, value))
            {
                if (OnBonusCashChanged != null)
                    OnBonusCashChanged(m_cash);
                TotalCash = m_cash + m_bonusCash;
            }
        }
    }

    public float TotalCash
    {
        get { return m_totalCash; }
        protected set
        {
            if (Utils.SetProperty(ref m_totalCash, value))
            {
                TrackingKit.SetCashAmount((int)m_totalCash);
                if (OnTotalCashChanged != null)
                    OnTotalCashChanged(m_cash + m_bonusCash);
            }
        }
    }

    public int VirtualCoins
    {
        get { return m_virtualCoins; }
        protected set
        {
            if (Utils.SetProperty(ref m_virtualCoins, value))
            {
                if (OnVirtualChanged != null)
                    OnVirtualChanged(m_virtualCoins);
            }
        }
    }

    public int LoyaltyPoints
    {
        get { return m_loyaltyPoints; }
        protected set
        {
            if (Utils.SetProperty(ref m_loyaltyPoints, value))
            {
                TrackingKit.SetLoyaltiesAmount(m_loyaltyPoints);
                if (OnLoyaltyChanged != null)
                    OnLoyaltyChanged(m_loyaltyPoints);
            }
        }
    }

    public int TotalLoyaltyPoints
    {
        get { return m_totalLoyaltyPoints; }
        protected set
        {
            if (Utils.SetProperty(ref m_totalLoyaltyPoints, value) && OnTotalLoyaltyChanged != null)
                OnTotalLoyaltyChanged(m_totalLoyaltyPoints);
        }
    }

    public int LoyaltyMultiplier
    {
        get { return m_loyaltyMultiplier; }
        protected set { m_loyaltyMultiplier = value; }
    }


    #region Constructors
    public Wallet(Dictionary<string, object> walletDict)
    {
        WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.Wallet, SetWallet);
        SetWallet(walletDict);
    }
    #endregion Constructors

    #region Destructor
    ~Wallet()
    {
        WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.Wallet, SetWallet);
    }
    #endregion Destructor

    #region Set Events

    private void SetWallet(object o)
    {
        Dictionary<string, object> walletDict = (Dictionary<string, object>)o;
        if (walletDict == null) return;

        object walletObject;
        if (walletDict.TryGetValue("Cash", out walletObject))
            Cash = walletObject.ParseFloat();

        if (walletDict.TryGetValue("BonusCash", out walletObject))
            BonusCash = walletObject.ParseFloat();

        if (walletDict.TryGetValue("Virtual", out walletObject))
            VirtualCoins = walletObject.ParseInt();

        if (walletDict.TryGetValue("LoyaltyPoints", out walletObject))
            LoyaltyPoints = walletObject.ParseInt();

        if (walletDict.TryGetValue("TotalLoyaltyPoints", out walletObject))
            TotalLoyaltyPoints = walletObject.ParseInt();

        if (walletDict.TryGetValue("Multiplier", out walletObject))
            LoyaltyMultiplier = walletObject.ParseInt();
    }
    #endregion Set Events

    #region Static Functions
    public static string MatchKindToPrefix(Enums.MatchKind kind)
    {
        switch (kind)
        {
            case Enums.MatchKind.Cash:
                return CashPostfix;
            case Enums.MatchKind.Virtual:
                return VirtualPostfix;
            default:
                return string.Empty;
        }
    }

    public static void SetGradientForMatchKind(Enums.MatchKind kind, UnityEngine.UI.Gradient gradient)
    {
        if (gradient == null)
            return;

        Color startColor;
        Color endColor;
        switch (kind)
        {
            case Enums.MatchKind.Cash:
                startColor = new Color(0.0f, 1.0f, 0.09f);
                endColor = new Color(0.0f, 0.39f, 0.02f);
                break;
            case Enums.MatchKind.Virtual:
                if (AppInformation.GAME_ID == Enums.GameID.BackgammonBlockChain)
                {
                    startColor = new Color(0.0f, 1.0f, 0.09f);
                    endColor = new Color(0.0f, 0.39f, 0.02f);
                }
                else
                {
                    startColor = new Color(0.0f, 0.54f, 1.0f);
                    endColor = new Color(0.0f, 0.27f, 0.35f);
                }
                break;
            default:
                startColor = new Color(0.0f, 1.0f, 0.09f);
                endColor = new Color(0.0f, 0.39f, 0.02f);
                break;
        }

        gradient.startColor = startColor;
        gradient.endColor = endColor;
    }

    public static string AmountToStringStartingFromExponent(int maxExponent, float amount, int maxPrecision = 1)
    {
        int exp = (int)Mathf.Log(amount, 10);
        if (exp < maxExponent)
        {
            return amount.ToString(new System.Text.StringBuilder("#,##0.").Append('#', maxPrecision).ToString());
        }
        else return AmountToString(amount, maxPrecision);
    }

    public static string AmountToString(float amount, Enums.MatchKind kind)
    {
        int precision = kind == Enums.MatchKind.Cash ? 2 : 1;
        return MatchKindToPrefix(kind) + AmountToString(amount, precision);
    }

    public static string AmountToString(float amount, int maxPrecision = 1)
    {
        string postfix = "";
        int rank = (int)Mathf.Log(amount, 1000);
        switch (rank)
        {
            case 1:
                postfix = "K";
                break;
            case 2:
                postfix = "M";
                break;
            case 3:
                postfix = "B";
                break;
        }

        if (!string.IsNullOrEmpty(postfix) && maxPrecision > 1)
            maxPrecision -= 1;

        float div = rank <= 0 ? 1 : Mathf.Pow(1000, rank);
        float m = amount / div;
        if (Mathf.Round(m) == m)
            maxPrecision = 0;
        string str = m.ToString();
        if (maxPrecision == 0)
        {
            str = m.ToString("#0.");
        }
        else
        {
            try
            {
                if (str.IndexOf('.') >= 0)
                    str = str.Substring(0, str.IndexOf('.') + maxPrecision + 1);
                else
                    str = m.ToString(new System.Text.StringBuilder("#,##0.").Append('#', maxPrecision).ToString());
            }
            catch
            {
                str = m.ToString(new System.Text.StringBuilder("#,##0.").Append('#', maxPrecision).ToString());
            }
        }
        return str + postfix;
    }
    #endregion Static Functions

    #region Overrides
    public override string ToString()
    {
        return "Cash:" + Cash + " , BonusCash:" + BonusCash + " , LoyaltyPoints:" + LoyaltyPoints + " , TotalLoyaltyPoints:" + TotalLoyaltyPoints + " , Virtuals:" + VirtualCoins;
    }
    #endregion Overrides

    #region Public Methods
    public bool HaveEnoughMoney(List<BetRoom> rooms)
    {
        for (int i = 0; i < rooms.Count; i++)
            if (!HaveEnoughMoney(rooms[i]))
                return false;
        return true;
    }

    public bool HaveEnoughMoney(BetRoom room)
    {
        switch (room.Kind)
        {
            case Enums.MatchKind.Cash:
                return room.TotalAmount < TotalCash || Mathf.Approximately(room.TotalAmount, TotalCash);
            case Enums.MatchKind.Virtual:
                return room.TotalAmount < VirtualCoins || Mathf.Approximately(room.TotalAmount, VirtualCoins);
            default:
                Debug.LogError("HaveEnoughMoney Unrecognized MatchKind " + room.Kind);
                break;
        }
        return false;
    }

    public bool HaveEnoughMoney(Tourney tourney)
    {
        if (tourney == null) return false;
        return tourney.BuyIn < TotalCash || Mathf.Approximately(tourney.BuyIn, TotalCash);
    }

    public float AvailableCurrency(Enums.MatchKind kind)
    {
        float available = 0.0f;
        switch (kind)
        {
            case Enums.MatchKind.Cash:
                available = TotalCash;
                break;
            case Enums.MatchKind.Virtual:
                available = (float)VirtualCoins;
                break;
        }

        return available;
    }

    public void Reset(Dictionary<string, object> wallet)
    {
        SetWallet(wallet);
    }
    #endregion Public Methods
}
