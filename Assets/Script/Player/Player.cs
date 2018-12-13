﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.PlayerSM;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    [Header("Other Settings")]
    [SerializeField]
    GameObject graphics;

    /// <summary>
    /// Riferimento al player controller
    /// </summary>
    private PlayerCollisionController collisionCtrl;
    /// <summary>
    /// Riferimento al shoot controller
    /// </summary>
    private PlayerShootController shootCtrl;
    /// <summary>
    /// Riferimento al player movement controller
    /// </summary>
    private PlayerMovementController movementCtrl;
    /// <summary>
    /// Riferimento al parasite controller
    /// </summary>
    private PlayerParasiteController parasiteCtrl;
    /// <summary>
    /// Riferimento alla state machine del player
    /// </summary>
    private PlayerSMController playerSM;

    #region API
    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init()
    {
        //Prendo le referenze ai component e li inizializzo
        collisionCtrl = GetComponent<PlayerCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        shootCtrl = GetComponent<PlayerShootController>();
        if (shootCtrl != null)
            shootCtrl.Init(PoolManager.instance);

        movementCtrl = GetComponent<PlayerMovementController>();
        if (movementCtrl != null)
            movementCtrl.Init(collisionCtrl);

        parasiteCtrl = GetComponent<PlayerParasiteController>();
        if (parasiteCtrl != null)
            parasiteCtrl.Init(this);

        playerSM = GetComponent<PlayerSMController>();
        if (playerSM != null)
            playerSM.Init(this);
    }

    /// <summary>
    /// Funzione che manda il player nello stato di parassita
    /// rispetto al nemico passato come parametro
    /// </summary>
    /// <param name="e"></param>
    public void Parasite(IEnemy e)
    {
        parasiteCtrl.SetParasiteEnemy(e);
        e.Parasite(this);

        if (playerSM.OnPlayerParaiste != null)
            playerSM.OnPlayerParaiste(e);
    }

    /// <summary>
    /// Funzione che manda il player nello stato normale
    /// </summary>
    public void Normal()
    {
        parasiteCtrl.GetParasiteEnemy().Stun();
        parasiteCtrl.SetParasiteEnemy(null);

        if (playerSM.OnPlayerNormal != null)
            playerSM.OnPlayerNormal();
    }

    /// <summary>
    /// Funzione che attiva/disattiva la grafica in base al parametro passato
    /// </summary>
    /// <param name="_switch"></param>
    public void EnableGraphics(bool _switch)
    {
        graphics.SetActive(_switch);
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna lo shoot controller
    /// </summary>
    /// <returns></returns>
    public PlayerShootController GetShootController()
    {
        return shootCtrl;
    }
    /// <summary>
    /// Funzione che ritorna il collision controller
    /// </summary>
    /// <returns></returns>
    public PlayerCollisionController GetCollisionController()
    {
        return collisionCtrl;
    }
    /// <summary>
    /// Funzione che ritorna il movement controller
    /// </summary>
    /// <returns></returns>
    public PlayerMovementController GetMovementController()
    {
        return movementCtrl;
    }
    /// <summary>
    /// Funzione che ritorna il parasite controller
    /// </summary>
    /// <returns></returns>
    public PlayerParasiteController GetParasiteController()
    {
        return parasiteCtrl;
    }
    #endregion
    #endregion
}
