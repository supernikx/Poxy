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
    private bool isActive = false;

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
                timer += Time.deltaTime; 
            yield return null;
        }
    }

    private IEnumerator CStopForSeconds(float _value)
    {
        bonusTimer = 0;

        while (bonusTimer <= _value)
        {
            if (canCount)
                bonusTimer += Time.deltaTime;
            yield return null;
        }

        bonusTime = false;
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

    private void HandleStopForSeconds(float _val)
    {
        bonusTime = true;
        StartCoroutine(CStopForSeconds(_val));
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
        StopForSeconds -= HandleStopForSeconds;
    }
}
