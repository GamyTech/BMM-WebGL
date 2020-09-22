using UnityEngine;
using UnityEditor;

public class AutoSaveWindow : EditorWindow
{
    const int MIN_TIME = 60;
    const int MAX_TIME = 1200;

    private float saveInterval = 60f;
    private bool isOn = true;
    private float nextSave;

    [MenuItem("Window/Auto Save")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AutoSaveWindow));
    }

    void Awake()
    {
        EditorApplication.update = Update;
        EditorApplication.playModeStateChanged += playModeChanged;
    }

    private void OnDestroy()
    {
        EditorApplication.playModeStateChanged -= playModeChanged;
    }

    void Update()
    {
        if (isOn == false || EditorApplication.isPlayingOrWillChangePlaymode) return;

        if (Time.realtimeSinceStartup > nextSave)
        {
            StartCountDown();
            Save();
        }
    }

    void playModeChanged(PlayModeStateChange change)
    {
        if (isOn && EditorApplication.isPlayingOrWillChangePlaymode == false)
        {
            StartCountDown();
        }
    }

    void StartCountDown()
    {
        nextSave = Time.realtimeSinceStartup + saveInterval;
    }

    void OnGUI()
    {
        bool b = EditorGUILayout.Toggle("Enable Auto Save", isOn);
        if(b != isOn)
        {
            isOn = b;
            StartCountDown();
        }
        float i = EditorGUILayout.Slider("Save Interval (Sec)", saveInterval, MIN_TIME, MAX_TIME);
        if(i != saveInterval)
        {
            saveInterval = i;
            StartCountDown();
        }
        if (isOn)
        {
            EditorGUILayout.LabelField("Next Save in: " + (nextSave - Time.realtimeSinceStartup).ToString("N1") + " sec");
        }
    }

    private void Save()
    {
        Debug.Log("Saving...");
        AssetDatabase.SaveAssets();
    }
}
