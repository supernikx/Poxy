using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public AnimationCurve xVelocity, yVelocity, zVelocity;
    float time;

    void Update()
    {
        time += Time.deltaTime;
        transform.Rotate(xVelocity.Evaluate(time),  yVelocity.Evaluate(time), zVelocity.Evaluate(time));
    }
}
