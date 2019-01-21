using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthController : MonoBehaviour
{

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
    public void LoseHealthOverTime()
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
    public void DamageHit(int _health, float _time = 0)
    {
        if (_time == 0)
        {
            health = Mathf.Clamp(health - _health, minHealth, maxHealth);
            if (health == minHealth && canDie)
            {
                if (player.OnPlayerDeath != null)
                    player.OnPlayerDeath();
            }
        }
        else
        {
            StartCoroutine(DamageHitOverTime(_health, _time));
        }
    }
    /// <summary>
    /// Coroutine che fa perdere vita overtime al player
    /// </summary>
    /// <param name="_health"></param>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator DamageHitOverTime(int _health, float _time)
    {
        float tickDuration = 0.5f;
        float damgeEachTick = tickDuration * _health / _time;
        int ticks = Mathf.RoundToInt(_time / tickDuration);
        int tickCounter = 0;
        while (tickCounter < ticks)
        {
            health = Mathf.Clamp(health - damgeEachTick, minHealth, maxHealth);
            tickCounter++;
            yield return new WaitForSeconds(tickDuration);
        }
    }

    /// <summary>
    /// gain health every update
    /// </summary>
    public bool GainHealthOverTime()
    {
        health = Mathf.Clamp(health + lossPerSecond * Time.deltaTime, minHealth, maxHealth);
        if (health == maxHealth)
        {
            return true;
        }
        return false;
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna la vita attuale
    /// </summary>
    /// <returns></returns>
    public float GetHealth()
    {
        return health;
    }
    #endregion
    #endregion

}
