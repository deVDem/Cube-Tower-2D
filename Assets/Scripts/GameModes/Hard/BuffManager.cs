using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public GameObject[] BuffPrefabs;
    public int minBuffs = 10, maxBuffs = 25, maxX = 5, maxY = 300;
    public AnimationCurve curveFactor;
    private GameObject buffsObject;

    public void Init()
    {
        if (BuffPrefabs.Length < 1)
        {
            Debug.LogError("Add buff prefabs!");
            return;
        }

        spawnBuffs();
    }

    void spawnBuffs()
    {
        if (buffsObject != null) Destroy(buffsObject);
        buffsObject = new GameObject(name = "Buffs");
        int buffsCount = Random.Range(minBuffs, maxBuffs);
        for (int i = 0; i < buffsCount; i++)
        {
            int buffIndex = Random.Range(0, BuffPrefabs.Length);
            int x = Random.Range(-maxX, maxX);
            int y = Random.Range(5, maxY);
            bool spawn = Random.Range(0f, 1f) <= curveFactor.Evaluate((float) y/maxY);
            Debug.Log("spawn = " + spawn+". Y="+y+". MaxY="+maxY+". Time: "+(float) y/maxY+". Curve: "+curveFactor.Evaluate((float) y/maxY));
            if (spawn)
            {
                GameObject newBuff = Instantiate(BuffPrefabs[buffIndex], new Vector2(x, y), Quaternion.identity);
                newBuff.transform.SetParent(buffsObject.transform);
            }
        }
    }
}