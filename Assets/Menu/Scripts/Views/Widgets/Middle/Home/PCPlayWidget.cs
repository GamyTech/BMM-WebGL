public class PCPlayWidget : PlayWidget
{
    #region Input
    public void NextPage()
    {
        ChangeCurrentCategory(1);
    }

    public void PreviousPage()
    {
        ChangeCurrentCategory(-1);
    }
    #endregion Input
}
