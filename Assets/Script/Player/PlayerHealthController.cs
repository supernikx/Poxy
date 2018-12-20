using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthController : MonoBehaviour {

    [Header("Health Settings")]
    [SerializeField]
    [Tooltip("The time the energy will take to drop from max to 0")]
    private float timeToDeplete;
    [SerializeField]
    [Tooltip("Max Health")]
    private float maxHealth = 100;
    [SerializeField]
    [Tooltip("Min Health")]
    private float minHealth = 0;

    [Header("UI Settings")]
    [SerializeField]
    [Tooltip("Reference to the health text")]
    private TextMeshProUGUI healthText;

    /// <summary>
    /// Player Health
    /// </summary>
    private float health;

    /// <summary>
    /// Amount of health lost every frame
    /// </summary>
    private float lossPerSecond;

    /// <summary>
    /// Temporary for debugging
    /// </summary>
    void Update()
    {
        healthText.text = "Health: " + Mathf.RoundToInt(health);
    }

    #region API
    /// <summary>
    /// Initialize this script
    /// </summary>
    public void Init()
    {
        lossPerSecond = (maxHealth - minHealth) / timeToDeplete;
        health = maxHealth;
    }

    /// <summary>
    /// Lose health every update
    /// </summary>
    public void LoseHealth()
    {
        health = Mathf.Clamp(health - lossPerSecond * Time.deltaTime, minHealth, maxHealth);
    }

    /// <summary>
    /// gain health every update
    /// </summary>
    public bool GainHealth()
    {
        health = Mathf.Clamp(health + lossPerSecond * Time.deltaTime, minHealth, maxHealth);
        if (health == maxHealth)
        {
            return true;
        }
        return false;
    }
    #endregion

}
