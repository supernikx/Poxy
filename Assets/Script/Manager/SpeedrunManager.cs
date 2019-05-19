using UnityEngine;
using System.Collections;
using System;

public class SpeedrunManager : MonoBehaviour
{
    #region Delegates
    public static Action StartTimer;
    public static Action StopTimer;

    // DA COLLEGARE
    public static Action PauseTimer;
    public static Action ResumeTimer;
    #endregion

    [SerializeField]
    private float timer;

    private bool canCount;
    private bool isActive;

    #region API
    public void Init(bool _isActive)
    {
        isActive = _isActive;

        if (isActive)
        {
            StartTimer += HandleStartTimer;
            StopTimer += HandleStopTimer; 
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator CTimerUpdate()
    {
        while (canCount)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    #region Handlers
    private void HandleStartTimer()
    {
        timer = 0;
        canCount = true;
        StopAllCoroutines();
        StartCoroutine(CTimerUpdate());
    }

    private void HandleStopTimer()
    {
        canCount = false;
        StopAllCoroutines();
        // timer is score
    }
    #endregion

    #region Getters
    public float GetTimer()
    {
        return timer;
    }

    public bool GetIsActive()
    {
        return isActive;
    }
    #endregion

    private void OnDisable()
    {
        StartTimer -= HandleStartTimer;
        StopTimer -= HandleStopTimer;
    }
}
