using UnityEngine;
using UnityEngine.UI;

public class MirrorManager : MonoBehaviour
{
    [Header("Mirror Cameras")]
    public Camera leftMirrorCamera;
    public Camera centerMirrorCamera;
    public Camera rightMirrorCamera;

    [Header("Mirror UI Prefab")]
    public GameObject mirrorUIPrefab;

    private GameObject mirrorUIInstance;
    private RawImage leftMirrorImage;
    private RawImage centerMirrorImage;
    private RawImage rightMirrorImage;

    [Header("First Person Camera")]
    public Camera firstPersonCamera;

    void Start()
    {
        if (mirrorUIPrefab != null)
        {
            mirrorUIInstance = Instantiate(mirrorUIPrefab);
            mirrorUIInstance.transform.SetParent(null);

            leftMirrorImage = mirrorUIInstance.transform.Find("LeftMirror").GetComponent<RawImage>();
            centerMirrorImage = mirrorUIInstance.transform.Find("CenterMirror").GetComponent<RawImage>();
            rightMirrorImage = mirrorUIInstance.transform.Find("RightMirror").GetComponent<RawImage>();

            if (leftMirrorCamera != null) leftMirrorImage.texture = leftMirrorCamera.targetTexture;
            if (centerMirrorCamera != null) centerMirrorImage.texture = centerMirrorCamera.targetTexture;
            if (rightMirrorCamera != null) rightMirrorImage.texture = rightMirrorCamera.targetTexture;
        }

        UpdateMirrorVisibility();
    }

    void Update()
    {
        UpdateMirrorVisibility();
    }

    void UpdateMirrorVisibility()
{
    bool showMirrors = firstPersonCamera != null && firstPersonCamera.enabled;

    if (mirrorUIInstance != null)
    {
        mirrorUIInstance.SetActive(showMirrors);
    }

    if (leftMirrorCamera != null) leftMirrorCamera.enabled = showMirrors;
    if (centerMirrorCamera != null) centerMirrorCamera.enabled = showMirrors;
    if (rightMirrorCamera != null) rightMirrorCamera.enabled = showMirrors;
}
}
