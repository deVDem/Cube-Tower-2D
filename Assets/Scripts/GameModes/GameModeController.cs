using System;
using System.Collections;
using System.Collections.Generic;
using Game_Scene;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameModeController : MonoBehaviour
{
    public string GameMode;

    public Text GameModeText;

    private GameController _gameController;
    
    // Buff's
    public BuffManager BuffManager;
    
    
    // Timer
    public float startSecounds = 5f;
    public float intervalTimer = 0.1f;
    public Image timerProgressBar;
    private float secounds;
    private Coroutine timer;
    private Animator timerAnimator;
    
    
    // Loss rate table
    private DateTime date;

    private void Awake()
    {
        GameMode = PlayerPrefs.GetString("gamemode", "default");
    }

    void Start()
    {
        _gameController = transform.parent.gameObject.GetComponent<GameController>();
        GameMode = GameMode.ToLower();
        switch (GameMode)
        {
            case "default":
                break;
            case "ontime":
                GameModeText.text = LocaleManager.GetLocalizedText("Game.UI.GameMode.OnTime");
                timerAnimator = timerProgressBar.GetComponent<Animator>();
                startSecounds = 10f;
                break;
            case "hard":
                GameModeText.text = LocaleManager.GetLocalizedText("Game.UI.GameMode.Hard");
                timerAnimator = timerProgressBar.GetComponent<Animator>();
                startSecounds = 5f;
                BuffManager.Init();
                break;
            case "creative":
                GameModeText.text = LocaleManager.GetLocalizedText("Game.UI.GameMode.Creative");
                break;
            default:
                Debug.LogError("Unknown gamemode!");
                break;
        }
    }

    IEnumerator Timer()
    {
        secounds = startSecounds;
        while (!_gameController.IsLose)
        {
            yield return new WaitForSeconds(intervalTimer);
            secounds -= intervalTimer;
            if (secounds <= -1f) _gameController.GameOver();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float lastAmount;
        float needAmount;
        switch (GameMode)
        {
            case "default":
                break;
            case "ontime":
                lastAmount = timerProgressBar.fillAmount;
                needAmount = lastAmount - (lastAmount-secounds / startSecounds)*Time.deltaTime*3;
                timerProgressBar.fillAmount = needAmount;
                break;
            case "hard":
                lastAmount = timerProgressBar.fillAmount;
                needAmount = lastAmount - (lastAmount-secounds / startSecounds)*Time.deltaTime*3;
                timerProgressBar.fillAmount = needAmount;
                break;
            case "creative":
                break;
        }
    }

    public void onGameStarted()
    {
        switch (GameMode)
        {
            case "default":
                break;
            case "ontime":
                timer = StartCoroutine(Timer());
                timerAnimator.SetTrigger("showTimer");
                break;
            case "hard":
                timer = StartCoroutine(Timer());
                timerAnimator.SetTrigger("showTimer");
                break;
            case "creative":
                break;
        }

        if (GameMode != "creative")
        {
            date = DateTime.Now;
            Debug.Log(date.Minute+":"+date.Second);
        }
    }



    public void onGameEnded()
    {
        switch (GameMode)
        {
            case "default":
                break;
            case "ontime":
                StopCoroutine(timer);
                timerAnimator.SetTrigger("hideTimer");
                secounds = -1f;
                break;
            case "hard":
                StopCoroutine(timer);
                timerAnimator.SetTrigger("hideTimer");
                break;
            case "creative":
                break;
        }

        if (GameMode != "creative")
        {
            DateTime dateTime = DateTime.Now;
            TimeSpan lossTime = dateTime-date;
            Debug.Log(dateTime.Minute+":"+dateTime.Second);
            Debug.Log(Convert.ToInt32(lossTime.TotalMilliseconds));
        }
    }

    public void CubePlaced()
    {
        switch (GameMode)
        {
            case "default":
                break;
            case "ontime":
                secounds += 1f;
                if (secounds > startSecounds) secounds = startSecounds;
                break;
            case "hard":
                secounds += 1f;
                if (secounds > startSecounds) secounds = startSecounds;
                break;
            case "creative":
                break;
        }
    }
}
