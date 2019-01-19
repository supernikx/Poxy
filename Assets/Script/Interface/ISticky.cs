using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Above,
    Below,
    None,
}

public interface ISticky
{
    void OnSticky(Direction _direction);
    void OnStickyEnd();
}
