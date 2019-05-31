using UnityEngine;
using System;
using TMPro;
using System.Collections;

/**
 * Nel SetCanStart c'è una riga usata solo per debug. DA RIMUOVERE
 * L'update è solo per debug. DA RIMUOVERE
 */

public class EnemyToleranceController : MonoBehaviour
{
    #region Delegates
    public delegate void EnemyToeleranceDelegate(float tolerance);
    public static EnemyToeleranceDelegate OnToleranceChange;
    public Action OnMaxTolleranceBar;
    #endregion

    [Header("Health Settings")]
    [SerializeField]
    [Tooltip("The time the energy will take to drop from max to 0")]
    private float timeToFill;
    [SerializeField]
    [Tooltip("Max Health")]
    private float maxTolerance = 25;
    [SerializeField]
    [Tooltip("Min Health")]
    private float minTolerance = 0;



    private float tolerance
    {
        set
        {
            _tolerance = value;
            if (OnToleranceChange != null)
                OnToleranceChange(_tolerance);
        }
        get
        {
            return _tolerance;
        }
    }
    private float _tolerance;

    private bool active = false;
    private EnemyManager enemyMng;

    /// <summary>
    /// Amount of tolerance gain every frame
    /// </summary>
    private float gainPerSecond;

    #region API
    public void Init()
    {
        enemyMng = LevelManager.instance.GetEnemyManager();
        gainPerSecond = (maxTolerance - minTolerance) / timeToFill;
    }

    public void Setup()
    {
        tolerance = minTolerance;
    }

    /// <summary>
    /// Funzione che aggiunge tolleranza in base ai dati impostati
    /// </summary>
    public void AddTolleranceOvertime()
    {
        if (enemyMng != null && (!enemyMng.GetCanAddTolleranceDefaultBehaviour() || !enemyMng.GetCanAddTollerance()))
            return;

        tolerance = Mathf.Clamp(tolerance + gainPerSecond * Time.deltaTime, minTolerance, maxTolerance);
    }

    /// <summary>
    /// Funzione che aumenta la tolleranza del valore passato come parametro
    /// </summary>
    public void AddTolerance(float _damage, float _time = 0)
    {
        if (_time == 0)
        {
            tolerance = Mathf.Clamp(tolerance + _damage, minTolerance, maxTolerance);
        }
        else
        {
            StartCoroutine(AddToleranceOverTimeCoroutine(_damage, _time));
        }
    }
    /// <summary>
    /// Coroutine che fa aumentare la tollercane dle nemcio overtime
    /// </summary>
    /// <param name="_damage"></param>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator AddToleranceOverTimeCoroutine(float _damage, float _time)
    {
        float tickDuration = 0.5f;
        float damgeEachTick = tickDuration * _damage / _time;
        int ticks = Mathf.RoundToInt(_time / tickDuration);
        int tickCounter = 0;
        while (tickCounter < ticks)
        {
            tolerance = Mathf.Clamp(tolerance + damgeEachTick, minTolerance, maxTolerance);
            tickCounter++;
            yield return new WaitForSeconds(tickDuration);
        }
    }

    /// <summary>
    /// Funzione che controlla la tolleranza e ritorna true se è maggiore di quella massima
    /// </summary>
    /// <returns></returns>
    public bool CheckTolerance()
    {
        if (tolerance == maxTolerance)
            return true;
        return false;
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna se la tolleranza è attiva o no
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return active;
    }

    /// <summary>
    /// Funzione che ritorna la tolerance attuale
    /// </summary>
    /// <returns></returns>
    public float GetTolerance()
    {
        return tolerance;
    }

    /// <summary>
    /// Funzione che ritrona la tolleranza massima
    /// </summary>
    /// <returns></returns>
    public float GetMaxTolerance()
    {
        return maxTolerance;
    }
    #endregion

    #region Setters
    /// <summary>
    /// Funzione che attiva/disattiva la tolleranza
    /// </summary>
    /// <param name="_active"></param>
    public void SetActive(bool _active)
    {
        active = _active;
    }
    #endregion
    #endregion


}
