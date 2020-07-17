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
        if (Instance._connected) return;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(b => { Instance._connected = b; });
        Debug.Log("GMS connected = " + Instance._connected);
    }

    void Start()
    {
        Instance = this;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
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
                    Debug.Log("Achievement getting - " + achievement.ID + " on progress: " + achievement.Progress);
                    PlayGamesPlatform.Instance.ReportProgress(achievement.ID, achievement.Progress, s =>
                    {
                        if (s) orderAchievements.Remove(achievement);
                        else Debug.LogWarning("Achievement not sended");
                    });

                    Debug.Log("Achievement get - " + achievement.ID + " on progress: " + achievement.Progress);
                }
                else Debug.LogWarning("GP not connected.");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public static void GetAchievement(string id, float progress)
    {
        Debug.Log("Achievement added: id=" + id + ". Progress: " + progress);
        Instance.orderAchievements.Add(new Achievement(id, progress));
    }

    public static void UploadScore(string id, int score)
    {
        if (Social.localUser.authenticated) PlayGamesPlatform.Instance.ReportScore(score, id, b => { });
        else Debug.LogWarning("GP not connected.");

        Debug.Log("Score get - " + id + " on score: " + score);
    }

    public static void ShowAchievements()
    {
        if (Social.localUser.authenticated) PlayGamesPlatform.Instance.ShowAchievementsUI();
        Debug.Log("Showed Achievements");
    }

    public static void ShowLeaderBoard()
    {
        if (Social.localUser.authenticated) PlayGamesPlatform.Instance.ShowLeaderboardUI();
        Debug.Log("Showed Leaderboards");
    }

    private void OnApplicationQuit()
    {
        PlayGamesPlatform.Instance.SignOut();
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