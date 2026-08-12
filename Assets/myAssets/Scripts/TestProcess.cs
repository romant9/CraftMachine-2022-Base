using ClipperLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

public class TestProcess : MonoBehaviour
{
    private Process process;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public static string RunCommand()
    {
        var output = string.Empty;
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = @"E:\Program Files\Epic Games\KirrKiller\TheWalkingDeadNoMansLand\The Walking Dead No Man's Land.exe",
                Arguments = @"'E:\Program Files\Epic Games\KirrKiller\TheWalkingDeadNoMansLand\The Walking Dead No Man'""'""'s Land.exe' -AUTH_LOGIN=unused -AUTH_PASSWORD=85efa08e51564b6bae71a6fa0c4ce0af -AUTH_TYPE=exchangecode -epicapp=b5271de997b44ef993d7a84196717452 -epicenv=Prod -EpicPortal -epicusername=BlodymaryHere -epicuserid=b796d93e807144d6a6da1e392f245f0d -epiclocale=ru -epicsandboxid=7a6c212ee9cf425c82d4b08af3f564a9",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = false
            };

            var proc = Process.Start(startInfo);
            
            output = proc.StandardOutput.ReadToEnd();
            
            proc.WaitForExit(20000);
            

            return output;
        }
        catch (Exception)
        {
            return output;
        }
    }
    public void OnClick()
    {
        string output = RunCommand();

        DebugTWD.Log(output);

        using (Process process = new Process())
        {
            string file = @"'E:\Program Files\Epic Games\KirrKiller\TheWalkingDeadNoMansLand\The Walking Dead No Man'""'""'s Land.exe' -AUTH_LOGIN=unused -AUTH_PASSWORD=85efa08e51564b6bae71a6fa0c4ce0af -AUTH_TYPE=exchangecode -epicapp=b5271de997b44ef993d7a84196717452 -epicenv=Prod -EpicPortal -epicusername=BlodymaryHere -epicuserid=b796d93e807144d6a6da1e392f245f0d -epiclocale=ru -epicsandboxid=7a6c212ee9cf425c82d4b08af3f564a9";

            process.StartInfo.FileName = @"E:\Program Files\Epic Games\KirrKiller\TheWalkingDeadNoMansLand\The Walking Dead No Man's Land.exe";
                //FileName = "e:\\Unity Projects\\TWD\\legendary.exe",
                //process.StartInfo.WorkingDirectory = @"c:\temp\";
            process.StartInfo.Arguments = file;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
       
            process.OutputDataReceived += DataReceivedEventHandler; //обработчик события при получении очередной строки с данными
            process.ErrorDataReceived += ErrorReceivedEventHandler; //обработчик события при получении ошибки

            process.Start();
            process.BeginOutputReadLine(); //начинаем считывать данные из потока 
            process.BeginErrorReadLine(); //начинаем считывать данные об ошибках

            //StreamReader reader = process.StandardOutput;
            //string output = reader.ReadToEnd();

            //string result = process.StandardOutput.ReadToEnd();
            //Debug.Log(output + '\n');

            process.WaitForExit(); //ожидаем окончания работы приложения, чтобы очистить буфер

            process.OutputDataReceived -= DataReceivedEventHandler;
            process.ErrorDataReceived -= ErrorReceivedEventHandler;

            process.Close(); //завершает процесс
            //Debug.Log("process closed with :\n" + output);
        }

        //StartCoroutine(WaitForProcess(Time.realtimeSinceStartup));

        //Process.Start("e:\\Unity Projects\\TWD\\legendary.exe", "/C legendary launch b5271de997b44ef993d7a84196717452 --json"); //запускаем процесс
    }

    public void GetProcess()
    {
        var processes = Process.GetProcessesByName("The Walking Dead No Man's Land");
        if (processes != null && processes.Length > 0)
        {
            process = processes.First();

            DebugTWD.Log(process.MainModule.FileName);

            DebugTWD.Log(process.MainModule.FileName + "\n" +
                process.MainModule.ModuleName + "\n" +
                process.MainModule.Container.ToString() + "\n");

            //process.OutputDataReceived += DataReceivedEventHandler; //обработчик события при получении очередной строки с данными
            //process.ErrorDataReceived += ErrorReceivedEventHandler; //обработчик события при получении ошибки

            //process.BeginOutputReadLine(); //начинаем считывать данные из потока 
            //process.BeginErrorReadLine(); //начинаем считывать данные об ошибках
        }
        else
        {
            DebugTWD.Log("no processes");
        }     
    }

    private IEnumerator WaitForProcess(float time)
    {
        var processes = Process.GetProcessesByName("The Walking Dead No Man's Land");
        if (Time.realtimeSinceStartup - time > 10)
        {
            DebugTWD.Log("no processes");
            yield break;
        }

        yield return new WaitUntil(() => processes != null && processes.Length > 0);
        process = processes.First();
        DebugTWD.Log(process.MainModule.FileName);

        //Debug.Log(process.MainModule.FileName + "\n" +
        //    process.MainModule.ModuleName + "\n" +
        //    process.MainModule.Container.ToString() + "\n");
      
        //process = processes.First();

        //using (Process process = processes.First())
        //{
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardError = true;

        //    StreamReader reader = process.StandardOutput;
        //    string output = reader.ReadToEnd();

        //    process.WaitForExit();

        //    Debug.Log("Is available " + process.ProcessName + " " + output);

        //}

        //process.StartInfo.RedirectStandardOutput = true;
        //process.StartInfo.RedirectStandardError = true;

        //process.OutputDataReceived += DataReceivedEventHandler; //обработчик события при получении очередной строки с данными
        //process.ErrorDataReceived += ErrorReceivedEventHandler; //обработчик события при получении ошибки

        //StreamReader reader = process.StandardOutput;
        //string output = reader.ReadToEnd();

        //process.WaitForExit();

        //process.BeginOutputReadLine(); //начинаем считывать данные из потока 
        //process.BeginErrorReadLine(); //начинаем считывать данные об ошибках       

    }

    static void DataReceivedEventHandler(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            DebugTWD.Log($"Внешний процесс вернул данные: {e.Data}");
        }

    }
    static void ErrorReceivedEventHandler(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            DebugTWD.Log($"Внешний процесс вернул ошибку: {e.Data}");
        }
    }

}
