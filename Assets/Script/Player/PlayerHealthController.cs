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
    [SerializeField]
    private bool canDie = true;

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
    /// Reference to Player
    /// </summary>
    private Player player;

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
    public void Init(Player _player)
    {
        lossPerSecond = (maxHealth - minHealth) / timeToDeplete;
        health = maxHealth;

        player = _player;
    }

    /// <summary>
    /// Lose health every update
    /// </summary>
    public void LoseHealthOvertime()
    {
        health = Mathf.Clamp(health - lossPerSecond * Time.deltaTime, minHealth, maxHealth);
        if (health == minHealth && canDie)
        {
            if (player.OnPlayerDeath != null)
                player.OnPlayerDeath();
        }
    }

    /// <summary>
    /// Funzione che diminuisce la vita del valore passato come parametro
    /// </summary>
    /// <param name="_health"></param>
    public void LoseHealth(int _health)
    {
        health = Mathf.Clamp(health - _health, minHealth, maxHealth);
        if (health == minHealth && canDie)
        {
            if (player.OnPlayerDeath != null)
                player.OnPlayerDeath();
        }
    }

    /// <summary>
    /// gain health every update
    /// </summary>
    public bool GainHealthOvertime()
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
