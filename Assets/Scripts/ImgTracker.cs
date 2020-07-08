using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImgTracker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The camera to set on the world space UI canvas far each instantiated image info.")]
    Camera m_WorldSpaceCanvasCamera;

    public Camera worldSpaceCanvasCamera
    {
        get { return m_WorldSpaceCanvasCamera; }
        set { m_WorldSpaceCanvasCamera = value; }
    }


    [SerializeField]
    [Tooltip("If an image is detected but no source texture can be found, this texture will be used instead.")]
    Texture2D m_DefaultTexture;

    public Texture2D defaultTexture
    {
        get { return m_DefaultTexture; }
        set { m_DefaultTexture = value; }
    }

    ARTrackedImageManager m_TrackedImageMgr;

    void Awake()
    {
        m_TrackedImageMgr = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageMgr.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageMgr.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void UpdateInfo(ARTrackedImage trackedImage)
    {
        // Set canvas camera
        var canvas = trackedImage.GetComponentInChildren<Canvas>();
        canvas.worldCamera = worldSpaceCanvasCamera;

        // Update info about the tracked image
        var text = canvas.GetComponentInChildren<Text>();
        text.text = string.Format(
            "{0}\ntrackingState: {1}\nGUID: {2}\nReference size: {3}\nDetected size: {4} cm",
            trackedImage.referenceImage,
            trackedImage.trackingState,
            trackedImage.referenceImage.guid,
            trackedImage.referenceImage.size * 100f,
            trackedImage.size * 100f);

        var planeParentGO = trackedImage.transform.GetChild(0).gameObject;
        var planeGO = planeParentGO.transform.GetChild(0).gameObject;

        // Disable the visual plane if it is not being tracked
        if (trackedImage.trackingState != TrackingState.None)
        {
            planeGO.SetActive(true);

            // The image extents is only valid when the image is being tracked
            trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);

            // Set the texture
            var material = planeGO.GetComponentInChildren<MeshRenderer>().material;
            material.mainTexture = (trackedImage.referenceImage.texture == null) ?
                                    defaultTexture : trackedImage.referenceImage.texture;
        }
        else
        {
            planeGO.SetActive(false);
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateInfo(trackedImage);
        }
    }
}
