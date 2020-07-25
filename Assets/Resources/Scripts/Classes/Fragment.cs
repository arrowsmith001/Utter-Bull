using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using Firebase.Unity.Editor;
using Newtonsoft.Json;

public abstract class Fragment : MonoBehaviour
{
    public MainScript main;
    public GameObject mainCanvas;

    public virtual string GetID()
    {
        throw new Exception("NO ID SET");
    }

    /// <summary>
    /// Calls GetMain()
    /// </summary>
    public virtual void Awake()
    {
        GetMain();
    }

    /// <summary>
    /// Calls Initialise() then RegisterEventListeners()
    /// </summary>
    public virtual void Start()
    {
        Initialise();
        RegisterEventListeners();
    }

    /// <summary>
    /// Calls UnregisterEventListeners()
    /// </summary>
    public virtual void OnDestroy()
    {
        UnregisterEventListeners();
    }

    public void GetMain()
    {
        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();
    }

    public abstract void Initialise();

    public abstract void RegisterEventListeners();

    public abstract void UnregisterEventListeners();

    public abstract void RoomChangeResult(string code, Task task, bool succes, object args);

    public void SetTransformPos(Component cmp, string posName, PositionsHolder ph)
    {
        cmp.gameObject.GetComponent<RectTransform>().localPosition = ph.positions[posName].GetComponent<RectTransform>().localPosition;
        cmp.gameObject.GetComponent<RectTransform>().anchorMin = ph.positions[posName].GetComponent<RectTransform>().anchorMin;
        cmp.gameObject.GetComponent<RectTransform>().anchorMax = ph.positions[posName].GetComponent<RectTransform>().anchorMax;
    }

    public void Tween(Component cmp, string endPosName, float duration, PositionsHolder ph,
        float delay, Interpolator interp, bool ActivenessOnStart, bool ActivenessOnEnd,
        Action endAction)
    {
        try
        {
            GameObject go = cmp.gameObject;

            if (interp == null)
            {
                interp = new LinearInterpolator();
            }

            Vector2 startSize = new Vector2();
            Vector2 endSize = new Vector2();

            RectTransform startRect;
            RectTransform endRect;

            startRect = go.GetComponent<RectTransform>();
            endRect = ph.positions[endPosName].GetComponent<RectTransform>();


            Vector3 startPos = startRect.localPosition;
            Vector3 endPos = endRect.localPosition;

            Vector2 startMin = startRect.anchorMin;
            Vector2 endMin = endRect.anchorMin;
            Vector2 startMax = startRect.anchorMax;
            Vector2 endMax = endRect.anchorMax;

            LeanTween.value(0, 1, duration).setDelay(delay)
                .setOnUpdate((float t) =>
                {
                    try
                    {
                        if (go != null)
                        {

                            go.GetComponent<RectTransform>().localPosition = interp.getVec3Interpolation(startPos, endPos, t);
                            go.GetComponent<RectTransform>().anchorMin = interp.getVec2Interpolation(startMin, endMin, t);
                            go.GetComponent<RectTransform>().anchorMax = interp.getVec2Interpolation(startMax, endMax, t);

                        }
                    }
                    catch { }
                })
                .setOnStart(() =>
                {

                    go.GetComponent<RectTransform>().localPosition = startPos;
                    go.GetComponent<RectTransform>().anchorMin = startMin;
                    go.GetComponent<RectTransform>().anchorMax = startMax;
                })
                .setOnComplete(() =>
                {

                    go.GetComponent<RectTransform>().localPosition = endPos;
                    go.GetComponent<RectTransform>().anchorMin = endMin;
                    go.GetComponent<RectTransform>().anchorMax = endMax;
                });

        }
        catch (Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

    }

    public void Resize(Component cmp, float startScale, float endScale, float duration, float delay, Interpolator interp, bool ActivenessOnStart, bool ActivenessOnEnd,
        Action endAction)
    {
        try
        {
            GameObject go = cmp.gameObject;

            if (interp == null)
            {
                interp = new LinearInterpolator();
            }

            RectTransform rect;

            rect = go.GetComponent<RectTransform>();


            Vector2 startMin = new Vector2(0.5f - 0.5f * (startScale), 0.5f - 0.5f * (startScale));
            Vector2 endMin = new Vector2(0.5f - 0.5f * (endScale), 0.5f - 0.5f * (endScale));
            Vector2 startMax = new Vector2(0.5f + 0.5f * (startScale), 0.5f + 0.5f * (startScale));
            Vector2 endMax = new Vector2(0.5f + 0.5f * (endScale), 0.5f + 0.5f * (endScale));

            LeanTween.value(0, 1, duration).setDelay(delay)
                .setOnUpdate((float t) =>
                {
                    try
                    {
                        if (go != null)
                        {
                            go.GetComponent<RectTransform>().anchorMin = interp.getVec2Interpolation(startMin, endMin, t);
                            go.GetComponent<RectTransform>().anchorMax = interp.getVec2Interpolation(startMax, endMax, t);

                        }
                    }
                    catch { }
                })
                .setOnStart(() =>
                {
                    go.GetComponent<RectTransform>().anchorMin = startMin;
                    go.GetComponent<RectTransform>().anchorMax = startMax;
                })
                .setOnComplete(() =>
                {
                    go.GetComponent<RectTransform>().anchorMin = endMin;
                    go.GetComponent<RectTransform>().anchorMax = endMax;
                });

        }
        catch (Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

    }
}
