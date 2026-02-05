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
    
    Texture2D screenA;
    Texture2D screenB;

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
        if (screenB.width != screenA.width || screenB.height != screenA.height)
            return 0f;

        Color[] p1 = screenB.GetPixels();
        Color[] p2 = screenA.GetPixels();

        int matchCount = 0;
        int total = p1.Length;

        const float hueTolerance = 0.05f;        // 0–1 range (≈ 7°)
        const float satTolerance = 0.05f;
        const float valTolerance = 0.05f;

        for (int i = 0; i < total; i++)
        {
            Color.RGBToHSV(p1[i], out float h1, out float s1, out float v1);
            Color.RGBToHSV(p2[i], out float h2, out float s2, out float v2);

            bool hueMatch = Mathf.Abs(h1 - h2) < hueTolerance ||
                            Mathf.Abs(h1 - h2) > 1f - hueTolerance; // wrap-around

            bool satMatch = Mathf.Abs(s1 - s2) < satTolerance;
            bool valMatch = Mathf.Abs(v1 - v2) < valTolerance;

            if (hueMatch && satMatch && valMatch)
                matchCount++;
        }

        float similarity = (float)matchCount / total * 100f;
        Debug.Log(similarity.ToString("F2") + "% similarity (HSV)");
        return similarity;

    }

}