using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorsButtonsManager : MonoBehaviour
{

    [Header("Element Containers")]
    [SerializeField]
    private Transform doorsContainer;
    [SerializeField]
    private Transform buttonsContainer;

    private List<IDoor> doors = new List<IDoor>();
    private List<IButton> buttons = new List<IButton>();

    #region API
    public void Init()
    {
        for (int i = 0; i < doorsContainer.childCount; i++)
        {
            IDoor _current = doorsContainer.GetChild(i).GetComponent<IDoor>();
            if (_current != null)
            {
                _current.Init();
                doors.Add(_current);
            }
        }

        for (int i = 0; i < buttonsContainer.childCount; i++)
        {
            IButton _current = doorsContainer.GetChild(i).GetComponent<IButton>();
            if (_current != null)
            {
                _current.Init();
                buttons.Add(_current);
            }
        }
    }
    #endregion

}
