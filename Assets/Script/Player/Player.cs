using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// Riferimento alla state machine del player
    /// </summary>
    private Animator playerSM;

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

        playerSM = GetComponent<Animator>();
        playerSM.SetTrigger("StartSM");
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
    /// Funzione che ritorna il movement controller
    /// </summary>
    /// <returns></returns>
    public PlayerMovementController GetMovementController()
    {
        return movementCtrl;
    }
    #endregion
    #endregion
}
