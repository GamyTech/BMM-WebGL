using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AppsFlyerTrackerCallbacks : MonoBehaviour {

	public Text callbacks;

	// Use this for initialization
	void Start () {
		print ("AppsFlyerTrackerCallbacks on Start");
		
	}
	
	public void didReceiveConversionData(string conversionData) {
		printCallback ("AppsFlyerTrackerCallbacks:: got conversion data = " + conversionData);
	}
	
	public void didReceiveConversionDataWithError(string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got conversion data error = " + error); 
            TrackingKit.SendCustomEvent("AppsFlyerError", new System.Collections.Generic.Dictionary<string, object>() { { "error", "got conversion data error = " + error } });
    }
	
	public void didFinishValidateReceipt(string validateResult) {
		printCallback ("AppsFlyerTrackerCallbacks:: got didFinishValidateReceipt  = " + validateResult);
    }

    public void didFinishValidateReceiptWithError (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got idFinishValidateReceiptWithError error = " + error);
        TrackingKit.SendCustomEvent("AppsFlyerError", new System.Collections.Generic.Dictionary<string, object>() { { "error", "got idFinishValidateReceiptWithError error = " + error } });
	}
	
	public void onAppOpenAttribution(string validateResult) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onAppOpenAttribution  = " + validateResult);
    }

    public void onAppOpenAttributionFailure (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onAppOpenAttributionFailure error = " + error);
        TrackingKit.SendCustomEvent("AppsFlyerError", new System.Collections.Generic.Dictionary<string, object>() { { "error", "got onAppOpenAttributionFailure error = " + error } });
	}
	
	public void onInAppBillingSuccess () {
		printCallback ("AppsFlyerTrackerCallbacks:: got onInAppBillingSuccess succcess");
	}

	public void onInAppBillingFailure (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onInAppBillingFailure error = " + error);
        TrackingKit.SendCustomEvent("AppsFlyerError", new System.Collections.Generic.Dictionary<string, object>() { { "error", "got onInAppBillingFailure error = " + error } });
	}

	void printCallback(string str) {
        if (callbacks != null)
            callbacks.text += str + "\n";
        else
            Debug.Log(str);
    }
}
