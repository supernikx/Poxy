
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GIFBehaviour : MonoBehaviour
{
    [SerializeField]
    Image gifImage;
    [SerializeField]
    Sprite[] frames;
    [SerializeField]
    int framesPerSecond = 10;
    float index;

    private void Update()
    {
        index = Time.unscaledTime * framesPerSecond;
        index = index % frames.Length;
        gifImage.sprite = frames[(int)index];
    }
}
