using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLBrowserTabKit
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void OpenNewTabOnMouseUp(string url);
#endif

    public static void OpenURLOnMouseUp(string url)
    {
#if UNITY_WEBGL
        OpenNewTabOnMouseUp(url);
#endif
    }
}
