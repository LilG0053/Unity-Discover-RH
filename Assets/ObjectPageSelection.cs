using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discover.Menus;
using Discover;

public class ObjectPageSelection : MonoBehaviour
{
    private CurrentAppMenuController appMenu;

    private void Start()
    {
        appMenu = GameObject.FindObjectOfType<CurrentAppMenuController>(true).GetComponent<CurrentAppMenuController>();
        if (appMenu)
        {
            Debug.Log("App menu found!");
        }
    }
    public void Selected()
    {
        Debug.Log("Selected object");
        NetworkApplicationContainer container = GetComponentInParent<NetworkApplicationContainer>();
        appMenu.InitializeApp(container.AppName);
        NetworkApplicationManager.Instance.CurrentApplication = container;
    }
}          
