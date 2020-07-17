using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PostProcessColor : MonoBehaviour
{
    public PostProcessVolume ProcessVolume;

    public Image fadeout;

    private bool _isfadeoutNotNull;

    // Start is called before the first frame update
    void Start()
    {
        _isfadeoutNotNull = fadeout != null;
        if (ProcessVolume == null)
        {
            ProcessVolume = GetComponent<PostProcessVolume>();
            var fadeoutColor = fadeout.color;
            fadeoutColor.a = 1;
            fadeout.color = fadeoutColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isfadeoutNotNull) ProcessVolume.weight = 1 - fadeout.color.a;
    }
}