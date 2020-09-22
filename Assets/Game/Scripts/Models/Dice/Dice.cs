using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GT.Backgammon.Logic
{
    public class Dice
    {
        private const int MIN_DIE = 1;
        private const int MAX_DIE = 7;

        private int m_firstDie;
        private int m_secondDie;
        private bool m_isRolled;
        private bool m_isSet;

        public int FirstDie
        {
            get { return m_firstDie; }
            private set { m_firstDie = value; }
        }
        public int SecondDie
        {
            get { return m_secondDie; }
            private set { m_secondDie = value; }
        }
        public bool IsRolled
        {
            get { return m_isRolled; }
            private set { m_isRolled = value; }
        }

        public bool IsSet
        {
            get { return m_isSet; }
            private set { m_isSet = value; }
        }


        public Dice()
        {
            ResetDice();
        }

        public void ResetDice()
        {
            IsRolled = false;
            IsSet = false;
        }

        public bool RollDice()
        {
            if (IsRolled)
            {
                Debug.LogWarning("Dice Already Rolled. Reset First!");
                return false;
            }
            IsRolled = true;
            return true;
        }

        public bool SetDice(int first, int second)
        {
            if (IsSet)
            {
                Debug.LogWarning("Dice Already Set. Reset First!");
                return false;
            }
            IsSet = true;
            FirstDie = first;
            SecondDie = second;
            return true;
        }

        public int[] GetListOfDice()
        {
            int[] list;
            if (FirstDie == SecondDie)
                list = new int[] { FirstDie, FirstDie, FirstDie, FirstDie };
            else
                list = new int[] { FirstDie, SecondDie };
            return list;
        }

        public override string ToString()
        {
            return "Rolled: " + IsRolled + "Set: " + IsSet + " (" + FirstDie.ToString() + "," + SecondDie.ToString() + ")";
        }

        #region Static Random Generator
        private static System.Random rand = new System.Random();

        /// <summary>
        /// Normal dice roll
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static void GetRandomDice(out int first, out int second)
        {
            first = rand.Next(MIN_DIE, MAX_DIE);
            second = rand.Next(MIN_DIE, MAX_DIE);
        }

        /// <summary>
        /// Cannot be double
        /// </summary>
        /// <returns>Dice string</returns>
        public static void GetRandomStartDice(out int first, out int second)
        {
            first = rand.Next(MIN_DIE, MAX_DIE);
            List<int> nums = new List<int>();
            for (int i = MIN_DIE; i < MAX_DIE; i++)
            {
                nums.Add(i);
            }
            nums.Remove(first);

            int index = rand.Next(0, nums.Count - 1);
            second = nums[index];
        }
        #endregion Static Random Generator
    }
}
