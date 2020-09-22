using UnityEngine;
using System.Collections;

public delegate void ActiveStateAndSizeChanged();

public interface IResizable
{
    event ActiveStateAndSizeChanged OnActiveStateAndSizeChanged;
}
