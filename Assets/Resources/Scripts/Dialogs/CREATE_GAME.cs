using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CREATE_GAME : MonoBehaviour
{
    private GameObject mainCanvas;
    private MainScript main;

    public GameObject dialogBox;
    public Button button_cancel;
    public Button button_create;
    public InputField edittext_name;

    // Start is called before the first frame update
    void Start()
    {
        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();


        SetValues();

        SetListeners();

    }

    void SetValues()
	{
        edittext_name.text = main.playerName;
	}

   

    void SetListeners()
    {
        button_cancel.onClick.AddListener(() => main.CloseCurrentDialog());
        button_create.onClick.AddListener(() =>
        {
            if (edittext_name.text.ToString().Trim() != "") main.OnCreateGame(edittext_name.text.ToString().Trim());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
