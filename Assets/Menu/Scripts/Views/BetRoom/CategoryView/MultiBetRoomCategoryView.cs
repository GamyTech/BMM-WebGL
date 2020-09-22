public class MultiBetRoomCategoryView : BetRoomCategoryView
{
    public RoomCollectionView roomCollection;

    protected override void InitRooms()
    {
        roomCollection.Init(Rooms, true);
    }
}
