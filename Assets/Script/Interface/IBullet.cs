using UnityEngine;

/// <summary>
/// Interfaccia implementata sui proiettili
/// </summary>
public interface IBullet
{
    void Shot(float _speed, float _range, Transform _shotPosition, Vector3 _direction);
    void Shot(float _speed, float _range, Transform _shotPosition, Transform _target);
}
