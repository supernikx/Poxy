using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class EnemyManager : MonoBehaviour
{
    #region Delegates
    public delegate void EnemyEvents(IEnemy e); //TODO: passare enemy e non game object
    public static EnemyEvents OnEnemyStun;
    public static EnemyEvents OnEnemyEndStun;
    public static EnemyEvents OnEnemyDeath;
    public static EnemyEvents OnEnemyEndDeath;
    #endregion

    [SerializeField]
    private Transform enemiesParent;
    [SerializeField]
    private LayerMask enemiesLayerMask;
    private int enemiesLayerValue;

    private List<IEnemy> enemyList;
    private List<IEnemy> stunnedEnemies;
    private List<IEnemy> deadEnemies;

    public void Init()
    {
        enemiesLayerValue = LayerMaskToLayer(enemiesLayerMask);

        enemyList = new List<IEnemy>();
        stunnedEnemies = new List<IEnemy>();
        deadEnemies = new List<IEnemy>();

        enemyList = (FindObjectsOfType<EnemyBase>() as IEnemy[]).ToList();

        OnEnemyStun += HandleEnemyStun;
        OnEnemyEndStun += HandleEnemyEndStun;
        OnEnemyDeath += HandleEnemyDeath;
        OnEnemyEndDeath += HandleEnemyEndDeath;
    }
    private void OnDisable()
    {
        OnEnemyStun -= HandleEnemyStun;
        OnEnemyDeath -= HandleEnemyDeath;
    }

    public void EnemiesSetup()
    {
        foreach (IEnemy e in enemyList)
        {
            e.Init(this);
        }
    }

    #region Handler
    private void HandleEnemyStun(IEnemy e)
    {
        if (!stunnedEnemies.Contains(e))
        {
            e.Stun();
            stunnedEnemies.Add(e);
        }
    }

    private void HandleEnemyEndStun(IEnemy e)
    {
        if (stunnedEnemies.Contains(e))
        {
            stunnedEnemies.Remove(e);
        }
    }

    private void HandleEnemyDeath(IEnemy e)
    {
        if (stunnedEnemies.Contains(e))
        {
            stunnedEnemies.Remove(e);
        }

        if (!deadEnemies.Contains(e))
        {
            e.Die();
            deadEnemies.Add(e);
        }
    }

    private void HandleEnemyEndDeath(IEnemy e)
    {
        if (deadEnemies.Contains(e))
        {
            deadEnemies.Remove(e);
        }
    }
    #endregion

    /// <summary>
    /// Ritorna l'id del layer corrispondente alla layer mask
    /// </summary>
    /// <param name="_layerMask"></param>
    /// <returns></returns>
    private int LayerMaskToLayer(LayerMask _layerMask)
    {
        int layerNumber = 0;
        int layer = enemiesLayerMask.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }

    #region API
    public IEnemy GetNearestStunnedEnemy(Transform _pointTransform, float _range)
    {
        if (stunnedEnemies == null || stunnedEnemies.Count <= 0)
            return null;

        IEnemy neartesEnemy = null;
        float nearestDistance = -1;
        foreach (IEnemy e in stunnedEnemies)
        {
            float distance = Vector3.Distance(_pointTransform.position, e.gameObject.transform.position);
            if (distance <= _range)
            {
                if (nearestDistance == -1 || distance < nearestDistance)
                {
                    nearestDistance = distance;
                    neartesEnemy = e;
                }
            }
        }
        return neartesEnemy;
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna il parent dei nemici
    /// </summary>
    /// <returns></returns>
    public Transform GetEnemyParent()
    {
        return enemiesParent;
    }

    /// <summary>
    /// Funzione che ritorna il layer dei nemici
    /// </summary>
    /// <returns></returns>
    public int GetEnemyLayer()
    {
        return enemiesLayerValue;
    }
    #endregion
    #endregion
}
