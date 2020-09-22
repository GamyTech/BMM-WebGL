using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class LogController : MonoBehaviour
{
    public enum UploadFrequency
    {
        everyMonth,
        everyWeek,
        everyDay,
    }

    private static LogController m_instance;
    public static LogController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<LogController>()); }
        private set { m_instance = value; }
    }

    public string filePath = "C:/Users/123/Desktop/output_log.txt";
    public UploadFrequency uploadFrequency;
    public int MaxLine;
    private int m_lineCount;
    StreamWriter streamWriter;
    //bool pendingSubmit = false;

    void Start()
    {
        streamWriter = new StreamWriter(filePath);
        bool shouldSend = false;
        switch(uploadFrequency)
        {
            case UploadFrequency.everyMonth:
                shouldSend = (System.DateTime.UtcNow.Subtract(File.GetLastWriteTimeUtc(filePath)).TotalDays >= 30);
                break;
            case UploadFrequency.everyWeek:
                shouldSend = (System.DateTime.UtcNow.Subtract(File.GetLastWriteTimeUtc(filePath)).TotalDays >= 7);
                break;
            case UploadFrequency.everyDay:
                shouldSend = (System.DateTime.UtcNow.Subtract(File.GetLastWriteTimeUtc(filePath)).TotalHours >= 24);
                break;
        }

        if (shouldSend)
            SendAndClear();
    }

    void OnDestroy()
    {
        streamWriter.Close();
    }

    void Update()
    {
        //if (pendingSubmit && !string.IsNullOrEmpty(UserController.Instance.gtUser.Id))
        //{
        //    string fileName = UserController.Instance.gtUser.Id + "|" + AppInformation.GAME_ID + "|" + System.DateTime.UtcNow;
        //    GTUploadController.Instance.Upload(ContentType.Logs, filePath, fileName);
        //    pendingSubmit = false;
        //}
    }

    private void UploadCallback(CloudResponse response)
    {
        //if (response.responseType == ResponseType.OK)
        //    NetworkController.Instance.sendLog(response.uploadUrl);
        //else
        //    Debug.Log(response.rawResponse);
    }

    public void SendAndClear()
    {
        Send();
        streamWriter.Write("");
    }

    private void Send()
    {
        //pendingSubmit = true;
    }

    public void Log(string message)
    {
        streamWriter.WriteLine(message);
        streamWriter.Flush();
    }

}
