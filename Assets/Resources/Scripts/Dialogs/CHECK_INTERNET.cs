using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CHECK_INTERNET : MonoBehaviour
{
    GameObject mainCanvas;
    MainScript main;

    public Button retry;

    public void BeginChecking()
    {

        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();

        retry.onClick.AddListener(() =>
        {
            main.InternetCheck();
            main.dm.CloseCurrentDialog();
        });
    }
}
