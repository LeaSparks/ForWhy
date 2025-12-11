using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 300;
    public int currentHealth;
    [Tooltip("How much HP lost to progress to the next pattern")]
    public int thresholdStep = 60;
    private int nextThreshold;

    [Header("Components")]
    public AttackManager attackManager; 

    [Header("State")]
    public BossState currentState = BossState.Pattern1;
    private bool isChangingState = false;

    void Awake()
    {
        currentHealth = maxHealth;
        nextThreshold = Mathf.Max(0, maxHealth - thresholdStep);
    }

    void Start()
    {
        EnterState(currentState);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        if (!isChangingState && currentHealth <= nextThreshold)
        {
            StartCoroutine(ChangeToNextState());
            nextThreshold = Mathf.Max(0, nextThreshold - thresholdStep);
        }
    }

    IEnumerator ChangeToNextState()
    {
        isChangingState = true;

      
        yield return new WaitForSeconds(0.6f);

        currentState++;
        if ((int)currentState > (int)BossState.Pattern5)
            currentState = BossState.Pattern5;

        EnterState(currentState);

        isChangingState = false;
    }

    void EnterState(BossState state)
    {
        if (attackManager == null)
        {
            Debug.LogWarning("AttackManager not assigned to BossController.");
            return;
        }

        switch (state)
        {
            case BossState.Pattern1:
                attackManager.StartPattern(AttackManager.PatternId.P1);
                break;
            case BossState.Pattern2:
                attackManager.StartPattern(AttackManager.PatternId.P2);
                break;
            case BossState.Pattern3:
                attackManager.StartPattern(AttackManager.PatternId.P3);
                break;
            case BossState.Pattern4:
                attackManager.StartPattern(AttackManager.PatternId.P4);
                break;
            case BossState.Pattern5:
                attackManager.StartPattern(AttackManager.PatternId.P5);
                break;
        }
    }
}
