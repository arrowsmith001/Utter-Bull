using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JOIN_GAME : MonoBehaviour
{
    private GameObject mainCanvas;
    private MainScript main;

    public GameObject dialogBox;
    public Button button_cancel;
    public Button button_join;
    public InputField edittext_name;
    public InputField edittext_code;

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
        if (main.debugModeOn) edittext_code.text = "ABC99";
        edittext_name.text = main.playerName;
    }

    void SetListeners()
    {
        button_cancel.onClick.AddListener(() => main.CloseCurrentDialog());
        button_join.onClick.AddListener(() =>
        {
            if (edittext_name.text.ToString().Trim() != ""
            && edittext_code.text.ToString().Trim().Length == 5)
            {
                main.OnJoinGame(edittext_name.text.ToString().Trim(), edittext_code.text.ToString().Trim());
            }
        });

        edittext_code.onValueChanged.AddListener((text) =>
        {
            edittext_code.text = edittext_code.text.ToUpper();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
