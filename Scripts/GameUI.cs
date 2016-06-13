using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public Image fadePlane;

    [Header("Interface")]
    public Text score;
    public Text ammo;
    public Text health;
    public Text gameOverScoreUIText;
    public GameObject gameOverUI;

    [Header("Banner")]
    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;

    [Header("Kill Streak Banner")]
    public RectTransform killStreakBanner;
    public Text killStreak;
    public RectTransform killStreakTimerBar;

    Spawner spawner;
    Player player;

    float killStreakTimerBarTime;
    float killStreakEndDelayTime;
    bool killStreakBannerToggle = false;
    public int totalPoints;

    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        totalPoints = 0;
    }

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;

        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    void Update()
    {
        score.text = ScoreKeeper.score.ToString();
        float currentHealth = 0;
        if (player != null)
        {
            currentHealth = player.health;
        }
        health.text = currentHealth.ToString();

        if (killStreakTimerBarTime >= 0)
        {
            killStreakTimerBarTime -= Time.deltaTime;
            killStreakTimerBar.transform.localScale = new Vector3((killStreakTimerBarTime / 2f), 1, 1);
        }
    }

    void OnNewWave(int waveNumber)
    {
        string[] numbers = {"One", "Two", "Three", "Four", "Five", "Six"};
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    public void RefreshAmmo(int bullets)
    {
        if (bullets == -1)
        {
            ammo.text = "Reloading";
        }
        else
        {
            ammo.text = bullets.ToString();
        }
    }

    public void KillStreakBanner(int streakCount)
    {
        if (!killStreakBannerToggle)
        {
            killStreakBannerToggle = true;
            killStreak.text = streakCount.ToString();
            
            StartCoroutine("AnimateKillStreakBanner");
        }
        else
        {
            killStreak.text = streakCount.ToString();
            killStreakEndDelayTime = Time.time + 2f;
        }

        killStreakTimerBarTime = 2f;
    }

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.95f), 1));
        gameOverScoreUIText.text = score.text;
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    IEnumerator AnimateKillStreakBanner()
    {
        float animatePercent = 0;
        float speed = 5f;
        float delayTime = 1.8f;
        int direction = 1;

        killStreakEndDelayTime = Time.time + 1 / speed + delayTime;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * direction;

            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > killStreakEndDelayTime)
                {
                    direction = -1;
                }
            }

            killStreakBanner.anchoredPosition = Vector2.left * Mathf.Lerp(-330, 30, animatePercent);
            yield return null;
        }
        killStreakBannerToggle = false;
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float animatePercent = 0;
        float speed = 3f;
        float delayTime = 2f;
        int direction = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * direction;

            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    direction = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-170, 45, animatePercent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
