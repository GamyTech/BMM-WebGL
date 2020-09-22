//using System;
//using System.Collections;
//using System.Runtime.InteropServices;
//using System.Diagnostics;
//using UnityEngine;
//using System.Threading;
//using System.Windows.Forms;
//using System.Drawing;

//public class windowsTest : MonoBehaviour
//{


//    [DllImport("user32.dll")]
//    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
//    [DllImport("user32.dll")]
//    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
//    [DllImport("user32.dll")]
//    static extern IntPtr GetForegroundWindow();
//    [DllImport("user32.dll")]
//    static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);
//    [DllImport("user32.dll")]
//    public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

//    [StructLayout(LayoutKind.Sequential)]
//    public struct RECT
//    {
//        public int Left;        // x position of upper-left corner
//        public int Top;         // y position of upper-left corner
//        public int Right;       // x position of lower-right corner
//        public int Bottom;      // y position of lower-right corner
//    }

//    // not used rigth now
//    //const uint SWP_NOMOVE = 0x2;
//    //const uint SWP_NOSIZE = 1;
//    //const uint SWP_NOZORDER = 0x4;
//    //const uint SWP_HIDEWINDOW = 0x0080;

//    const uint SWP_SHOWWINDOW = 0x0040;
//    const int GWL_STYLE = -16;
//    const int WS_BORDER = 1;
//    Thread thread;

//    void Start()
//    {
//        thread = new Thread(fixedUpdate);
//        thread.Start();
//    }

//    void OnDestroy()
//    {
//        if (thread != null)
//            thread.Abort();
//    }

//    int lastPosX = 0;
//    int lastPosY = 0;
//    int lastWidth = 0;
//    int lastHeight = 0;
//    void fixedUpdate()
//    {
//        while(true)
//        {
//            IntPtr ptr = GetForegroundWindow();
//            UnityEngine.Debug.LogError(ptr);

//            const int nChars = 256;
//            System.Text.StringBuilder Buff = new System.Text.StringBuilder(nChars);
//            if (GetWindowText(ptr, Buff, nChars) > 0)
//                UnityEngine.Debug.LogError(Buff.ToString());

//            RECT rect = new RECT();
//            GetWindowRect(ptr, ref rect);
//            Rect screenPosition = new Rect();
//            screenPosition.x = rect.Left;
//            screenPosition.y = rect.Top;
//            screenPosition.width = rect.Right - rect.Left;
//            screenPosition.height = rect.Bottom - rect.Top;
//            UnityEngine.Debug.LogError(screenPosition.size);
//            float minRatio = 1.5f;
//            float maxRatio = 1.777f;
//            if (lastWidth != 0)
//            {
//                if (screenPosition.height < 300)
//                {
//                    screenPosition.y = lastPosY;
//                    screenPosition.height = 300;
//                }
//                if (screenPosition.width < 600)
//                {
//                    screenPosition.x = lastPosX;
//                    screenPosition.width = 600;
//                }


//                float ratio = (float)screenPosition.width / (float)screenPosition.height;
//                if (ratio < minRatio)
//                {
//                    if (lastHeight != (int)screenPosition.height)
//                        screenPosition.width = screenPosition.height * minRatio;
//                    else if (lastWidth != (int)screenPosition.width)
//                        screenPosition.height = screenPosition.width / minRatio;
//                }
//                else if (ratio > maxRatio)
//                {
//                    if (lastHeight != (int)screenPosition.height)
//                        screenPosition.width = screenPosition.height * maxRatio;
//                    else if (lastWidth != (int)screenPosition.width)
//                        screenPosition.height = screenPosition.width / maxRatio;
//                }

//                bool result = SetWindowPos(ptr, 0, (int)screenPosition.x, (int)screenPosition.y, (int)screenPosition.width, (int)screenPosition.height, SWP_SHOWWINDOW);
//            }

//            lastWidth = (int)screenPosition.width;
//            lastHeight = (int)screenPosition.height;
//            lastPosX = (int)screenPosition.x;
//            lastPosY = (int)screenPosition.y;

//            Thread.Sleep(0);
//        }


//        //if (Input.GetKeyDown(KeyCode.UpArrow))
//        //{


//        //    Process[] processes = Process.GetProcesses(".");
//        //    UnityEngine.Debug.LogError(processes.Length);
//        //    for (int x = 0; x < processes.Length; ++x)
//        //    {
//        //        IntPtr handle = processes[x].MainWindowHandle;
//        //        UnityEngine.Debug.LogError(x + " + " + handle);
//        //        Control form = Control.FromHandle(handle);
//        //        if(form != null)
//        //        {
//        //            UnityEngine.Debug.LogError(x + " : " + form.Name);
//        //            form.MinimumSize = new System.Drawing.Size(800, 600);
//        //        }
//        //    }
//        //}

//    }
//    bool listening = true;

//    void AdaptSize(int width, int height)
//    {


//    }

//}