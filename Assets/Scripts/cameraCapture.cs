using System.IO;
using UnityEngine;
using UnityEngine.UI;
using EasyButtons;
using System.Collections;
using System;

public class cameraCapture : MonoBehaviour
{
    private WebCamTexture webcamTexture;
        public RawImage webcamPreview;
            public Text countdownText; 

    private int imageCounter = 1;

    public float countdownTime = 3f;
    public float remainingTime;


    void Start()
    {
        Application.targetFrameRate = 60;   // wihtout this, the camera does not capture continuously
        webcamTexture = new WebCamTexture();
        webcamPreview.texture = webcamTexture;
        webcamTexture.Play();
        remainingTime = countdownTime;
        countdownText.text = countdownTime.ToString();
    }


    [Button]
    public void StartCountdown()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        remainingTime = countdownTime;
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            float displayTime = (float)(Math.Floor(remainingTime * 10) / 10);
            countdownText.text = displayTime.ToString();            
            yield return null;
        }
    
        CaptureImage();    
        remainingTime = countdownTime;
        countdownText.text = remainingTime.ToString();
    }

    private void CaptureImage()
    {
        Texture2D snap = new Texture2D(webcamTexture.width, webcamTexture.height);
        snap.SetPixels(webcamTexture.GetPixels());
        snap.Apply();

        byte[] bytes = snap.EncodeToPNG();
        string filename = GenerateFilename();

        File.WriteAllBytes(filename, bytes);

        Debug.Log("Saved image to " + filename);
        Destroy(snap);

        Analyze(filename);
    }
    

    private string GenerateFilename()
    {
        string folderPath = Application.dataPath + "/";
        string fileNameBase = "image_";
        string fullFilePath;

        do
        {
            string fileName = fileNameBase + imageCounter.ToString("D3") + ".png";
            fullFilePath = folderPath + fileName;
            imageCounter = (imageCounter % 999) + 1;
        } while (File.Exists(fullFilePath));

        return fullFilePath;
    }

       void Analyze(string path)
    {
        GetComponent<CallPythonScript>().CallScript(path);
    }
}
