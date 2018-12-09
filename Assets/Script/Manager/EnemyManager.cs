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
    #endregion

    public List<IEnemy> enemyList;
    public List<IEnemy> stunnedEnemies;

    public void Init()
    {
        enemyList = new List<IEnemy>();
        stunnedEnemies = new List<IEnemy>();

        enemyList = (FindObjectsOfType<EnemyBase>() as IEnemy[]).ToList();

        EnemySetup();

        OnEnemyStun += HandleEnemyStun;
        OnEnemyEndStun += HandleEnemyEndStun;
    }
    private void OnDisable()
    {
        OnEnemyStun -= HandleEnemyStun;
    }

    private void EnemySetup()
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
    #endregion

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
    #endregion
}
