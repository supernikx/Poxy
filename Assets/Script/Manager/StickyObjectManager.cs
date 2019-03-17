using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class StickyObjectManager : MonoBehaviour
{
    private List<StickyObject> stickyObjects = new List<StickyObject>();

    public void Init()
    {
        stickyObjects = FindObjectsOfType<StickyObject>().ToList();
    }

    void Update()
    {
        foreach (StickyObject sticky in stickyObjects)
        {
            if (sticky.CurrentState == State.InUse)
                sticky.StickyBehaviour();
        }
    }
}
