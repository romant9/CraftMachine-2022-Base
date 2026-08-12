using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TwdCustomMod;
using UnityEngine;

public class LogToUi : MonoBehaviour
{
    private Color guicolor_backup;
    public Texture2D backSolid;

    // Adjust via the Inspector
    public int maxLines = 8;
    public int textSize = 14;
    private Queue<string> queue = new Queue<string>();
    private string currentText = "";

    private string allText;

    public ShowTooltip LogInfo;

    public bool IsShowLog;

    public void ShowLog(UIToggle tg)
    {
        IsShowLog = tg.value;

        if (!IsShowLog && !string.IsNullOrEmpty(allText)) MyTools.CopyToClipboard(allText);

    }

    void OnEnable()
    {
        LogInfo.EnCustomText = "Open System Log. Save to " + Application.persistentDataPath + '/' + "SystemLog_xxx.txt";
        LogInfo.RuCustomText = "Открыть системный лог. Сохранение в " + Application.persistentDataPath + '/' + "SystemLog_xxx.txt";

        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Delete oldest message
        if (queue.Count >= maxLines) 
        {
            queue.Dequeue();
        }

        queue.Enqueue(logString);

        var builder = new StringBuilder();
        foreach (string st in queue)
        {
            builder.Append(st).Append("\n");
        }

        currentText = builder.ToString();

        allText += logString + '\n';

    }

    void OnGUI()
    {
        if (IsShowLog)
        {
            guicolor_backup = GUI.backgroundColor;

            var colorBoxStyle = new GUIStyle(GUI.skin.box);
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.UpperLeft;

            colorBoxStyle.normal.background = backSolid;
            DrawColorBackground(new Rect(0, 0, Screen.width, Screen.height), new Color(0, 0, 0, 0.5f));

            GUI.skin.label.onHover.textColor = Color.white;
            labelStyle.fontSize = textSize * Screen.height / 1080;
            GUI.skin.textArea.fontSize = textSize;
            GUI.skin.button.fontSize = textSize + 6;

            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), currentText, labelStyle);
            DrawBox(new Rect(Screen.width - 299, 41, 168, 58), "", Color.red, colorBoxStyle);

            if (GUI.Button(new Rect(Screen.width - 300, 40, 170, 60), "Save Log"))
            {
                MyTools.CopyToClipboard(allText);

                string path = Application.persistentDataPath + '/' + "SystemLog_" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd_HH-mm") + ".txt";
                string content = allText;
                MyTools.SaveToFile(content, path, append: false);

                Process.Start(path);
            }

            if (GUI.Button(new Rect(Screen.width - 300, 100, 170, 60), "Close Log"))
            {
                IsShowLog = false;
            }
        }
    }

    public void DrawBox(Rect position, string text, Color color, GUIStyle style)
    {
        GUI.backgroundColor = color;
        GUI.Box(position, text, style);
        GUI.backgroundColor = guicolor_backup;
    }

    public void DrawColorBackground(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }
}