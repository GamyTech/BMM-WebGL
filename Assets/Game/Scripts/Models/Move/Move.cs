using GT.Backgammon.Player;
using System.Collections.Generic;
using System.Text;

namespace GT.Backgammon.Logic
{
    public class Move
    {
        public int from;
        public int to;
        public PlayerColor color;
        public int dice;
        public bool isEaten;

        public Move(int from, int to, PlayerColor color, int dice, bool isEaten)
        {
            this.from = from;
            this.to = to;
            this.color = color;
            this.dice = dice;
            this.isEaten = isEaten;
        }

        public override string ToString()
        {
            return "[ " + color + " (" + from + "," + to + ") Die: " + dice + " Eaten: " + isEaten + " ]";
        }

        #region Static Functions
        public static int[] GetDiceUsed(params Move[] moves)
        {
            int[] dice = new int[moves.Length];
            for (int i = 0; i < moves.Length; i++)
            {
                dice[i] = moves[i].dice;
            }
            return dice;
        }
        #endregion Static Functions

        #region Serialization
        public static string Serialize(Move move)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}:{1}:{2}:{3}:{4}", move.from, move.to, move.color.ToString()[0], move.dice, move.isEaten.ToString()[0]);
            return sb.ToString();
        }

        public static string SerializeMoves(Move[] moves)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < moves.Length; i++)
            {
                if(i!=0)
                    sb.Append(",");
                sb.Append(Serialize(moves[i]));
            }
            return sb.ToString();
        }

        public static Move[] DeserializeMoves(string str)
        {
            List<Move> movesList = new List<Move>();
            if (string.IsNullOrEmpty(str) == false)
            {
                string[] movesString = str.Split(',');
                for (int i = 0; i < movesString.Length; i++)
                {
                    movesList.Add(Deserialize(movesString[i]));
                }
            }
            return movesList.ToArray();
        }

        public static Move Deserialize(string moveString)
        {
            string[] splitString = moveString.Split(':');
            int from = splitString[0].ParseInt();
            int to = splitString[1].ParseInt();
            PlayerColor color = splitString[2] == "W" ? PlayerColor.White : PlayerColor.Black;
            int dice = splitString[3].ParseInt();
            bool eaten = splitString[4] == "T" ? true : false;
            return new Move(from, to, color, dice, eaten);
        }
        #endregion Serialization
    }
}
