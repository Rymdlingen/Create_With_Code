﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelArtCanvas : CanvasScaler
{
    private Vector2 previousScreenSize;
    private RenderTexture renderTexture;

    protected override void Start()
    {
        base.Start();
    }

    protected override void HandleConstantPixelSize()
    {
        var currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize == previousScreenSize) return;
        previousScreenSize = currentScreenSize;

        Recalculate();
    }

    public void Recalculate()
    {
        var currentScreenSize = new Vector2(Screen.width, Screen.height);

        // We need to be able to display at least 320 x 180 screen (16:9).
        var safeAreaSize = new Vector2(260, 150);

        var safeAreaWorldHeight = safeAreaSize.y * 3f;

        // Find the biggest scale that still shows the whole safe area.
        int scale = 1;

        while (Vector2.Max(safeAreaSize * (scale + 1), currentScreenSize) == currentScreenSize) scale++;
        // Debug.Log($"Display scale is {scale}.");

        // Clean up the previous render texture.
        if (renderTexture) renderTexture.Release();

        // Create the new render texture.
        Vector2Int renderTargetSize = new Vector2Int
        {
            x = Mathf.CeilToInt(currentScreenSize.x / scale / 2) * 2,
            y = Mathf.CeilToInt(currentScreenSize.y / scale / 2) * 2
        };
        // Debug.Log("Rendering to target size:");
        // Debug.Log(renderTargetSize);

        RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(renderTargetSize.x, renderTargetSize.y, RenderTextureFormat.Default, 24);
        renderTextureDescriptor.sRGB = true;
        renderTextureDescriptor.autoGenerateMips = false;
        renderTextureDescriptor.useMipMap = false;

        renderTexture = new RenderTexture(renderTextureDescriptor);
        renderTexture.filterMode = FilterMode.Point;

        // Set render target on scene cameras.
        Camera sceneCamera = GameObject.Find("Scene Camera").GetComponent<Camera>();
        Camera zoomCamera = GameObject.Find("Zoom Camera").GetComponent<Camera>();

        if (sceneCamera)
        {
            sceneCamera.targetTexture = renderTexture;
            // Change camera settings.
            sceneCamera.orthographicSize = safeAreaWorldHeight / safeAreaSize.y * renderTargetSize.y / 2;
        }

        if (zoomCamera)
        {
            zoomCamera.targetTexture = renderTexture;
            // Change camera settings.
            zoomCamera.orthographicSize = safeAreaWorldHeight / safeAreaSize.y * renderTargetSize.y / 6;
        }



        // Resize the scene display and apply new render texture.
        GameObject sceneDisplay = GameObject.Find("Scene Display");
        sceneDisplay.transform.localScale = new Vector3(renderTargetSize.x, renderTargetSize.y, 1);
        sceneDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = renderTexture;

        // Resize the display camera viewport.
        Camera displayCamera = GameObject.Find("Display Camera").GetComponent<Camera>();
        displayCamera.orthographicSize = (float)Screen.height / scale / 2;

        // Resize the canvas scale.
        this.scaleFactor = scale;
    }
}
