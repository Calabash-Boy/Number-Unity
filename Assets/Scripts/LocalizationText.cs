using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    public string key = " ";
    private void Start()
    {
        GetComponent<Text>().text = LocalizationManager.GetInstance.GetValue(key);
    }  
}


