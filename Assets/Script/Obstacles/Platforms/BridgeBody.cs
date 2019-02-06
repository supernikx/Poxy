using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BridgeBody : MonoBehaviour, ISticky
{
    private Sequence sequence;

    public void Init(Sequence _sequence)
    {
        sequence = _sequence;
    }

    public void OnSticky(Direction _direction)
    {
        sequence.Pause();
    }

    public void OnStickyEnd()
    {
        sequence.Play();
    }
}
