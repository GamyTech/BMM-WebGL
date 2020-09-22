using UnityEngine;
using System.Collections;
using GT.Backgammon.Player;

public class GameDiceView : MonoBehaviour
{
    public GT.Assets.DiceItemData DefaultdiceData;
    public PlayerDiceView BottomDiceView;
    public PlayerDiceView TopDiceView;

    private bool currentPlayerIsBottom;
    private bool rollAnimationFinished = false;

    public void Init(string playerOneDice = null, string playerTwoDice = null)
    {
        SetDiceId(true, playerOneDice);
        SetDiceId(false, playerTwoDice);

        BottomDiceView.OnView_EndDiceRoll += DiceView_OnView_EndDiceRoll;
        TopDiceView.OnView_EndDiceRoll += DiceView_OnView_EndDiceRoll;
    }

    void OnDestroy()
    {
        BottomDiceView.OnView_EndDiceRoll -= DiceView_OnView_EndDiceRoll;
        TopDiceView.OnView_EndDiceRoll -= DiceView_OnView_EndDiceRoll;
    }

    public void SetDiceId(bool bottomPlayer, string itemID)
    {
        GT.Assets.DiceItemData diceData = AssetController.Instance != null? AssetController.Instance.GetStoreAsset(itemID) as GT.Assets.DiceItemData : null;

        diceData = diceData ?? DefaultdiceData;

        if (bottomPlayer && BottomDiceView.GetCurrentDiceDataId() != itemID)
        {
            BottomDiceView.SetDiceData(diceData);
            GameSoundController.Instance.SetPlayerOneDiceSounds(diceData.RollSound, diceData.IdleSound);
        }
        else if (TopDiceView.GetCurrentDiceDataId() != itemID)
        {
            TopDiceView.SetDiceData(diceData);
            GameSoundController.Instance.SetPlayerTwoDiceSounds(diceData.RollSound, diceData.IdleSound);
        }
    }

    public void SetDiceAnimationSpeed(float speed)
    {
        TopDiceView.SetAnimationSpeed(speed);
        BottomDiceView.SetAnimationSpeed(speed);
    }

    public IEnumerator ShowDice(int[] dice, bool bottomPlayer)
    {
        rollAnimationFinished = false;
        currentPlayerIsBottom = bottomPlayer;

        GameSoundController.Instance.PlayDiceSound(currentPlayerIsBottom, false);

        if (currentPlayerIsBottom)
            BottomDiceView.ShowDiceRoll(dice);
        else
            TopDiceView.ShowDiceRoll(dice);

        yield return new WaitWhile(() => !rollAnimationFinished);
    }

    public void HideDice()
    {
        PlayerDiceView view = currentPlayerIsBottom ? BottomDiceView : TopDiceView;
        view.HideDice();
        GameSoundController.Instance.StopSecondSoundEffect();
    }

    private void DiceView_OnView_EndDiceRoll()
    {
        rollAnimationFinished = true;
        GameSoundController.Instance.PlayDiceSound(currentPlayerIsBottom, true);
    }

}
