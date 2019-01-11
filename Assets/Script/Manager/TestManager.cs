using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    private void Awake()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        LevelManager lvl = FindObjectOfType<LevelManager>();
        if (gm == null && lvl != null)
            lvl.Init();
    }
}
