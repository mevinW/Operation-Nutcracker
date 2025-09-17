using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TwoColorCountdown : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float duration = 30f;

    [Header("UI References")]
    [Tooltip("Red ring for elapsed time")]
    [SerializeField] private Image redRing;
    [Tooltip("Green ring for remaining time")]
    [SerializeField] private Image greenRing;
    [Tooltip("Text showing seconds remaining")]
    [SerializeField] private Text timerText;

    [Header("On Timer End")]
    public UnityEvent onTimerEnd;

    private float remaining;
    private bool running;

    void Awake()
    {
        remaining = duration;
        UpdateUI();
    }

    void Start()
    {
        StartTimer();  // remove if you want manual start
    }

    void Update()
    {
        if (!running) return;

        remaining = Mathf.Max(remaining - Time.deltaTime, 0f);
        UpdateUI();

        if (remaining <= 0f)
        {
            running = false;
            onTimerEnd?.Invoke();
        }
    }

    public void StartTimer()
    {
        remaining = duration;
        running = true;
        UpdateUI();
    }

    public void StopTimer()
    {
        running = false;
    }

    private void UpdateUI()
    {
        float pctRemaining = remaining / duration;      // 1→0
        float pctElapsed = 1f - pctRemaining;         // 0→1

        // Text
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(remaining).ToString();

        // Green (remaining)
        if (greenRing != null)
            greenRing.fillAmount = pctRemaining;

        // Red (elapsed)
        if (redRing != null)
            redRing.fillAmount = pctElapsed;
    }
}
