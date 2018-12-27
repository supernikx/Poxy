using UnityEngine;
using System;
using TMPro;

/**
 * Nel SetCanStart c'è una riga usata solo per debug. DA RIMUOVERE
 * L'update è solo per debug. DA RIMUOVERE
 */

public class EnemyToleranceController : MonoBehaviour
{
    #region Delegates
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


    [Header("UI Settings")]
    [SerializeField]
    [Tooltip("Reference to the health text")]
    private TextMeshProUGUI ToleranceText;

    [SerializeField]
    private float tolerance;

    private bool active = false;
    //Provvisorio per debug
    public bool inState = false;

    /// <summary>
    /// Amount of tolerance gain every frame
    /// </summary>
    private float gainPerSecond;

    private void Update()
    {
        if (inState)
            ToleranceText.text = "Tolerance: " + Mathf.RoundToInt(tolerance) + " / " + maxTolerance;
    }

    #region API
    public void Init()
    {
        gainPerSecond = (maxTolerance - minTolerance) / timeToFill;
    }

    public void Setup()
    {
        tolerance = minTolerance;
        ToleranceText.text = "Tolerance: " + Mathf.RoundToInt(tolerance) + " / " + maxTolerance;
        inState = true;
    }

    /// <summary>
    /// Funzione che aggiunge tolleranza in base ai dati impostati
    /// </summary>
    public void AddTolleranceOvertime()
    {
        tolerance = Mathf.Clamp(tolerance + gainPerSecond * Time.deltaTime, minTolerance, maxTolerance);
    }

    /// <summary>
    /// Funzione che aumenta la tolleranza del valore passato come parametro
    /// </summary>
    public void AddTollerance(float _damage)
    {
        tolerance = Mathf.Clamp(tolerance + _damage, minTolerance, maxTolerance);
    }

    /// <summary>
    /// Funzione che controlla la tolleranza e ritorna true se è maggiore di quella massima
    /// </summary>
    /// <returns></returns>
    public bool CheckTollerance()
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
    #endregion

    #region Setters
    /// <summary>
    /// Funzione che attiva/disattiva la tolleranza
    /// </summary>
    /// <param name="_active"></param>
    public void SetActive(bool _active)
    {
        active = _active;
        if (!active)
        {
            inState = false;
            ToleranceText.text = "Tolerance: ";
        }
    }
    #endregion
    #endregion


}
