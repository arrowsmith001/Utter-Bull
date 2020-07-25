using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RESUME_GAME : MonoBehaviour
{
    GameObject mainCanvas;
    MainScript main;

    public Text resumeText;
    public Text playersText;
    public Image img;
    public Button btn_resume;
    public Button btn_quit;

    GameChecker gcheck;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("RESUME_GAME CALLED");

        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();

        this.gcheck = main.gameObject.GetComponent<GameChecker>();

        // Set text
        resumeText.text = gcheck.resumeText;
        playersText.text = gcheck.playersText;

        // Set image (if exists)
        if (gcheck.phaseImage != null)
        {
            img.gameObject.SetActive(true);
            img.sprite = gcheck.phaseImage;
        }
        // Set buttons
        btn_resume.onClick.AddListener(() =>
        {
            main.OnGameResume(true);
            main.dm.CloseCurrentDialog();
        });
        btn_quit.onClick.AddListener(() =>
        {
            main.OnGameResume(false);
            main.dm.CloseCurrentDialog();
        });
    }

}
