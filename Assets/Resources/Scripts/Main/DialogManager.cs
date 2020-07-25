using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    // Public
    public GameObject dialogPanel; // Parent to-be
    public LeanTweenType easeType;

    public GameObject activePosition;
    public GameObject popEntryScale;
    public GameObject popFinalScale;

    public float animSpeed;

    public Interpolator interp;
    public float interpFactor;

    private bool isTransitioning;
    private bool isAnimating;

    private GameObject newDialog;
    private GameObject newDialogBox;

    // Private
    private Dictionary<string, Transform> scale;
    private float t;

    // Start is called before the first frame update
    void Start()
    {
        isTransitioning = false;
        isAnimating = false;

        interp = new OvershootInterpolator(2);
        interp.factor = interpFactor;

        scale = new Dictionary<string, Transform>();
        scale.Add("ENTRY", popEntryScale.transform);
        scale.Add("FINAL", popFinalScale.transform);
    }

    public void RequestDialog(string fragName)
    {

        //if (!isTransitioning && dialogPanel.transform.childCount <= 1)
            ShowDialog(fragName);
    }

    void ShowDialog(string fragName)
    {
        isTransitioning = true;

        // Load in new fragment
        newDialog = Resources.Load<GameObject>("Prefabs/Dialogs/" + fragName);

        // Instantiate above fragment
        Instantiate(newDialog, activePosition.transform.position, Quaternion.identity, dialogPanel.transform);

        // Assign newFrag to newly instantiated frag
        newDialog = dialogPanel.transform.GetChild(0).gameObject;
        newDialogBox = newDialog.transform.Find("DIALOG_BOX").gameObject;

        // Animate
        // newDialogBox.transform.localScale = scale["ENTRY"];
        // LeanTween.scale(newDialogBox, scale["FINAL"], 0.35f).setEase(easeType).setFrom(scale["ENTRY"]);

        LeanTween.value(0, 1, 0.35f).setOnUpdate((float t) =>
        {
          try{  newDialogBox.transform.localScale = interp.getVec3Interpolation(
                scale["ENTRY"].transform.localScale, scale["FINAL"].transform.localScale, t); } catch { }
        })
            .setOnStart(() =>
            {
                DestroyDialogs(false);
            })
            ;

        // On proper use, this will prompt current fragment to quit listeners etc
        // OnCurrentFragmentStop();
    }

    // Callback method for when the current fragment has completed its "stop" routine and transition can commence
    public void OnCurrentFragmentStop()
    {
        t = 0;
        isAnimating = true;
    }


    public void CloseCurrentDialog()
	{
            DestroyDialogs(true);
        if(newDialog != null)
		{
            newDialog = null;
		}
	}

    private void DestroyDialogs(bool v)
    {
        int n;

        if(v)
        {
            n = dialogPanel.transform.childCount;
        }
        else
        {
            n = dialogPanel.transform.childCount - 1;
        }

        for (int i = 0; i < n; i++)
        {
            Destroy(dialogPanel.transform.GetChild(i).gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
       

    }
}