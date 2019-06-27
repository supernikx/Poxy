using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateImage : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite defaultImage;
    [SerializeField]
    private Sprite selectedImage;

    public void Selected(bool _selected)
    {
        if (_selected)
            image.sprite = selectedImage;
        else
            image.sprite = defaultImage;
    }
}
