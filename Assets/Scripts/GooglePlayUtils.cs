using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GooglePlayUtils : MonoBehaviour
{
    private bool _connected;
    public static GooglePlayUtils Instance;
    private List<Achievement> orderAchievements = new List<Achievement>();
    
    public static void ReLogin()
    {
#if UNITY_ANDROID
        if (Instance._connected) return;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(b => { Instance._connected = b; });
        Debug.Log("GMS connected = " + Instance._connected);
#endif
    }

    public bool IsConnected()
    {
        return _connected;
    }

    void Start()
    {

#if UNITY_ANDROID
        Instance = this;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(b =>
        {
#if UNITY_EDITOR
            _connected = !b;
#else
            _connected = b;
            if (_connected)
            {
                StartCoroutine(SendAchievement());
            }
#endif
        });
        Debug.Log("GMS connected = " + _connected);
#endif
    }

    IEnumerator SendAchievement()
    {
        while (true)
        {
            if (orderAchievements.Count > 0 && Social.localUser.authenticated)
            {
                if (Social.localUser.authenticated)
                {
                    Achievement achievement = orderAchievements[0];

#if UNITY_ANDROID
                    PlayGamesPlatform.Instance.ReportProgress(achievement.ID, achievement.Progress, s =>
                    {
                        if (s) orderAchievements.Remove(achievement);
                    });
#endif

                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public static void GetAchievement(string id, float progress)
    {
        Instance.orderAchievements.Add(new Achievement(id, progress));
    }

    public static void UploadScore(string id, int score)
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated) PlayGamesPlatform.Instance.ReportScore(score, id, b => { });
#endif
    }

    public static void ShowAchievements()
    {

#if UNITY_ANDROID
        if (Social.localUser.authenticated) PlayGamesPlatform.Instance.ShowAchievementsUI();
#endif
    }

    public static void ShowLeaderBoard()
    {

#if UNITY_ANDROID
        if (Social.localUser.authenticated) PlayGamesPlatform.Instance.ShowLeaderboardUI();
#endif
    }

    private void OnApplicationQuit()
    {

#if UNITY_ANDROID
        PlayGamesPlatform.Instance.SignOut();
#endif
    }

    public bool Connected => _connected;
}

public class Achievement
{
    public string ID;
    public float Progress;

    public Achievement(string id, float progress)
    {
        ID = id;
        Progress = progress;
    }
}