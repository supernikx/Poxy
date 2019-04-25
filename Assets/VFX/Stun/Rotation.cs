using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public AnimationCurve xVelocity, yVelocity, zVelocity;
    float time;
    IEnumerator rotatingCoroutineRef;

    private void OnEnable()
    {
        rotatingCoroutineRef = RotatingCoroutine();
        StartCoroutine(rotatingCoroutineRef);
    }

    IEnumerator RotatingCoroutine()
    {
        while (true)
        {
            time += Time.deltaTime;
            transform.Rotate(xVelocity.Evaluate(time), yVelocity.Evaluate(time), zVelocity.Evaluate(time));
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDisable()
    {
        StopCoroutine(rotatingCoroutineRef);
    }
}
