using System.Collections;
using UnityEngine;

public class ScreenPixelComparer : MonoBehaviour
{
    public static ScreenPixelComparer Instance;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    public int captureWidth = 1920;
    public int captureHeight = 1080;

    public Texture2D screenA;
    public Texture2D screenB;
    [ContextMenu("CaptureFirstScreen")]

    public IEnumerator CaptureFirstScreen()
    {
        yield return new WaitForEndOfFrame();
        screenA = ScreenCapture.CaptureScreenshotAsTexture();
        Debug.Log("First captured");
    }

    public IEnumerator CaptureSecondScreen()
    {
        yield return new WaitForEndOfFrame();
        screenB = ScreenCapture.CaptureScreenshotAsTexture();
        Debug.Log("Second captured");
    }

    public float CompareScreens()
    {
        if (screenA == null || screenB == null)
        {
            Debug.LogError("Capture both first!");
            return 0f;
        }

        if (screenA.width != screenB.width ||
            screenA.height != screenB.height)
        {
            Debug.LogError("Resolution mismatch!");
            return 0f;
        }

        byte[] a = screenA.GetRawTextureData();
        byte[] b = screenB.GetRawTextureData();

        int total = a.Length;
        int match = 0;

        for (int i = 0; i < total; i++)
        {
            if (a[i] == b[i])
                match++;
        }

        float similarity = (float)match / total;
        Debug.Log($"RAW Similarity = {similarity * 100f}%");

        return (similarity*100f);
    }

}