using System;
using UnityEngine;
using UnityEngine.UI;

public class OpenClosePanel : MonoBehaviour
{
    public GameObject panel;
    public Image anotherPanel;
    
    public bool isOpened = false;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (RobloxStyleController.instance.isCursorEnabled == false)
        {
            if (isOpened == true)
            {
                OpenClose();
            }
        }
    }

    public void OpenClose()
    {
        isOpened = !isOpened;

        if (isOpened)
        {
            anotherPanel.raycastTarget = false;
            panel.SetActive(true);
        }
        else
        {
            anotherPanel.raycastTarget = true;
            panel.SetActive(false);
        }
    }
}
