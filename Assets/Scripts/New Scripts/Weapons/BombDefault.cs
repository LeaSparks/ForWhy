using UnityEngine;
using TMPro; 
using UnityEngine.UI;  

public class BombLauncher : WeaponBase
{
    [Header("Bomb Launcher Settings")]
    public GameObject bombPrefab;       
    public Transform spawnPoint;        
    public int maxBombs = 3;            
    public PlayerController playerController; 

    private int currentBombCount = 0;    

    public TMP_Text bombCountText;     
    public Slider bombCountSlider;      
    private ScoreManager scoreManager; 

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    public override void UpdateWeapon()
    {
        if (isEquipped)
        {
            if (Input.GetKeyDown(KeyCode.Space) && currentBombCount < maxBombs)
            {
                PlaceBomb();

                // Increment bombs used in the ScoreManager
                if (scoreManager != null)
                {
                    scoreManager.IncrementBombsUsed();  
                }
            }

            UpdateBombCountUI();
        }
    }

    private void PlaceBomb()
    {
        playerController.FreezeMovement();

        GameObject bomb = Instantiate(bombPrefab, spawnPoint.position, spawnPoint.rotation);

        currentBombCount++;

        Debug.Log("Bomb placed! Current bomb count: " + currentBombCount);

        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            bombScript.OnBombDestroyed += BombDestroyed;  
        }
    }

    private void BombDestroyed()
    {
        currentBombCount--;  // Decrease the bomb count when a bomb is destroyed

        Debug.Log("Bomb destroyed! Current bomb count: " + currentBombCount);
    }

    private void UpdateBombCountUI()
    {
        if (bombCountText != null)
        {
            bombCountText.text = "Bombs: " + currentBombCount + "/" + maxBombs;
        }

        if (bombCountSlider != null)
        {
            bombCountSlider.value = currentBombCount;
            bombCountSlider.maxValue = maxBombs;  
        }
    }
}
