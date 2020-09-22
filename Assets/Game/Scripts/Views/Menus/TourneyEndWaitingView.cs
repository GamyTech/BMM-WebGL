public class TourneyEndWaitingView : SmallLoadingView
{
    void OnEnable()
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.ViewShow);
    }
}
