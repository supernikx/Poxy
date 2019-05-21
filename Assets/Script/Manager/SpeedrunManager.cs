using UnityEngine;
using System.Collections;
using System;

public class SpeedrunManager : MonoBehaviour
{
    #region Delegates
    public static Action StartTimer;
    public static Action StopTimer;
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
            PauseTimer += HandlePauseTimer;
            ResumeTimer += HandleResumeTimer;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator CTimerUpdate()
    {
        while (true)
        {
            if (canCount)
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

    private void HandlePauseTimer()
    {
        canCount = false;
    }

    private void HandleResumeTimer()
    {
        canCount = true;
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
