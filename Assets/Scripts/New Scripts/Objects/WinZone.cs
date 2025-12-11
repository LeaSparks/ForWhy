using UnityEngine;

public class WinZone : MonoBehaviour
{
    public WinManager winManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("WinZone: Player entered!");
            winManager.HandleWin();
        }
    }
}