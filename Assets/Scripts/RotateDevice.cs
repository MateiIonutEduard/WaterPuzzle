using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateDevice : MonoBehaviour
{
    private RectTransform t;

    public void Start()
    {
        t = GetComponent<RectTransform>();
        init();
    }

    public void FixedUpdate()
    {
        init();
    }

    public void init()
    {
        var orient = (ScreenOrientation)Input.deviceOrientation;

        if (orient == ScreenOrientation.Portrait || orient == ScreenOrientation.PortraitUpsideDown)
        {
            // portrait
            t.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            // landspace
            t.localScale = new Vector3(1.5f, 1.5f, 1f);
        }

    }
}
