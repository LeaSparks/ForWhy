using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public enum PatternId { P1, P2, P3, P4, P5 }

    [Header("Links")]
    public SkeletonBossAttacks attacks; 

    [Header("General Settings")]
    public float globalCooldown = 0.15f; // small gap between attacks

    [Header("Pattern Timings")]
    public float pattern1Delay = 2f;
    public float pattern2Delay = 3f;
    public float pattern3Delay = 2.5f;
    public float pattern4Delay = 3.5f;
    public float pattern5Delay = 1.2f;

    private Coroutine patternRoutine;

    void Awake()
    {
        if (attacks == null)
            Debug.LogWarning("AttackManager: attacks reference not assigned.");
    }

    public void StartPattern(PatternId id)
    {
        StopCurrentPattern();
        patternRoutine = StartCoroutine(PatternLoop(id));
    }

    public void StopCurrentPattern()
    {
        if (patternRoutine != null)
        {
            StopCoroutine(patternRoutine);
            patternRoutine = null;
        }
    }

    IEnumerator PatternLoop(PatternId id)
    {
        while (true)
        {
            switch (id)
            {
                case PatternId.P1:
                    attacks.TelegraphedPunch();
                    yield return new WaitForSeconds(pattern1Delay);
                    break;

                case PatternId.P2:
                    attacks.DoubleHandSlam();
                    yield return new WaitForSeconds(pattern2Delay);
                    break;

                case PatternId.P3:
                    attacks.SpikeBurst();
                    yield return new WaitForSeconds(pattern3Delay);
                    break;

                case PatternId.P4:
                    attacks.LaserSweep();
                    yield return new WaitForSeconds(pattern4Delay);
                    break;

                case PatternId.P5:
                    attacks.TelegraphedPunch();
                    yield return new WaitForSeconds(pattern5Delay);

                    attacks.SpikeBurst();
                    yield return new WaitForSeconds(pattern5Delay);

                    attacks.LaserBlast();
                    yield return new WaitForSeconds(pattern5Delay);
                    break;
            }

            yield return new WaitForSeconds(globalCooldown);
        }
    }
}
