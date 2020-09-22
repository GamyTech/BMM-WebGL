using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerStats {

    public string id;
    public int eaten;
    public int sixFive;
    public int doubles;

    public PlayerStats(Dictionary<string,object> data, int key)
    {
        Dictionary<string, object> doublesDict = (Dictionary<string, object>)data["Doubles"];
        Dictionary<string, object> eatenDict = (Dictionary<string, object>)data["Eaten"];
        Dictionary<string, object> sixFivesDict = (Dictionary<string, object>)data["SixFive"];

        id = doublesDict.ElementAt(key).Key;
        doubles = doublesDict[id].ParseInt();
        sixFive = sixFivesDict[id].ParseInt();
        eaten = eatenDict[id].ParseInt();
    }
}
