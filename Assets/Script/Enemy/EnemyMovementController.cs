using UnityEngine;
using System.Collections;

public class EnemyMovementController : MonoBehaviour
{
    [Header("Gravity Settings")]
    /// <summary>
    /// Variabile che definisce la gravità applicata overtime
    /// se non si è in collisione sopra/sotto
    /// </summary>
    [SerializeField]
    private float gravity;

    /// <summary>
    /// Riferimento al collision controller
    /// </summary>
    private EnemyCollisionController collisionCtrl;

    /// <summary>
    /// Velocità del nemico su ogni asse
    /// </summary>
    [SerializeField]
    private Vector3 movementVelocity;

    /// <summary>
    /// Funzione che aggiunge gravità al player
    /// </summary>
    private void AddGravity()
    {
        //Aggiungo gravità al player
        movementVelocity.y += gravity * Time.deltaTime;
    }

    #region API
    /// <summary>
    /// Funzione di inizializzazione del player
    /// </summary>
    public void Init(EnemyCollisionController _collisionCtrl)
    {
        collisionCtrl = _collisionCtrl;
    }

    public Vector3 GravityCheck()
    {

        if (collisionCtrl.collisions.above || collisionCtrl.collisions.below)
        {
            //Se sono in collisione con qualcosa sopra/sotto evito di accumulare gravità
            movementVelocity.y = 0;
        }

        AddGravity();

        return movementVelocity;
        //transform.Translate(collisionCtrl.CheckMovementCollisions(movementVelocity * Time.deltaTime));
    }
    #endregion

}
