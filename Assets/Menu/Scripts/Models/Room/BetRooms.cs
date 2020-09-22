using GT.User;
using System.Collections.Generic;
using System;

public class BetRooms{

    // Use this for initialization
    public static Dictionary<string, List<BetRoom>> GetBetRooms(List<float> bets, List<float> fees, List<int> loyalties, List<string> categories, Enums.MatchKind kind)
    {
        Dictionary<string, List<BetRoom>> rooms = new Dictionary<string, List<BetRoom>>();
        for (int i = 0; i < bets.Count; i++)
        {
            BetRoom room = new BetRoom(bets[i], fees[i], loyalties[i], categories[i], kind, false);
            List<BetRoom> roomsInCat;
            if (rooms.TryGetValue(categories[i], out roomsInCat))
                roomsInCat.Add(room);
            else
                rooms.Add(categories[i], new List<BetRoom>() { room });

            BetRoom customRoom = new BetRoom(bets[i], fees[i], loyalties[i], ContentController.CustomCatId, kind, false);
            if (rooms.TryGetValue(ContentController.CustomCatId, out roomsInCat))
                roomsInCat.Add(customRoom);
            else
                rooms.Add(ContentController.CustomCatId, new List<BetRoom>() { customRoom });
        }

        return rooms;
    }

    public static void RefreshBetSelection(string cat, Enums.MatchKind kind, Dictionary<string, float[]> savedSelectedInCat)
    {
        List<BetRoom> rooms = null;
        if(ContentController.GetByCategory(kind).TryGetValue(cat, out rooms))
        {
            if (cat != ContentController.CustomCatId)
            {
                float[] selectedAmounts;
                for (int x = 0; x < rooms.Count; ++x)
                    rooms[x].Selected = !savedSelectedInCat.TryGetValue(cat, out selectedAmounts) || selectedAmounts.Contains(rooms[x].BetAmount);
            }
            else
            {
                float[] selectedAmounts;
                if(savedSelectedInCat.TryGetValue(ContentController.CustomCatId, out selectedAmounts))
                {
                    for (int x = 0; x < rooms.Count; ++x)
                        rooms[x].Selected = selectedAmounts.Contains(rooms[x].BetAmount);
                }
                else
                {
                    float availableMoney = UserController.Instance.wallet.AvailableCurrency(kind);
                    int selectCount = 0;
                    for (int x = rooms.Count - 1; x >= 0; --x)
                    {
                        bool selected = selectCount < 3 && rooms[x].BetAmount <= availableMoney;
                        rooms[x].Selected = selected;
                        if (selected)
                            ++selectCount;
                    }
                }
            }
        }
    }

    public static List<BetRoom> CloneBetsList(List<BetRoom> bets)
    {
        List<BetRoom> newBets = new List<BetRoom>();
        for (int x = 0; x < bets.Count; ++x)
            newBets.Add(new BetRoom(bets[x]));
        return newBets;
    }
}
