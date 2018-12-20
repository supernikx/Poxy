using UnityEngine;
using System.Collections;
using TMPro;

/**
 * Nel SetCanStart c'è una riga usata solo per debug. DA RIMUOVERE
 * L'update è solo per debug. DA RIMUOVERE
 */

public class EnemyToleranceController : MonoBehaviour
{
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

    private bool canStart = false;

    /// <summary>
    /// Amount of tolerance gain every frame
    /// </summary>
    private float gainPerSecond;

    private void Update()
    {
        if (canStart)
        {
            ToleranceText.text = "Tolerance: " + Mathf.RoundToInt(tolerance) + " / " + maxTolerance;
        }
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
    }

    public bool CheckTolerance ()
    {
        if (canStart)
        {
            tolerance = Mathf.Clamp(tolerance + gainPerSecond * Time.deltaTime, minTolerance, maxTolerance);

            if (tolerance == maxTolerance)
            {
                return true;
            }
        }

        return false;
        
    }

    #region Setters
    public void SetCanStart(bool _canStart)
    {
        canStart = _canStart;
        if (!_canStart)
        {
            ToleranceText.text = "Tolerance: ";
        }
    }
    #endregion
    #endregion


}
