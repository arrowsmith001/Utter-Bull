using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CONFIRM_TRUTH : MonoBehaviour
{
    private GameObject mainCanvas;
    private MainScript main;

    public Button Button_Confirm_True;



    // Start is called before the first frame update
    void Start()
    {

        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();
        SetListeners();
    }

    void SetListeners()
    {
        Button_Confirm_True.onClick.AddListener(() =>
        {
            main.CloseCurrentDialog();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
