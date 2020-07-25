
using UnityEngine;
using UnityEngine.UI;

public class CLICK : MonoBehaviour
{
    public Image img;
    public float time;

    private float t;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("CLICK started");

        t = time;

        try
        {
            LeanTween.scale(img.gameObject, new Vector3(1, 1, 1), time).setFrom(new Vector3(0.5f, 0.5f, 0.5f))
                .setOnComplete(() => { try{Destroy(); } catch { } });

        }catch { Destroy(); }
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;

        if (t < 0) Destroy();
    }
}
