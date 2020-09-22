using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class IOSBridge
{
#if UNITY_IOS
    [DllImport("__Internal")]
	private static extern void _InitPaypal (string clientId);
	[DllImport("__Internal")]
	private static extern void _ChangePaypalViewController (string item,int price,string email);
	[DllImport("__Internal")]
	private static extern void _ChangeCardViewController ();
#endif

    public static void ChangePaypalViewController(string item, int price, string email)
    {
#if UNITY_IOS
        _ChangePaypalViewController(item, price, email);
#endif
    }

    public static void ChangeCardScannerViewController()
    {
#if UNITY_IOS
		_ChangeCardViewController ();
#endif
    }

    public static void InitPaypal(string clientId)
    {
#if UNITY_IOS
		_InitPaypal (clientId);
#endif
    }
}
