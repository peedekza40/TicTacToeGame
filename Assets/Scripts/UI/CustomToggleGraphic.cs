using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggleGraphic : MonoBehaviour
{
    public GameObject ImageOn;
    public GameObject ImageOff;

    private void Awake() 
    {
        var isOn = GetComponent<Toggle>()?.isOn ?? false;
        SetGraphic(isOn);
    } 

    public void SetGraphic(bool isOn)
    {
        ImageOn?.SetActive(isOn);
        ImageOff?.SetActive(!isOn);
    }
}
