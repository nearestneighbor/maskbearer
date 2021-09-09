using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class CameraPipeline : MonoBehaviour
{
    [SerializeField] private bool m_Debug;

    [SerializeField] private List<PipelineTexture> m_Textures = new List<PipelineTexture>();
    [SerializeField] private List<PipelineCamera> m_Cameras = new List<PipelineCamera>();
    [SerializeField] private List<PipelineImage> m_Images = new List<PipelineImage>();

    private List<IDisposable> m_Disposable = new List<IDisposable>();
    private GameObject m_CamerasRoot;
    private GameObject m_ImagesRoot;

    private Camera m_MasterCamera;
    private int m_MasterLayer = -1;
    private bool m_Invalidated;
    private Vector2Int m_ScreenSize;

    private void OnValidate() => m_Invalidated = true;
    
    private void OnEnable()
    {
        m_MasterLayer = gameObject.layer = GetMasterLayer(gameObject.layer);
        m_MasterCamera = GetComponent<Camera>();
        m_MasterCamera.cullingMask = 1 << m_MasterLayer;
        m_Invalidated = true;
    }

    private void OnDisable()
    {
        Clear();
        m_MasterCamera.cullingMask = ~0;
        m_MasterCamera = null;
    }

    private void Update()
    {
        if (m_Invalidated || m_ScreenSize.x != Screen.width || m_ScreenSize.y != Screen.height)
        {
            m_Invalidated = false;
            m_ScreenSize = new Vector2Int(Screen.width, Screen.height);
            Clear();
            Create();
        }
        else
        {
            UpdateMasterCameraProperties();
        }
    }

    private void Clear()
    {
        foreach (var item in m_Disposable)
            item.Dispose();

        m_Disposable.Clear();

        Destroy(m_CamerasRoot);
        Destroy(m_ImagesRoot);
    }

    private void Create()
    {
        // Containers

        var hideFlags = m_Debug
            ? HideFlags.DontSave | HideFlags.NotEditable
            : HideFlags.HideAndDontSave;

        m_CamerasRoot = new GameObject("Cameras");
        m_CamerasRoot.transform.SetParent(transform, false);
        m_CamerasRoot.hideFlags = hideFlags;

        m_ImagesRoot = new GameObject("Canvas");
        m_ImagesRoot.transform.SetParent(transform, false);
        m_ImagesRoot.hideFlags = hideFlags;
        m_ImagesRoot.layer = m_MasterLayer;

        var canvas = m_ImagesRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = m_MasterCamera;
        canvas.planeDistance = m_MasterCamera.nearClipPlane;

        // Parts

        for (var i = 0; i < m_Textures.Count; i++)
        {
            m_Textures[i].Create(i);
        }

        for (var i = 0; i < m_Cameras.Count; i++)
        {
            m_Cameras[i].Create(
                m_MasterCamera,
                m_CamerasRoot,
                GetRenderTextureByName(m_Cameras[i].TextureName),
                i,
                m_Cameras.Count
            );
        }

        for (var i = 0; i < m_Images.Count; i++)
        {
            m_Images[i].Create(
                GetRenderTextureByName(m_Images[i].TextureName),
                m_ImagesRoot
            );
        }

        // Copy lists to disposable

        m_Disposable.AddRange(m_Images);
        m_Disposable.AddRange(m_Cameras);
        m_Disposable.AddRange(m_Textures);
    }

    private void UpdateMasterCameraProperties()
    {
        foreach (var camera in m_Cameras)
            camera.CopyPropertiesFromMaster();

        if (m_MasterLayer != gameObject.layer)
        {
            m_MasterLayer = gameObject.layer = GetMasterLayer(gameObject.layer);
            m_MasterCamera.cullingMask = 1 << m_MasterLayer;
            m_ImagesRoot.layer = m_MasterLayer;
        }
    }

    private RenderTexture GetRenderTextureByName(string name)
    {
        foreach (var target in m_Textures)
            if (target.Name == name)
                return target.Texture;
            
        Debug.LogWarning($"RenderTexture '{name}' isn't defined");
        return null;
    }

    private static int GetMasterLayer(int layer)
    {
        if (layer != 0)
            return layer;

        for (var layerIndex = 1; layerIndex < 32; layerIndex++)
        {
            var layerName = LayerMask.LayerToName(layerIndex);
            if (layerName == null || layerName == string.Empty)
            {
                Debug.LogWarning($"Use layer #{layerIndex}, setup layer explicitly to avoid this message");
                return layerIndex;
            }
        }

        // Use built-in 'Ignore Raycast'
        const int fallbackLayer = 2;
        Debug.LogError($"Use built-in layer '{LayerMask.LayerToName(fallbackLayer)}' because there isn't any free Layer");
        return fallbackLayer;
    }

    private static new void Destroy(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            if (Application.isPlaying)
                GameObject.Destroy(obj);
            else
                GameObject.DestroyImmediate(obj);
        }
    }
    
    //
    // Internal classes
    //

    [Serializable]
    private class PipelineTexture : IDisposable
    {
        public RenderTexture Texture { get; private set; }
        public string Name => m_Name;

        [SerializeField] private string m_Name;
        [SerializeField] private RenderTextureFormat m_Format = RenderTextureFormat.Default;
        [SerializeField] [Range(1, 32)] private int m_Scale = 1;
        [SerializeField] [Range(0, 32)] private int m_Depth = 0;
        [SerializeField] [Range(1, 10)]  private int m_MipMaps = 0;

        public void Create(int index)
        {
            //
            // Validate
            //
            m_Scale = Mathf.Clamp(m_Scale, 1, 32);
            m_MipMaps = Mathf.Max(m_MipMaps, 1);

            /**/ if (m_Depth < 16) m_Depth = 0;
            else if (m_Depth < 24) m_Depth = 16;
            else if (m_Depth < 32) m_Depth = 24;

            if (m_Name == null || m_Name == string.Empty)
                m_Name = "RT" + index;

            //
            // Create
            //
            Texture = new RenderTexture(
                Screen.width / m_Scale,
                Screen.height / m_Scale,
                m_Depth,
                m_Format,
                m_MipMaps
            );

            Texture.useMipMap = m_MipMaps > 1;
            Texture.name = $"{m_Name}-{Texture.format}-{Texture.width}x{Texture.height}";
            Texture.Create();

            Shader.SetGlobalTexture($"_Pipeline{m_Name}Tex", Texture);
        }

        public void Dispose()
        {
            CameraPipeline.Destroy(Texture);
        }
    }

    [Serializable]
    private class PipelineCamera : IDisposable
    {
        public Camera Camera { get; private set; }
        public string TextureName => m_TextureName;

        private enum PipelineCameraRaycaster { None, Raycaster, Raycaster2D }

        [SerializeField] private string m_TextureName;
        [SerializeField] private LayerMask m_Culling;
        [SerializeField] private float m_ClippingFrom = 0;
        [SerializeField] private float m_ClippingTo = 1000;
        [SerializeField] private bool m_ColorClear = true;
        [SerializeField] private Color m_Color;
        [SerializeField] private PipelineCameraRaycaster m_Raycaster;

        [SerializeField] private Canvas[] m_Canvases;

        private Camera m_Master;
        private int m_MasterDepthOffset;

        public void Create(Camera master, GameObject parent, RenderTexture target, int index, int count)
        {
            //
            // Validate
            //
            m_ClippingFrom = Mathf.Min(m_ClippingFrom, m_ClippingTo);
            m_Culling &= ~master.cullingMask;

            //
            // Create
            //
            var cameraName = "Camera_" + (index+1);
            if (target != null)
                cameraName += " (" + target.name + ")";

            var cameraObject = new GameObject(cameraName);
            cameraObject.transform.SetParent(parent.transform, false);
            cameraObject.hideFlags = parent.hideFlags;
            Camera = cameraObject.AddComponent<Camera>();

            // Create raycaster
            if (m_Raycaster != PipelineCameraRaycaster.None)
            {
                var raycaster = m_Raycaster == PipelineCameraRaycaster.Raycaster
                    ? cameraObject.AddComponent<PhysicsRaycaster>()
                    : cameraObject.AddComponent<Physics2DRaycaster>();

                raycaster.eventMask = m_Culling;
            }

            // Setup properties
            Camera.targetTexture = target;

            Camera.cullingMask = m_Culling;
            Camera.nearClipPlane = m_ClippingFrom;
            Camera.farClipPlane = m_ClippingTo;

            Camera.clearFlags = m_ColorClear ? CameraClearFlags.SolidColor : CameraClearFlags.Nothing;
            Camera.backgroundColor = m_Color;

            // Copy properties from master
            m_Master = master;
            m_MasterDepthOffset = index - count;
            CopyPropertiesFromMaster();

            // Setup canvases
            if (m_Canvases != null)
                foreach (var canvas in m_Canvases)
                    if (canvas != null)
                            canvas.worldCamera = Camera;
        }

        public void CopyPropertiesFromMaster()
        {
            Camera.depth = m_Master.depth + m_MasterDepthOffset;
            Camera.orthographic = m_Master.orthographic;
            Camera.orthographicSize = m_Master.orthographicSize;

            Camera.allowHDR = m_Master.allowHDR;
            Camera.allowMSAA = m_Master.allowMSAA;
            Camera.allowDynamicResolution = m_Master.allowDynamicResolution;
        }

        public void Dispose()
        {
            if (Camera != null)
            {
                CameraPipeline.Destroy(Camera.gameObject);

                if (m_Canvases != null)
                    foreach (var canvas in m_Canvases)
                        if (canvas != null)
                            canvas.worldCamera = m_Master;
            }
        }
    }

    [Serializable]
    private class PipelineImage : IDisposable
    {
        public string TextureName => m_TextureName;

        [SerializeField] private string m_TextureName;
        [SerializeField] private Material m_Material;

        private RawImage m_RawImage;

        public void Create(RenderTexture texture, GameObject parent)
        {
            var gameObject = new GameObject("RawImage_" + parent.transform.childCount);
            gameObject.transform.SetParent(parent.transform, false);
            gameObject.hideFlags = parent.hideFlags;

            m_RawImage = gameObject.AddComponent<RawImage>();
            
            m_RawImage.rectTransform.anchorMin = Vector2.zero;
            m_RawImage.rectTransform.anchorMax = Vector2.one;
            m_RawImage.rectTransform.sizeDelta = Vector2.zero;

            m_RawImage.texture = texture;
            m_RawImage.material = m_Material;
            m_RawImage.raycastTarget = false;
            m_RawImage.maskable = false;
        }

        public void Dispose()
        {
            if (m_RawImage != null)
            {
                CameraPipeline.Destroy(m_RawImage.gameObject);
            }
        }
    }
}