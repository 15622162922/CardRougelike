using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class TestSceneView : EditorWindow
{
    private static TestSceneView m_window;
    private Scene m_previewScene;
    private Canvas m_previewCanvas;
    private Camera m_previewCamera;
    private RenderTexture m_previewTexture;
    private GameObject m_previewInstance;

    private float zoom = 1.0f;
    private Vector2 drag;
    
    [MenuItem("Test/TestSceneView")]
    public static void ShowWindow(){
        m_window = EditorWindow.GetWindow<TestSceneView>();
        m_window.titleContent = new GUIContent("测试场景预览窗口");
        m_window.Show();
    }

    void OnEnable()
    {
        //创建一个预览场景
        m_previewScene = EditorSceneManager.NewPreviewScene();

        LoadLighting();
        CreateCanvas();
        LoadPrefab();
        LoadCamera();
    }

    void OnDisable()
    {
        if (m_previewScene != null)
        {
            if (m_previewCanvas != null)
            {
                DestroyImmediate(m_previewCanvas);
                m_previewCanvas = null;
            }

            if (m_previewCamera != null)
            {
                DestroyImmediate(m_previewCamera);
                m_previewCamera = null;
            }
            
            if (m_previewTexture != null)
            {
                m_previewTexture.Release();
                DestroyImmediate(m_previewTexture);
            }
            
            EditorSceneManager.ClosePreviewScene(m_previewScene);
            m_previewScene = default(Scene);
        }
    }

    // 创建Canvas
    void CreateCanvas()
    {
        var canvasObj = new GameObject("Canvas");
        m_previewCanvas = canvasObj.AddComponent<Canvas>();
        m_previewCanvas.renderMode = RenderMode.WorldSpace;
        m_previewCanvas.transform.localPosition = Vector3.zero;
        m_previewCanvas.transform.localRotation = Quaternion.identity;
        m_previewCanvas.transform.localScale = Vector3.one;
        EditorSceneManager.MoveGameObjectToScene(canvasObj, m_previewScene);
    }
    
    // 读取UI
    void LoadPrefab()
    {
        var uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/RawResources/UI/Prefab/Test/UITestView.prefab");
        if (m_previewCanvas != null)
        {
            m_previewInstance = GameObject.Instantiate(uiPrefab, m_previewCanvas.transform);
            m_previewInstance.transform.localPosition = Vector3.zero;
            Debug.Log("成功加载uiPrefab");
        }
    }

    // 创建摄像机
    void LoadCamera()
    {
        // 创建相机
        var camGO = new GameObject("PreviewCamera");
        m_previewCamera = camGO.AddComponent<Camera>();
        m_previewCamera.clearFlags = CameraClearFlags.SolidColor;
        m_previewCamera.backgroundColor = Color.black;
        m_previewCamera.orthographic = true;
        m_previewCamera.orthographicSize = 500;
        m_previewCamera.transform.position = new Vector3(0, 0, -10);
        m_previewCamera.cullingMask = ~0;
        SceneManager.MoveGameObjectToScene(camGO, m_previewScene);
        m_previewCamera.scene = m_previewScene;

        m_previewTexture = new RenderTexture(1334, 1334, 100);
        m_previewCamera.targetTexture = m_previewTexture;
    }

    void LoadLighting()
    {
        var lightingObj = new GameObject("Lighting");
        lightingObj.AddComponent<Light>();
        SceneManager.MoveGameObjectToScene(lightingObj, m_previewScene);
    }

    private void OnGUI()
    {
        Rect previewRect = new Rect(0, 10, position.width, position.height);
        
        HandleInput();
        // 摄像机位置和缩放
        m_previewCamera.orthographicSize = 500 / zoom;
        m_previewCamera.transform.position = new Vector3(drag.x, drag.y, -1000);
        m_previewCamera.transform.rotation = Quaternion.identity;
        
        m_previewCamera.Render();
        if (Event.current.type == EventType.Repaint)
        {
            Handles.DrawCamera(previewRect, m_previewCamera, DrawCameraMode.Normal);
        }
    }
    
    // 输入处理
    private void HandleInput()
    {
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel)
        {
            zoom += -e.delta.y * 0.01f;
            zoom = Mathf.Clamp(zoom, 0.1f, 10f);
            Debug.Log($"触发滚轮：缩放为：{zoom}");
            e.Use();
        }
        else if (e.type == EventType.MouseDrag && e.button == 0)
        {
            drag.x -= e.delta.x * (1 / zoom);
            drag.y += e.delta.y * (1 / zoom);
            Debug.Log($"触发拖拽，方向为：{drag.x}, {drag.y}");
            e.Use();
        }
        else if (e.type == EventType.MouseDown && e.button == 0)
        {
            // 点击选中逻辑（射线检测）
            Ray ray = m_previewCamera.ScreenPointToRay(new Vector3(e.mousePosition.x, position.height - e.mousePosition.y));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Selection.activeGameObject = hit.collider.gameObject;
            }
            e.Use();
        }
    }
}
