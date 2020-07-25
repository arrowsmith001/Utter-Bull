using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QUIT_GAME : MonoBehaviour
{
    private GameObject mainCanvas;
    private MainScript main;

    public GameObject dialogBox;

    public Button button_cancel;
    public Button button_quit;

    //public Text text_host;

    // Start is called before the first frame update
    void Start()
    {
        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();

        //text_host.gameObject.SetActive(false);
        //text_host.gameObject.SetActive(main.rl.playersLookup[main.playerName].isHosting);

        button_cancel.onClick.AddListener(() =>
        {
            main.CloseCurrentDialog();
        });

        button_quit.onClick.AddListener(() =>
        {
            main.rc.ChangePhase("Lobby", null);
            main.CloseCurrentDialog();
        });

        //InitialiseViews();

        //SetValues();

        //SetListeners();

    }

    // Update is called once per frame
    void Update()
    {

    }
}