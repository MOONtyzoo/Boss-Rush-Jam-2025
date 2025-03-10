using UnityEngine;
using UnityEngine.UI;

public class SpeedRunTimer : MonoBehaviour
{
    public static SpeedRunTimer Instance;

    private float timer = 0f;
    private bool isRunning = false;

    public Text timerText;

    private void Awake()
    {
        //Singleton so that we can use in multiple levels and not break the timer
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetTimer()
    {
        return timer;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer % 60F);
            int milliseconds = Mathf.FloorToInt((timer * 100F) % 100F);
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
    }
}