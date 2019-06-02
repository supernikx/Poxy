using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthController : MonoBehaviour
{
    #region Delegates
    public delegate void PlayerHealthDelegates(float health);
    public static PlayerHealthDelegates OnHealthChange;
    #endregion

    [Header("Health Settings")]
    [SerializeField]
    [Tooltip("The time the energy will take to drop from max to 0")]
    private float timeToDeplete;
    [SerializeField]
    [Tooltip("The time the energy will take to increase from 0 to max")]
    private float timeToFill;
    [SerializeField]
    [Tooltip("Max Health")]
    private float maxHealth = 100;
    [SerializeField]
    [Tooltip("Min Health")]
    private float minHealth = 0;

    /// <summary>
    /// Player Health
    /// </summary>
    private float health
    {
        set
        {
            _health = value;
            if (OnHealthChange != null)
                OnHealthChange(_health);
        }
        get
        {
            return _health;
        }
    }
    private float _health;

    /// <summary>
    /// Amount of health lost every frame
    /// </summary>
    private float lossPerSecond;

    /// <summary>
    /// Amount of health gained every frame
    /// </summary>
    private float gainPerSecond;

    /// <summary>
    /// Reference to Player
    /// </summary>
    private Player player;

    /// <summary>
    /// Identifica se il player può seguire il default behaviour
    /// </summary>
    private bool canLoseHealth;

    /// <summary>
    /// Identifica se il player di default può perdere vita nel tempo
    /// </summary>
    private bool canLoseHealthDefaultBehaviour;

    #region API
    /// <summary>
    /// Initialize this script
    /// </summary>
    public void Init(Player _player)
    {
        lossPerSecond = (maxHealth - minHealth) / timeToDeplete;
        gainPerSecond = (maxHealth - minHealth) / timeToFill;
        player = _player;
        Setup();
    }

    public void Setup()
    {
        health = maxHealth;
        canLoseHealthDefaultBehaviour = canLoseHealth = true;
    }

    /// <summary>
    /// Lose health every update
    /// </summary>
    public void LoseHealthOverTime()
    {
        if (!canLoseHealthDefaultBehaviour || !canLoseHealth)
            return;

        health = Mathf.Clamp(health - lossPerSecond * Time.deltaTime, minHealth, maxHealth);
        if (health == minHealth)
            player.StartDeathCoroutine();
    }

    /// <summary>
    /// Funzione che diminuisce la vita del valore passato come parametro
    /// </summary>
    /// <param name="_health"></param>
    public void DamageHit(float _health)
    {
        health = Mathf.Clamp(health - _health, minHealth, maxHealth);
        if (health == minHealth)
            player.StartDeathCoroutine();
    }

    /// <summary>
    /// gain health every update
    /// </summary>
    public bool GainHealthOverTime()
    {
        health = Mathf.Clamp(health + gainPerSecond * Time.deltaTime, minHealth, maxHealth);
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

    #region Setter
    /// <summary>
    /// Funzione che imposta se il player esegue il defaul behaviour
    /// </summary>
    /// <param name="_enable"></param>
    public void SetCanLoseHealth(bool _enable)
    {
        canLoseHealth = _enable;
    }

    /// <summary>
    /// Funzione che imposta se il player può perdere vita overtime
    /// </summary>
    /// <param name="_enable"></param>
    public void SetCanLoseHealthDefaultBehaviour(bool _enable)
    {
        canLoseHealthDefaultBehaviour = _enable;
    }
    #endregion
    #endregion

}
