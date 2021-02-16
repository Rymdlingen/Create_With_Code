using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PixelArtGraphicRaycaster : GraphicRaycaster
{
    private PixelArtCanvas pixelArtCanvas;

    protected override void Start()
    {
        base.Start();

        pixelArtCanvas = GetComponent<PixelArtCanvas>();
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        eventData.position /= pixelArtCanvas.scaleFactor;
        base.Raycast(eventData, resultAppendList);
    }
}