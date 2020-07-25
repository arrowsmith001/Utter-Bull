using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CHECK_FIREBASE : MonoBehaviour
{

    GameObject mainCanvas;
    MainScript main;

    public Button retry;

    private void Start()
    {

        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();

        retry.onClick.AddListener(() =>
        {
            main.FirebaseInit();
            main.dm.CloseCurrentDialog();
        });
    }
}
