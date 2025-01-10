using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeyBind : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
            ui.SetActive(!ui.activeSelf);
    }
}
