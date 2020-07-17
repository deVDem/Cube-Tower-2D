using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text displayText;
    public bool TextEnabled;

    private int framesPerSecound;

    public void Awake()
    {
        StartCoroutine(Timer());
    }

    public void Update()
    {
        framesPerSecound++;
        Color displayTextColor = displayText.color;
        if (TextEnabled) displayTextColor.a = displayTextColor.a + (1f - displayTextColor.a) * Time.deltaTime * 4f;
        else displayTextColor.a = displayTextColor.a + (0f - displayTextColor.a) * Time.deltaTime * 4f;
        displayText.color = displayTextColor;
    }

    IEnumerator Timer()
    {
        while (true)
        {
            displayText.text = framesPerSecound*4 + " FPS";
            framesPerSecound = 0;
            yield return new WaitForSeconds(0.25f);
        }
    }
}