using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_LeaderboardField : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI timeText;

    public void Setup(string _nameText, string _timeText)
    {
        nameText.text = _nameText;
        timeText.text = _timeText;
    }
}
