using UnityEngine;
using System.Collections;

public struct RoomCategory
{
    public int Index { get; private set; }
    public string Name { get; private set; }
    public string Sign { get; private set; }

    public RoomCategory(int id, string name, string sign)
    {
        Index = id;
        Name = name;
        Sign = sign;
    }
}
