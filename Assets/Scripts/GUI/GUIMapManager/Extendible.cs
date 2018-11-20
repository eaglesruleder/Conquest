using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Extendible : MonoBehaviour {

    public GameObject button;
    public bool buttonActive = false;
    public float buttonPercent = 0f;

    public GameObject panel;
    public bool panelActive = false;
    public float panelPercent = 0f; 

    public void SwitchButtonActive()
    {
        buttonActive = !buttonActive;
    }

    public void SwitchPanelActive()
    {
        panelActive = !panelActive;
    }
}
