using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRatioElement : MonoBehaviour {

    private Vector2 InitialSize;

	void OnEnable () {
        SettingsController.OnScreenSizeChanged += Rescale;
        StartCoroutine(InitialRescale());
    }
	
	void OnDisable() {
        SettingsController.OnScreenSizeChanged -= Rescale;
    }

    IEnumerator InitialRescale()
    {
        RectTransform rectt = this.transform as RectTransform;
        InitialSize = new Vector2(rectt.rect.width, rectt.rect.height);
        yield return null;
        Rescale(Screen.width, Screen.height);
    }

    private void Rescale(int width, int height)
    {
        RectTransform rectt = this.transform.parent as RectTransform;
        Vector2 ParentSize = new Vector2(rectt.rect.width, rectt.rect.height);
        float WidthRatio = ParentSize.x / InitialSize.x;
        float heightRatio = ParentSize.y / InitialSize.y;
        float minimumScale = WidthRatio < heightRatio ? WidthRatio : heightRatio;
        transform.localScale = new Vector3(minimumScale, minimumScale, 1.0f);
    }

}
