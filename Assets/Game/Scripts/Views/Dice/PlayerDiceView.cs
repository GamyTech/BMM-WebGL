using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GT.Backgammon.View;
using GT.Backgammon.Player;
using GT.Assets;

public class PlayerDiceView : BasePlayerDiceView
{
    public delegate void View_EndDiceRollHandler();
    public event View_EndDiceRollHandler OnView_EndDiceRoll;

    private SpriteRenderer topDiceSR;
    private SpriteRenderer bottomDiceSR;
    private List<Texture2D> topDiceList;
    private List<Texture2D> bottomDiceList;
    private DiceView dicePerfab;
    private string diceDataId = "VOID";
    private int diceOneNumber = 1;
    private int diceTwoNumber = 1;
    private bool reseted = true;

    public void SetDiceData(DiceItemData diceData)
    {
        if(dicePerfab != null)
            Destroy(dicePerfab.gameObject);

        LoadDiceData(diceData);

        if (reseted)
            ResetCube();
        else
            RollCube(diceOneNumber, diceTwoNumber);
    }

    public void SetAnimationSpeed(float speed)
    {
        dicePerfab.AnimatorController.speed = speed;
    }

    private void LoadDiceData(DiceItemData diceData)
    {
        if (diceData == null)
            return;

        diceDataId = diceData.itemId;
        GameSoundController.Instance.SetPlayerOneDiceSounds(diceData.RollSound, diceData.IdleSound);

        dicePerfab = Instantiate(diceData.prefab);
        dicePerfab.gameObject.InitGameObjectAfterInstantiation(transform);
        dicePerfab.SetAction(RollAnimationEnded);

        bottomDiceList = diceData.bottomDicePictures;
        topDiceList = diceData.topDicePictures;

        topDiceSR = dicePerfab.transform.Find("FinalDiceTop").GetComponent<SpriteRenderer>();
        bottomDiceSR = dicePerfab.transform.transform.Find("FinalDiceBottom").GetComponent<SpriteRenderer>();
    }

    private void RollCube(int up, int down)
    {
        dicePerfab.gameObject.SetActive(true);

        diceOneNumber = up;
        diceTwoNumber = down;
        reseted = false;

        dicePerfab.AnimatorController.SetBool("ResetDice", reseted);
        if (diceOneNumber < 0 || (diceOneNumber - 1) > topDiceList.Count)
            Debug.LogError("Die 1 result out of range : " + diceOneNumber);
        if (diceTwoNumber < 0 || (diceTwoNumber - 1) > topDiceList.Count)
            Debug.LogError("Die 2 result out of range : " + diceTwoNumber);

        topDiceSR.sprite = topDiceList[diceOneNumber - 1].ToSprite();
        bottomDiceSR.sprite = bottomDiceList[diceTwoNumber - 1].ToSprite();
        dicePerfab.AnimatorController.SetTrigger("RoleDice");      
    }

    private void ResetCube()
    {
        if (topDiceSR == null || bottomDiceSR == null || dicePerfab.AnimatorController == null)
            return;

        topDiceSR.enabled = false;
        bottomDiceSR.enabled = false;
        reseted = true;
        dicePerfab.AnimatorController.SetBool("ResetDice", reseted);
        topDiceSR.color = new Color(255, 255, 255, 1f);
        bottomDiceSR.color = new Color(255, 255, 255, 1f);
        dicePerfab.gameObject.SetActive(false);
    }

    public string GetCurrentDiceDataId()
    {
        return diceDataId;
    }

    public void RollAnimationEnded()
    {
        if (OnView_EndDiceRoll != null)
            OnView_EndDiceRoll();
    }

    public override void HideDice()
    {
        ResetCube();
    }

    public void ShowDiceRoll(int[] dice)
    {
        RollCube(dice[0], dice[1]);
    }

    public override IEnumerator ShowDice(int[] dice, IPlayer player)
    {
        yield return new WaitForEndOfFrame();
    }
}
