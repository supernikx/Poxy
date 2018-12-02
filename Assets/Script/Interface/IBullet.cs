using UnityEngine;

/// <summary>
/// Interfaccia implementata sui proiettili
/// </summary>
public interface IBullet
{
    void Shoot(float _speed, float _range, Transform _shootPosition, Vector3 _direction);
}
