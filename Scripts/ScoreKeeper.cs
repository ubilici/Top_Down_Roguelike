using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour {

    public static int score { get; private set; }
    float lastEnemyKillTime;
    int streakCount;
    float streakExpiryTime = 1.8f;
    GameUI gameUI;

    void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        gameUI = FindObjectOfType<GameUI>().GetComponent<GameUI>();

        score = 0;

        streakCount = 1;
    }

    void OnEnemyKilled()
    {
        if (Time.time < lastEnemyKillTime + streakExpiryTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 1;
        }

        Debug.Log(streakCount);

        if (streakCount >= 3)
        {
            gameUI.KillStreakBanner(streakCount);
        }

        lastEnemyKillTime = Time.time;

        score += 5 + streakCount * 2; //(int)Mathf.Pow(2, streakCount);
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }

}
