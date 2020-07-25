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

    public Text whiteBoxText;

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

        SetText();
    }

    private void SetText()
    {
        int TITLE_SIZE = 75;
        int NAME_SIZE = 100;

        string text = "";
        text += "<size="+TITLE_SIZE+">Game design and development</size>";
        text += "\n" + "<size="+NAME_SIZE+ "><b>Alex Arrowsmith</b></size>";
        text += "\n\n" + "<size=" + TITLE_SIZE + ">Sound design</size>";
        text += "\n" + "<size="+NAME_SIZE+ "><b>Sam Yately</b></size>";

        if(main.credits != null && main.credits.Count != 0)
        {
            text += "\n\n" + "<size="+TITLE_SIZE+ ">Special thanks</size>";
            foreach (string name in main.credits)
            {
                text += "\n" + "<size=" + NAME_SIZE + "><b>" + name + "</b></size>";
            }
        }

        whiteBoxText.text = text;
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
