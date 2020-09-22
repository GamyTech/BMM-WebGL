using UnityEngine;
using System.Collections.Generic;
using GT.Backgammon.Player;
using System.Collections;
using UnityEngine.Events;

namespace GT.Backgammon.View
{
    public class TestPlayerDiceView : BasePlayerDiceView
    {
        private readonly Dictionary<PlayerColor, Color> playerColorDict = new Dictionary<PlayerColor, Color>()
        {
            { PlayerColor.Black, Color.black },
            { PlayerColor.White, Color.white },
        };

        private TestDieView[] m_dices;
        protected TestDieView[] Dices
        {
            get
            {
                if (m_dices == null)
                {
                    m_dices = new TestDieView[4];
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        m_dices[i] = transform.GetChild(i).GetComponent<TestDieView>();
                    }
                }
                return m_dices;
            }
        }

        private float animationLenth;

        #region Impelentations
        public void InitView(TestDiceViewInitData initData)
        {
            animationLenth = initData.AnimationLength;
        }

        public override IEnumerator ShowDice(int[] dice, IPlayer player)
        {
            float count = 0;
            float waitTime = .01f;
            while (count < animationLenth)
            {
                waitTime += .01f;
                ShowRandomDice(player);
                yield return new WaitForSeconds(waitTime);
                count += waitTime;
            }
            ShowDices(dice, player);
        }

        public override void HideDice()
        {
            for (int i = 0; i < Dices.Length; i++)
            {
                Dices[i].HideDie();
            }
        }

        //public override void SetDiceUsed(int[] dice, bool used)
        //{
        //    for (int i = 0; i < dice.Length; i++)
        //    {
        //        for (int j = 0; j < Dices.Length; j++)
        //        {
        //            if (Dices[j].Die == dice[i] && Dices[j].Used != used)
        //            {
        //                Dices[j].DieUsed(used);
        //                break;
        //            }
        //        }
        //    }
        //}

        //public override void SetDieUsed(int die, bool used)
        //{
        //    for (int j = 0; j < Dices.Length; j++)
        //    {
        //        if (Dices[j].Die == die && Dices[j].Used != used)
        //        {
        //            Dices[j].DieUsed(used);
        //            break;
        //        }
        //    }
        //}
        #endregion Impelentations

        #region Private Methods
        private void ShowRandomDice(IPlayer player)
        {
            int f, s;
            Logic.Dice.GetRandomDice(out f, out s);
            Dices[0].ShowDie(f, playerColorDict[player.playerColor], playerColorDict[player.playerColor].OppositeColor());
            Dices[1].ShowDie(s, playerColorDict[player.playerColor], playerColorDict[player.playerColor].OppositeColor());
        }

        private void ShowDices(int[] dice, IPlayer player)
        {
            for (int i = 0; i < dice.Length; i++)
            {
                Dices[i].ShowDie(dice[i], playerColorDict[player.playerColor], playerColorDict[player.playerColor].OppositeColor());
            }
        }
        #endregion Private Methods
    }
}
