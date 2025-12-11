using UnityEngine;
using TMPro;

public class WinManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject gameCanvas;  
    public GameObject winCanvas;    

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera winCamera;

    [Header("Player Stats")]
    public PlayerStats playerStats; 

    [Header("Win UI")]
    public TMP_Text winTimeText;
    public TMP_Text winDistanceText;

    private bool hasWon = false;

    void Start()
    {
        if (winCanvas != null)
            winCanvas.SetActive(false);

        if (winCamera != null)
            winCamera.enabled = false;

        if (mainCamera != null)
            mainCamera.enabled = true;
    }

    public void HandleWin()
    {
        if (hasWon) return; 
        hasWon = true;

        Debug.Log("WinManager: Player has reached the win zone!");

        if (playerStats != null)
            playerStats.StopTracking();

        if (gameCanvas != null)
            gameCanvas.SetActive(false);

        if (winCanvas != null)
            winCanvas.SetActive(true);

        if (mainCamera != null)
            mainCamera.enabled = false;

        if (winCamera != null)
            winCamera.enabled = true;

        if (playerStats != null)
        {
            if (winTimeText != null)
                winTimeText.text = FormatTime(playerStats.playTime);

            if (winDistanceText != null)
                winDistanceText.text = playerStats.movementDistanceFeet.ToString("F1") + " ft";
        }
    }

    // Formats time as MM:SS:MS (capped at 99:99:99)
    private string FormatTime(float time)
    {
        int totalMilliseconds = Mathf.FloorToInt(time * 1000);

        int minutes = (totalMilliseconds / 60000) % 100;
        int seconds = (totalMilliseconds / 1000) % 60;
        int milliseconds = (totalMilliseconds / 10) % 100;

        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}
