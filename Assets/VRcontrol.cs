using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRcontrol : MonoBehaviour
{
    bool game = false;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!game && SceneManager.GetActiveScene().name == "Game")
        {
            transform.position = new Vector3(-20.96f, 28f, -93.81f);
            game = true;
        }
    }
}
