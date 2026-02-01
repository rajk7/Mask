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

    private Texture2D screenA;
    private Texture2D screenB;
    [ContextMenu("CaptureFirstScreen")]
    public void CaptureFirstScreen()
    {
        screenA = CaptureScreen(captureWidth, captureHeight);
        Debug.Log("First screen captured!");
    }
    [ContextMenu("CaptureSecondScreen")]
    public void CaptureSecondScreen()
    {
        screenB = CaptureScreen(captureWidth, captureHeight);
        Debug.Log("Second screen captured!");
    }

    Texture2D CaptureScreen(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        RenderTexture rt = new RenderTexture(width, height, 24);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        Destroy(rt);

        return tex;
    }
    [ContextMenu("CompareScreens")]
    public float CompareScreens()
    {
        if (screenA == null || screenB == null)
        {
            Debug.LogError("Capture both screens first!");
            return 0f;
        }

        Color[] pixelsA = screenA.GetPixels();
        Color[] pixelsB = screenB.GetPixels();

        int totalPixels = pixelsA.Length;
        int matchingPixels = 0;

        for (int i = 0; i < totalPixels; i++)
        {
            if (ColorApproximatelyEqual(pixelsA[i], pixelsB[i]))
                matchingPixels++;
        }

        float similarity = (float)matchingPixels / totalPixels;
        Debug.Log($"Image Similarity: {similarity * 100f}%");

        return similarity;
    }

    bool ColorApproximatelyEqual(Color a, Color b, float tolerance = 0.005f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}