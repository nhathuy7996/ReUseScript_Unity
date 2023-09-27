using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixIphoneX : MonoBehaviour
{
    [SerializeField]
    bool _FixBottom, _FixTop = true, _FixBottom_withScale = false;
    [SerializeField]
    float _Bottom = 0;

    // Start is called before the first frame updated
    void Start()
    {
        if (_FixTop)
            this.GetComponent<RectTransform>().offsetMax = new Vector2(this.GetComponent<RectTransform>().offsetMax.x, -Screen.safeArea.y);

        if (_FixBottom_withScale)
        {
            this.GetComponent<RectTransform>().offsetMin = new Vector2(this.GetComponent<RectTransform>().offsetMin.x, Screen.height - Screen.safeArea.height);
        }

        if (_FixBottom)
            this.GetComponent<RectTransform>().offsetMin = new Vector2(this.GetComponent<RectTransform>().offsetMin.x, 0);



    }

    // Update is called once per frame
    void Update()
    {

        if (AdManager.Instant.Banner_active)
        {
            this.GetComponent<RectTransform>().offsetMax = new Vector2(this.GetComponent<RectTransform>().offsetMax.x, -adHeight());
            if (_FixBottom)
                this.GetComponent<RectTransform>().offsetMin = new Vector2(this.GetComponent<RectTransform>().offsetMin.x, 0);
        }
    }

    public static float adHeight()
    {
        float f = Screen.dpi / 160f;
        float dp = Screen.height / f;
        return (dp > 720f) ? 90f * f
            : (dp > 400f) ? 50f * f
            : 32f * f;
    }
}
