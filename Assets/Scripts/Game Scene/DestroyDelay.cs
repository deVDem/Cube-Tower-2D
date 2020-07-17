using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    public float delayInSeconds = 2f;
    void Start()
    {
        StartCoroutine(Destroy());
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(delayInSeconds);
        Destroy(gameObject);
    }
}
