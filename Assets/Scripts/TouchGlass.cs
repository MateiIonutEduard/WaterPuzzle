using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TouchGlass : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData e)
    {
        var glass = GetComponent<Glass>();
        var loader = FindObjectOfType<Loader>();
        loader.SetSource(glass);
    }
}
