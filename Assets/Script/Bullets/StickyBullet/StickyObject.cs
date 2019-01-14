using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyObject : MonoBehaviour, IPoolObject
{
    #region IPoolObject
    public GameObject ownerObject
    {
        get
        {
            return _ownerObject;
        }
        set
        {
            _ownerObject = value;
        }
    }
    GameObject _ownerObject;

    public State CurrentState
    {
        get
        {
            return _currentState;
        }

        set
        {
            _currentState = value;
        }
    }
    State _currentState;

    public event PoolManagerEvets.Events OnObjectSpawn;
    public event PoolManagerEvets.Events OnObjectDestroy;
    #endregion

    [Header("Stickyobject Settings")]
    [SerializeField]
    private float duration;
    [SerializeField]
    private float minRange;
    [SerializeField]
    private int stickyObjectRayCount = 4;
    [SerializeField]
    private LayerMask stickyObjectCollisionLayer;

    private BoxCollider boxCollider;
    float maxDistance;
    float maxXScale;

    #region API
    public void Setup()
    {
        boxCollider = GetComponent<BoxCollider>();
        Vector3 bottomLeftCorner = new Vector3(boxCollider.bounds.min.x, boxCollider.bounds.min.y, transform.position.z);
        Vector3 bottomRightCorner = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y, transform.position.z);
        maxDistance = Vector3.Distance(bottomLeftCorner, bottomRightCorner);
        maxXScale = transform.localScale.x;
    }

    public void Init(Vector3 _spawnPosition, Quaternion _rotation)
    {
        transform.position = _spawnPosition;
        transform.rotation = _rotation;
    }

    public void Spawn(Vector3 _rightPosition, Vector3 _leftPostion)
    {
        float actualDistance = Vector3.Distance(_rightPosition, _leftPostion);

        if (actualDistance < minRange)
        {
            if (OnObjectDestroy != null)
                OnObjectDestroy(this);
        }
        else
        {
            Vector3 newScale = new Vector3(actualDistance * maxXScale / maxDistance, transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
            boxCollider.size.Scale(newScale);
            transform.position = Vector3.Lerp(_rightPosition, _leftPostion, 0.5f);
            StartCoroutine(DespawnCoroutine());

            if (OnObjectSpawn != null)
                OnObjectSpawn(this);
        }
    }

    #region Getter
    public float GetDuration()
    {
        return duration;
    }

    public int GetRayCount()
    {
        return stickyObjectRayCount;
    }

    public LayerMask GetCollisionLayer()
    {
        return stickyObjectCollisionLayer;
    }

    public BoxCollider GetBoxCollider()
    {
        return boxCollider;
    }
    #endregion
    #endregion

    /// <summary>
    /// Funzione che conta il tempo per far despawnare l'oggetto
    /// </summary>
    /// <returns></returns>
    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(duration);
        if (OnObjectDestroy != null)
            OnObjectDestroy(this);
    }
}
