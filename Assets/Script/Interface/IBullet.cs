using UnityEngine;

/// <summary>
/// Interfaccia implementata sui proiettili
/// </summary>
public interface IBullet
{
    void Shot(float _speed, float _range, Transform _shootPosition, Vector3 _direction);
}
