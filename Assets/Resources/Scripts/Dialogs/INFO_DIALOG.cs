using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class INFO_DIALOG : MonoBehaviour
{
    private GameObject mainCanvas;
    private MainScript main;

    public GameObject dialogBox;

    public Button button_shade;
    public Button button_back;

    public Button button_TL;
    public Button button_TR;
    public Button button_BR;
    public Button button_BL;

    static int TL = 0;
    static int TR = 1;
    static int BR = 2;
    static int BL = 3;

    public int state = 0;
    public int i = 0;

    public GameObject button_panel;

    // Start is called before the first frame update
    void Start()
    {
        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();

        //button_panel.SetActive(main.isMember != MembershipChecker.MEMBER_TRUE);
        button_panel.SetActive(false);

        button_shade.onClick.AddListener(() =>
        {
            main.CloseCurrentDialog();
        });
        button_back.onClick.AddListener(() =>
        {
            main.CloseCurrentDialog();
        });

        // SetListeners();
    }

    void SetListeners()
    {
        button_TL.onClick.AddListener(() =>
        {
            OnButton(TL);
        });
        button_TR.onClick.AddListener(() =>
        {
            OnButton(TR);
        });
        button_BR.onClick.AddListener(() =>
        {
            OnButton(BR);
        });
        button_BL.onClick.AddListener(() =>
        {
            OnButton(BL);
        });
    }

    private void OnButton(int button_id)
    {
        if(button_id == state)
        {
            state = (state + 1) % 4;
            i++;

            if(i == 24)
            {
                OnReached();
            }
        }
        else
        {
            state = 0;
            i = 0;
        }
    }

    private void OnReached()
    {
        main.OnRemoveAdsPressed();
        main.CloseCurrentDialog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
