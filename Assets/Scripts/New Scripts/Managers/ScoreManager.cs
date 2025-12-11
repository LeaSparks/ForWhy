using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text survivalTimeText;   
    public TMP_Text totalBombsText;  
    private PlayerStats playerStats;    
    private int totalBombsUsed = 0;

    private bool isGameOver = false;

    void OnEnable()
    {
        Health.OnPlayerDeath += OnPlayerDeath;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
    }

    void OnDisable()
    {
        Health.OnPlayerDeath -= OnPlayerDeath;
    }

    void Update()
    {
        if (!isGameOver && playerStats != null)
        {
           
            if (survivalTimeText != null)
            {
                survivalTimeText.text = FormatTime(playerStats.playTime);
            }

            // Bomb count
            if (totalBombsText != null)
            {
                totalBombsText.text = totalBombsUsed.ToString();
            }
        }
    }

    public void OnPlayerDeath()
    {
        isGameOver = true;
        UpdateDeathCanvas();
    }

    private void UpdateDeathCanvas()
    {
        if (playerStats != null && survivalTimeText != null)
        {
            // Final formatted time
            survivalTimeText.text = FormatTime(playerStats.playTime);
        }

        if (totalBombsText != null)
            totalBombsText.text = totalBombsUsed.ToString();
    }

    public void IncrementBombsUsed()
    {
        totalBombsUsed++;
        Debug.Log("Bomb count updated: " + totalBombsUsed);
    }

    // --- Time Format MM:SS:MS (capped at 99:99:99) ---
    private string FormatTime(float time)
    {
        int totalMilliseconds = Mathf.FloorToInt(time * 1000);

        int minutes = (totalMilliseconds / 60000) % 100;
        int seconds = (totalMilliseconds / 1000) % 60;
        int milliseconds = (totalMilliseconds / 10) % 100;

        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}
