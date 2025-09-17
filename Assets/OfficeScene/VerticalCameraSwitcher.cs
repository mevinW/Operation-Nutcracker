using System.Collections;
using UnityEngine;

public class VerticalCameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Camera cameraA;  // follows P1
    [SerializeField] private Camera cameraB;  // follows P2
    [SerializeField] private Camera cameraC;  // shared view

    [Header("Player References")]
    [SerializeField] private Transform playerOne;
    [SerializeField] private Transform playerTwo;

    [Header("Switch Settings")]
    [SerializeField] private float switchDistance = 2f;
    [SerializeField] private float splitScreenDelay = 0.5f;

    [Header("Zoom Settings")]
    [SerializeField] private float fullSize = 5f;
    [SerializeField] private float splitSize = 2.5f;

    [Header("Y‑Clamp Settings")]
    [Tooltip("Minimum Y for shared camera")]
    [SerializeField] private float sharedMinY = 0f;
    [Tooltip("Minimum Y for split cameras")]
    [SerializeField] private float splitMinY = -2.5f;

    [Header("Smoothing")]
    [SerializeField] private float posSmoothTime = 0.3f;
    [SerializeField] private float sizeSmoothTime = 0.3f;

    private bool isSplit;
    private Coroutine splitCoroutine;

    // velocities for SmoothDamp
    private float velYA, velYB, velYC;
    private float velSizeA, velSizeB, velSizeC;

    private void Awake()
    {
        EnterShared();
    }

    private void LateUpdate()
    {
        float y1 = playerOne.position.y;
        float y2 = playerTwo.position.y;
        bool wantSplit = Mathf.Abs(y1 - y2) > switchDistance;

        // schedule or cancel split transition
        if (wantSplit && !isSplit && splitCoroutine == null)
            splitCoroutine = StartCoroutine(DelayedSplit());
        else if (!wantSplit && splitCoroutine != null)
        {
            StopCoroutine(splitCoroutine);
            splitCoroutine = null;
        }

        // if they’ve come back together, go shared immediately
        if (!wantSplit && isSplit)
            EnterShared();

        // decide which mode to render
        if (isSplit)
            UpdateSplit();
        else
            UpdateShared((y1 + y2) * 0.5f);
    }

    private IEnumerator DelayedSplit()
    {
        yield return new WaitForSeconds(splitScreenDelay);
        if (Mathf.Abs(playerOne.position.y - playerTwo.position.y) > switchDistance)
            EnterSplit();
        splitCoroutine = null;
    }

    private void EnterSplit()
    {
        isSplit = true;
        cameraC.gameObject.SetActive(false);
        cameraA.gameObject.SetActive(true);
        cameraB.gameObject.SetActive(true);

        // ——— SNAP both cameras to their players immediately ———
        float y1 = Mathf.Max(playerOne.position.y, splitMinY);
        float y2 = Mathf.Max(playerTwo.position.y, splitMinY);
        cameraA.transform.position = new Vector3(
            cameraA.transform.position.x, y1, cameraA.transform.position.z);
        cameraB.transform.position = new Vector3(
            cameraB.transform.position.x, y2, cameraB.transform.position.z);
        cameraA.orthographicSize = splitSize;
        cameraB.orthographicSize = splitSize;

        // ——— reset smoothing velocities so there’s no initial “drift” ———
        velYA = velYB = velSizeA = velSizeB = 0f;
    }

    private void EnterShared()
    {
        isSplit = false;
        if (splitCoroutine != null)
        {
            StopCoroutine(splitCoroutine);
            splitCoroutine = null;
        }
        cameraA.gameObject.SetActive(false);
        cameraB.gameObject.SetActive(false);
        cameraC.gameObject.SetActive(true);
    }

    private void UpdateSplit()
    {
        // determine which half of the screen each camera occupies
        bool oneIsHigher = playerOne.position.y > playerTwo.position.y;

        // Camera A always follows Player One
        SetupSplitCam(cameraA, playerOne, oneIsHigher, ref velYA, ref velSizeA);
        // Camera B always follows Player Two
        SetupSplitCam(cameraB, playerTwo, !oneIsHigher, ref velYB, ref velSizeB);
    }

    private void SetupSplitCam(Camera cam, Transform target, bool isTop,
                               ref float velY, ref float velSize)
    {
        // set viewport rect
        cam.rect = isTop
            ? new Rect(0f, 0.5f, 1f, 0.5f)
            : new Rect(0f, 0f, 1f, 0.5f);

        // move & zoom, clamped to splitMinY
        float ty = Mathf.Max(target.position.y, splitMinY);
        Smooth(cam, cam.transform.position.x, ty, splitSize, ref velY, ref velSize);
    }

    private void UpdateShared(float avgY)
    {
        cameraA.gameObject.SetActive(false);
        cameraB.gameObject.SetActive(false);
        cameraC.gameObject.SetActive(true);

        cameraC.rect = new Rect(0f, 0f, 1f, 1f);
        // move & zoom, clamped to sharedMinY
        float ty = Mathf.Max(avgY, sharedMinY);
        Smooth(cameraC, cameraC.transform.position.x, ty, fullSize,
               ref velYC, ref velSizeC);
    }

    private void Smooth(Camera cam, float fx, float ty, float size,
                        ref float velY, ref float velSize)
    {
        float newY = Mathf.SmoothDamp(
            cam.transform.position.y, ty, ref velY, posSmoothTime);
        float newSize = Mathf.SmoothDamp(
            cam.orthographicSize, size, ref velSize, sizeSmoothTime);

        cam.transform.position = new Vector3(fx, newY, cam.transform.position.z);
        cam.orthographicSize = newSize;
    }
}
