using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameHolder : MonoBehaviour
{
    public bool active;
    public string playerName;

    public InputField input;
    public Button submit;
    public Button prefs;

    public GameObject thisPanel;
    public MainScript main;

    // Start is called before the first frame update
    void Start()
    {

        if(active)
        {
            submit.gameObject.SetActive(true);
            prefs.gameObject.SetActive(true);
            input.gameObject.SetActive(true);

            submit.onClick.AddListener(() =>
            {
                if (input.text != "")
                {
                    playerName = input.text;

                    main.enabled = true;
                    thisPanel.SetActive(false);
                }
            });

            prefs.onClick.AddListener(() =>
            {
                PlayerPrefs.DeleteAll();
            });
        }
        else
        {
            main.enabled = true;
            thisPanel.SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (input.text != "")
            {
                playerName = input.text;

                main.enabled = true;
                thisPanel.SetActive(false);
            }
        }

    }
}
