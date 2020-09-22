using System.Collections.Generic;

public class BetRoom
{
    public float BetAmount { get; private set; }
    public float FeeAmount { get; private set; }
    public int Loyalty { get; private set; }
    public string Category { get; private set; }
    public Enums.MatchKind Kind { get; private set; }
    public bool Selected;

    public float WinAmount { get { return BetAmount * 2; } }
    public float TotalAmount { get { return BetAmount + FeeAmount; } }

    public BetRoom(float betAmount, float feeAmount, int loyalty, string category, Enums.MatchKind matchKind, bool selected)
    {
        BetAmount = betAmount;
        FeeAmount = feeAmount;
        Loyalty = loyalty;
        Category = category;
        Kind = matchKind;

        Selected = selected;
    }

    public BetRoom(BetRoom clone)
    {
        BetAmount = clone.BetAmount;
        FeeAmount = clone.FeeAmount;
        Loyalty = clone.Loyalty;
        Category = clone.Category;
        Kind = clone.Kind;

        Selected = clone.Selected;
    }

    public override string ToString()
    {
        return "RoomCategory: (Bet:" + BetAmount + "), (Fee:" + FeeAmount + "), (Loyalty:" + Loyalty +
            "), (MatchKind:" + Kind + "), (Category:" + Category + "), (Selected:" + Selected + ")";
    }

    public override bool Equals(object obj)
    {
        return BetAmount == obj.ParseFloat();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
