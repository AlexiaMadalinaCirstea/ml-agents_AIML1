using UnityEngine;
using TMPro;

public class IndividualTimer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the TextMeshProUGUI component for displaying time.")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [Tooltip("Should the timer start automatically?")]
    public bool startAutomatically = true;

    // Internal timer variable to track elapsed time
    private float elapsedTime = 0f;

    // Flag to control timer updates
    private bool isPaused = false;

    /// <summary>
    /// Initializes the timer based on the startAutomatically flag.
    /// </summary>
    private void Start()
    {
        if (startAutomatically)
        {
            isPaused = false;
        }
        else
        {
            isPaused = true;
        }

        // Initialize timer display
        UpdateTimerText();
    }

    /// <summary>
    /// Updates the timer each frame.
    /// </summary>
    private void Update()
    {
        if (timerText != null && !isPaused)
        {
            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Update the timer text with one decimal precision
            UpdateTimerText();
        }
    }

    /// <summary>
    /// Updates the timer text.
    /// </summary>
    private void UpdateTimerText()
    {
        timerText.text = $"Time: {elapsedTime:F1}s";
    }

    /// <summary>
    /// Pauses the timer.
    /// </summary>
    public void PauseTimer()
    {
        isPaused = true;
    }

    /// <summary>
    /// Resumes the timer.
    /// </summary>
    public void ResumeTimer()
    {
        isPaused = false;
    }
}
