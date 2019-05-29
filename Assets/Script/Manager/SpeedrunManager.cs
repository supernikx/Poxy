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
    public static Action<float> StopForSeconds;

    public static Action<float> OnTimerUpdate;
    public static Action<bool> OnTimerHold;
    #endregion

    [SerializeField]
    private float timer;
    [SerializeField]
    private float bonusTimer;

    private bool canCount;
    /// <summary>
    /// Attivo quando ci sono secondi guadagnati dai nemici
    /// </summary>
    private bool bonusTime = false;
    private static bool isActive = false;

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
            StopForSeconds += HandleStopForSeconds;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator CTimerUpdate()
    {
        while (true)
        {
            if (canCount && !bonusTime)
            {
                timer += Time.deltaTime;
                if (OnTimerUpdate != null)
                    OnTimerUpdate(timer);
            }
            yield return null;
        }
    }

    private IEnumerator CStopForSeconds(float _value)
    {
        bonusTimer = 0;

        if (OnTimerHold != null)
            OnTimerHold(true);

        while (bonusTimer <= _value)
        {
            if (canCount)
                bonusTimer += Time.deltaTime;
            yield return null;
        }

        if (OnTimerHold != null)
            OnTimerHold(false);
        
        bonusTime = false;
        stopForSecondsC = null;
    }
    #endregion

    #region Handlers
    private void HandleStartTimer()
    {
        timer = 0;
        canCount = true;
        bonusTime = false;
        StopAllCoroutines();
        StartCoroutine(CTimerUpdate());
        StopForSeconds += HandleStopForSeconds;
    }

    private void HandleStopTimer()
    {
        StopForSeconds -= HandleStopForSeconds;
        canCount = false;
        StopAllCoroutines();
        // timer is score
    }

    private void HandlePauseTimer()
    {
        StopForSeconds -= HandleStopForSeconds;
        canCount = false;
    }

    private void HandleResumeTimer()
    {
        StopForSeconds += HandleStopForSeconds;
        canCount = true;
    }

    Coroutine stopForSecondsC = null;
    private void HandleStopForSeconds(float _val)
    {
        bonusTime = true;
        if (stopForSecondsC != null)
        {
            StopCoroutine(stopForSecondsC);
        }
        stopForSecondsC = StartCoroutine(CStopForSeconds(_val));
    }
    #endregion

    #region Getters
    public float GetTimer()
    {
        return timer;
    }

    public static bool GetIsActive()
    {
        return isActive;
    }
    #endregion
}
