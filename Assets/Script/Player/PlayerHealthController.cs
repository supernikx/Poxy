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
    /// if true the player lose life, if false gains life
    /// </summary>
    private bool inNormalState = true;

    /// <summary>
    /// true if health is max
    /// </summary>
    private bool isHealthFull = true;

    /// <summary>
    /// Lose health every update
    /// </summary>
    private void LoseHealth()
    {
        health = Mathf.Clamp(health - lossPerSecond * Time.deltaTime, minHealth, maxHealth);
    }

    /// <summary>
    /// gain health every update
    /// </summary>
    private void GainHealth()
    {
        health = Mathf.Clamp(health + lossPerSecond * Time.deltaTime, minHealth, maxHealth);
    }

    /// <summary>
    /// Execute the functions to update health
    /// </summary>
    void Update()
    {

        if (inNormalState)
        {
            LoseHealth();
        }
        else
        {
            GainHealth();
        }

        if (health == maxHealth)
            isHealthFull = true;
        else
            isHealthFull = false;

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

    #region Setters
    /// <summary>
    /// set the inNormalState variable
    /// </summary>
    /// <param name="_newState"></param>
    public void SetInNormalState (bool _newState)
    {
        inNormalState = _newState;
    }
    #endregion

    #region Getters
    public bool GetIsHealthFull()
    {
        return isHealthFull;
    }
    #endregion
    #endregion

}
