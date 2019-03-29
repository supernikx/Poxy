using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeratore che contiene tutti gli oggetti presenti nella pool
/// </summary>
public enum ObjectTypes
{
    StunBullet,
    ParabolicBullet,
    StickyBullet,
    StickyObject,
    None,
}

/// <summary>
/// Classe che contiene le informazioni degli oggetti da mettere nella pool
/// </summary>
[System.Serializable]
public class PoolObjects
{
    public GameObject prefab;
    public int ammount;
    public ObjectTypes objectType;
}

/// <summary>
/// Pool Manager
/// </summary>
public class PoolManager : MonoBehaviour
{
    #region Events
    public delegate void PoolManagerEvent(IPoolObject pooledObject);
    public PoolManagerEvent OnObjectPooled;
    #endregion

    /// <summary>
    /// Singleton
    /// </summary>
    public static PoolManager instance;
    /// <summary>
    /// Lista degli oggetti da mettere nella pool
    /// </summary>
    public List<PoolObjects> poolObjects = new List<PoolObjects>();
    /// <summary>
    /// Pool location
    /// </summary>
    Vector3 poolPosition = new Vector3(1000, 1000, 1000);
    /// <summary>
    /// LIsta degli oggetti presenti nella pool
    /// </summary>
    Dictionary<ObjectTypes, List<IPoolObject>> poolDictionary;
    /// <summary>
    /// Pool parent
    /// </summary>
    Transform poolParent;

    /// <summary>
    /// Funzione che spawna tutti gli oggetti da mettere in pool
    /// e controlla se implementano l'interaccia IPoolObject
    /// </summary>
    public void Init()
    {
        instance = this;
        poolDictionary = new Dictionary<ObjectTypes, List<IPoolObject>>();
        poolParent = new GameObject("Pool").transform;
        poolParent.parent = transform;

        foreach (PoolObjects obj in poolObjects)
        {
            List<IPoolObject> objectsToAdd = new List<IPoolObject>();
            Transform spawnParent = new GameObject(obj.objectType.ToString()).transform;
            spawnParent.parent = poolParent;
            for (int i = 0; i < obj.ammount; i++)
            {
                GameObject instantiateObject = Instantiate(obj.prefab, spawnParent);
                IPoolObject instantiateObjectInterface = instantiateObject.GetComponent<IPoolObject>();
                if (instantiateObjectInterface != null)
                {
                    instantiateObjectInterface.OnObjectDestroy += OnObjectDestroy;
                    instantiateObjectInterface.OnObjectSpawn += OnObjectSpawn;
                    instantiateObjectInterface.Setup();
                    OnObjectDestroy(instantiateObjectInterface);
                    objectsToAdd.Add(instantiateObjectInterface);
                }
                else
                {
                    Debug.Log("il prefab: " + instantiateObject.ToString() + "     type:" + obj.objectType.ToString() + " non implementa l'interfaccia IPoolObject");
                    break;
                }
            }
            poolDictionary.Add(obj.objectType, objectsToAdd);
        }
        LevelManager.OnPlayerDeath += ResetPool;
    }

    /// <summary>
    /// Funzione che viene chiamata quando l'oggetto passato come parametro
    /// dev'essere rimesso nella pool
    /// </summary>
    /// <param name="objectToDestroy"></param>
    private void OnObjectDestroy(IPoolObject objectToDestroy)
    {
        objectToDestroy.CurrentState = State.InPool;
        objectToDestroy.gameObject.transform.position = poolPosition;
        objectToDestroy.ownerObject = null;
    }

    /// <summary>
    /// Funzione che viene chiamata quando l'oggetto passato come parametro
    /// dev'essere messo in gioco
    /// </summary>
    /// <param name="objectToSpawn"></param>
    private void OnObjectSpawn(IPoolObject objectToSpawn)
    {
        objectToSpawn.CurrentState = State.InUse;
    }

    /// <summary>
    /// Funzione che reimposta la pool
    /// </summary>
    public void ResetPool()
    {
        foreach (ObjectTypes type in poolDictionary.Keys)
        {
            foreach (IPoolObject pooledObject in poolDictionary[type])
            {
                OnObjectDestroy(pooledObject);
            }
        }
    }

    /// <summary>
    /// Funzione che ritorna il primo oggetto disponbile presente nella pool
    /// del tipo passato come parametro, assegna una referenza dell'owner all'oggetto
    /// </summary>
    /// <param name="type"></param>
    /// <param name="_ownerObject"></param>
    /// <returns></returns>
    public GameObject GetPooledObject(ObjectTypes type, GameObject _ownerObject)
    {
        foreach (IPoolObject _object in poolDictionary[type])
        {
            if (_object.CurrentState == State.InPool)
            {
                _object.ownerObject = _ownerObject;
                if (OnObjectPooled != null)
                {
                    OnObjectPooled(_object);
                }
                return _object.gameObject;
            }
        }
        Debug.Log("Nessun " + type + " disponibile");
        return null;
    }

    /// <summary>
    /// Se viene disabilitato distrugge tutti gli oggetti presenti nella pool
    /// </summary>
    private void OnDisable()
    {
        LevelManager.OnPlayerDeath -= ResetPool;
        poolDictionary = null;
        //foreach (ObjectTypes type in poolDictionary.Keys)
        //{
        //    foreach (IPoolObject pooledObject in poolDictionary[type])
        //    {
        //        pooledObject.OnObjectDestroy -= OnObjectDestroy;
        //        pooledObject.OnObjectSpawn -= OnObjectSpawn;
        //    }
        //}
    }
}
