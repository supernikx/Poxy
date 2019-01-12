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

    private BoxCollider boxCollider;
    float maxDistance;
    float maxXScale;

    public void Setup()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void Init(Vector3 _spawnPosition, Quaternion _rotation)
    {
        transform.position = _spawnPosition;
        transform.rotation = _rotation;

        Vector3 bottomLeftCorner = new Vector3(boxCollider.bounds.min.x, boxCollider.bounds.min.y, transform.position.z);
        Vector3 bottomRightCorner = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y, transform.position.z);
        maxDistance = Vector3.Distance(bottomLeftCorner, bottomRightCorner);
        maxXScale = transform.localScale.x;
    }

    public void Spawn(Vector3 _rightPosition, Vector3 _leftPostion)
    {
        float actualDistance = Vector3.Distance(_rightPosition, _leftPostion);
        transform.localScale = new Vector3(actualDistance * maxXScale / maxDistance, transform.localScale.y, transform.localScale.z);

        if (OnObjectSpawn != null)
            OnObjectSpawn(this);
    }

    public BoxCollider GetBoxCollider()
    {
        return boxCollider;
    }
}
