using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class DrawCallToolsWindow : EditorWindow
{
    public static DrawCallToolsWindow drawCallTools;
    Camera mainCamera;

    [MenuItem("Tools/DrawCallTools")]
    public static void ShowWindow()
    {
        drawCallTools = GetWindow<DrawCallToolsWindow>("DrawCallTools");
    }

    void InitFrameInfo()
    {
        // Camera.onPreCull += CollectDrawCalls;
        mainCamera = Camera.main;
    }

    private void OnGUI() 
    {
        
    }

}
