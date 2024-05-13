using UnityEngine;
using System.Diagnostics;
using System.Collections;
using EasyButtons;
using System;

public class CallPythonScript : MonoBehaviour
{
    private string pythonExecutable = @"Assets\python\venv\Scripts\python.exe";
    private string scriptPath = @"Assets\python\image_query.py";
    public string imagePath = @"Assets\frau.jpg";
    [TextAreaAttribute(1, 10)] public string prompt = "What is in this image?";

    [Button]
    public void CallScript()
    {
        StartCoroutine(CallPython());
    }

      public void CallScript(string imgPath)
    {
        imagePath = imgPath;
        StartCoroutine(CallPython());
    }

    IEnumerator CallPython()
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = pythonExecutable;
        psi.Arguments = $"\"{scriptPath}\" \"{imagePath}\" \"{prompt}\"";
        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.CreateNoWindow = true;

        string output = "";
        using (Process process = Process.Start(psi))
        {
            output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();  // Wait for the process to exit

            if (!string.IsNullOrEmpty(errors))
            {
                UnityEngine.Debug.LogError("Python Error: " + errors);
            }
        }

        if (!string.IsNullOrEmpty(output))
        {
            try
            {
                RootObject jsonData = JsonUtility.FromJson<RootObject>(output);
                if (jsonData.choices != null && jsonData.choices.Length > 0)
                {
                    UnityEngine.Debug.Log("Content: " + jsonData.choices[0].message.content);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No content found in JSON response.");
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Failed to parse JSON: " + e.Message);
            }
        }

        yield return null;
    }
}


[System.Serializable]
public class RootObject
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}
