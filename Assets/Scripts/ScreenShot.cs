using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenShot : MonoBehaviour
{
    private Camera _myCamera;

    public bool takeScreenshot=false;
    private int _counter=1;
    private DateTime _startTime;

    // Start is called before the first frame update
    void Start()
    {
        _myCamera = GetComponent<Camera>();
        _startTime=DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            takeScreenshot = !takeScreenshot;
        }
    }

    private void OnPostRender()
    {
        if (takeScreenshot)
        {
            _myCamera.targetTexture = RenderTexture.GetTemporary(1080, 1920, 16);
            takeScreenshot = false;
            RenderTexture renderTexture = _myCamera.targetTexture;
            Texture2D renderResult =
                new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToJPG();
            DateTime dateTime = DateTime.Now;
            string lang = LocaleManager.GetCurrentLocale();
            System.IO.File.WriteAllBytes("A:\\Projects\\Games\\Unity\\Cube Tower 2D\\Media\\Screenshots\\" +lang + "\\Screenshot_" + dateTime.Day+"_"+dateTime.Hour+"_"+dateTime.Minute+"_"+dateTime.Second + ".jpg", byteArray);
            //System.IO.File.WriteAllBytes("A:\\Projects\\Games\\Unity\\Cube Tower 2D\\Media\\Video\\" +lang + "\\"+SceneManager.GetActiveScene().name+"\\Shot_" + _counter + ".jpg", byteArray);
            Debug.Log("Screen saved!");
            RenderTexture.ReleaseTemporary(renderTexture);
            _myCamera.targetTexture = null;
            //_counter++;
            GC.Collect();
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Shots: "+_counter+". Avg.FPS: "+(DateTime.Now-_startTime).TotalMilliseconds/_counter);
    }
}