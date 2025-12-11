using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SkeletonBossAttacks : MonoBehaviour
{
    [Header("Hands & Animation Points")] // Unused
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftPunchPoint;
    public Transform rightPunchPoint;

    [Header("Shoulder & Elbow left")] // Unused
    public Transform leftShoulder;
    public Transform leftElbow;

    [Header("Shoulder & Elbow right")] // Unused
    public Transform rightShoulder;
    public Transform rightElbow;

    [Header("Procedural Punch Animator")] // Does not work, & is from a tutorial
    public ProceduralPunchAnimator punchAnimatorPrefab; 
    private ProceduralPunchAnimator punchAnimatorInstance;

    [Header("Laser")] // Kinda works!!!!!
    public Transform laserOrigin;
    public LineRenderer laserLine;
    public LayerMask laserFloorMask;
    public LaserDamage laserDamage; 

    [Header("Spikes")] // Does not work :< I wanted it to work so bad
    public GameObject spikePrefab;
    public GameObject warningCirclePrefab;
    public float spikeRadius = 6f;
    public int spikeCount = 8;
    public float telegraphDuration = 1f;

    [Header("Telegraphs (Punches + Slams)")] // Unused and also from a tutorial
    public GameObject leftTelegraphCircle;
    public GameObject rightTelegraphCircle;
    public float telegraphTime = 0.7f;

    [Header("Telegraph Visual Settings")] // Unused and same as above
    public Material telegraphPulseMaterial; 
    public float telegraphPulseStrength = 2f;

    void Awake()
    {
        if (punchAnimatorPrefab != null)
            punchAnimatorInstance = punchAnimatorPrefab;
        else
            punchAnimatorInstance = gameObject.AddComponent<ProceduralPunchAnimator>();

        if (laserLine != null)
            laserLine.enabled = false;
    }

    // ------------------------------------------------------------------------------------------------------------
    // TELEGRAPHED PUNCH | Does not work, tried using an animator I found online but I should have just used Mixamo
    // It was intended to either punch with the right or left arm, with the angle of the punch slightly changing
    // depending on the character's position.
    // ------------------------------------------------------------------------------------------------------------
    public void TelegraphedPunch()
    {
        int side = Random.Range(0, 2);
        if (side == 0)
            StartCoroutine(PunchSequence(leftHand, leftPunchPoint, leftTelegraphCircle, leftShoulder, leftElbow));
        else
            StartCoroutine(PunchSequence(rightHand, rightPunchPoint, rightTelegraphCircle, rightShoulder, rightElbow));
    }

    IEnumerator PunchSequence(Transform hand, Transform punchPoint, GameObject telegraph, Transform shoulder, Transform elbow)
    {
        if (telegraph != null)
        {
            StartCoroutine(TelegraphWindup(telegraph, telegraphTime));
        }

        yield return new WaitForSeconds(telegraphTime);

        Vector3 targetWorld = punchPoint.position;

        Vector3 pole = (shoulder != null) ? (shoulder.position + shoulder.right) : (transform.position + transform.right);

        if (punchAnimatorInstance != null)
        {
            punchAnimatorInstance.PlayPunch(hand, targetWorld, shoulder, elbow, pole, durationWind: telegraphTime * 0.55f, durationPunch: 0.18f, punchPower: 0.18f);
        }
        else
        {
            StartCoroutine(BasicFallbackPunch(hand, punchPoint));
        }
    }

    IEnumerator BasicFallbackPunch(Transform hand, Transform punchPoint)
    {
        Vector3 start = hand.localPosition;
        Vector3 end = punchPoint.localPosition;

        float t = 0f;
        float dur = 0.18f;
        while (t < dur)
        {
            t += Time.deltaTime;
            hand.localPosition = Vector3.Lerp(start, end, t / dur);
            yield return null;
        }

        yield return new WaitForSeconds(0.04f);

        t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            hand.localPosition = Vector3.Lerp(end, start, t / dur);
            yield return null;
        }

        hand.localPosition = start;
    }

    // -----------------------------------------------------------------------------------------------------------------------------
    // Double Hand Slam | Doesn't work at all, almost this entire attack is just copied from github and I have no idea how it works
    // The intended attack here is bestvisualized in rhe hand slam graphic I made {show graphic Handslam}
    // -----------------------------------------------------------------------------------------------------------------------------
    public void DoubleHandSlam()
    {
        StartCoroutine(DoubleSlamRoutine());
    }

    IEnumerator DoubleSlamRoutine()
    {
        if (leftTelegraphCircle != null) leftTelegraphCircle.SetActive(true);
        if (rightTelegraphCircle != null) rightTelegraphCircle.SetActive(true);

        yield return new WaitForSeconds(telegraphTime);

        if (leftTelegraphCircle != null) leftTelegraphCircle.SetActive(false);
        if (rightTelegraphCircle != null) rightTelegraphCircle.SetActive(false);

        Vector3 offset = Vector3.down * 0.6f;
        leftHand.localPosition += offset;
        rightHand.localPosition += offset;

        yield return new WaitForSeconds(0.2f);

        leftHand.localPosition -= offset;
        rightHand.localPosition -= offset;

    }

    // -------------------------------------------------------------------------
    // Spike Burst | Doesn't work sadly, I was really excited for this one
    // -------------------------------------------------------------------------
    public void SpikeBurst()
    {
        StartCoroutine(SpikeRoutine());
    }

    IEnumerator SpikeRoutine()
    {
        for (int i = 0; i < spikeCount; i++)
        {
            float angle = i * (360f / spikeCount);
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * spikeRadius;

            GameObject warn = Instantiate(warningCirclePrefab, pos, Quaternion.Euler(90f, 0f, 0f));
            StartCoroutine(ScaleWarning(warn));
            StartCoroutine(SpawnSpikeDelayed(pos, warn));
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator SpawnSpikeDelayed(Vector3 pos, GameObject warn)
    {
        yield return new WaitForSeconds(telegraphDuration);
        if (warn != null) Destroy(warn);
        Instantiate(spikePrefab, pos, Quaternion.identity);
    }

    IEnumerator ScaleWarning(GameObject warn)
    {
        float t = 0f;
        Vector3 start = Vector3.zero;
        Vector3 end = new Vector3(2f, 2f, 2f);

        while (t < telegraphDuration)
        {
            t += Time.deltaTime;
            if (warn != null) warn.transform.localScale = Vector3.Lerp(start, end, t / telegraphDuration);
            yield return null;
        }
    }

    // -------------------------------------------------
    // Laser Sweep | Works Kinda
    // -------------------------------------------------
    public void LaserSweep()
    {
        StartCoroutine(LaserSweepRoutine());
    }

    IEnumerator LaserSweepRoutine()
    {
        yield return new WaitForSeconds(telegraphTime);

        float sweepTime = 2f;
        float t = 0f;

        if (laserLine != null) laserLine.enabled = true;
        Quaternion originalRot = laserOrigin.localRotation;

        while (t < sweepTime)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(-80f, 80f, t / sweepTime);
            laserOrigin.localRotation = Quaternion.Euler(0, angle, 0);

            Vector3 start = laserOrigin.position;
            Vector3 dir = laserOrigin.forward;

            if (Physics.Raycast(start, dir, out RaycastHit hit, 200f, laserFloorMask))
            {
                if (laserLine != null)
                {
                    laserLine.SetPosition(0, start);
                    laserLine.SetPosition(1, hit.point);
                }

                if (laserDamage != null)
                    laserDamage.ApplyLaserDamage(hit.point);
            }
            else
            {
                Vector3 end = start + dir * 30f;
                if (laserLine != null)
                {
                    laserLine.SetPosition(0, start);
                    laserLine.SetPosition(1, end);
                }

                if (laserDamage != null)
                    laserDamage.StopLaser();
            }

            yield return null;
        }

        if (laserLine != null) laserLine.enabled = false;
        if (laserOrigin != null) laserOrigin.localRotation = originalRot;
        if (laserDamage != null) laserDamage.StopLaser();
    }

    // -------------------------------------------------
    // Laser Blast | Works kinda
    // -------------------------------------------------
    public void LaserBlast()
    {
        StartCoroutine(LaserBlastRoutine());
    }

    IEnumerator LaserBlastRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        if (laserLine != null) laserLine.enabled = true;

        Vector3 start = laserOrigin.position;
        Vector3 dir = laserOrigin.forward;

        if (Physics.Raycast(start, dir, out RaycastHit hit, 200f, laserFloorMask))
        {
            if (laserLine != null)
            {
                laserLine.SetPosition(0, start);
                laserLine.SetPosition(1, hit.point);
            }

            if (laserDamage != null)
                laserDamage.ApplyLaserDamage(hit.point);
        }
        else
        {
            Vector3 end = start + dir * 25f;
            if (laserLine != null)
            {
                laserLine.SetPosition(0, start);
                laserLine.SetPosition(1, end);
            }
        }

        yield return new WaitForSeconds(0.25f);

        if (laserLine != null) laserLine.enabled = false;
        if (laserDamage != null) laserDamage.StopLaser();
    }

    // -------------------------------------------------
    // TELEGRAPH WINDUP | Doesn't work
    // -------------------------------------------------
    IEnumerator TelegraphWindup(GameObject telegraph, float duration)
    {
        if (telegraph == null) yield break;
        float t = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * 1.8f;

        Material mat = null;
        Color baseColor = Color.black;
        if (telegraphPulseMaterial != null)
        {
            mat = telegraphPulseMaterial;
            if (mat.HasProperty("_EmissionColor"))
                baseColor = mat.GetColor("_EmissionColor");
        }

        telegraph.transform.localScale = startScale;
        telegraph.SetActive(true);

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duration);
            telegraph.transform.localScale = Vector3.Lerp(startScale, endScale, p);

            if (mat != null)
            {
                Color emit = baseColor * (1f + p * telegraphPulseStrength);
                mat.SetColor("_EmissionColor", emit);
            }

            yield return null;
        }

        yield return null;
    }

#if UNITY_EDITOR //This is cool, the internet taught me how to do thos
    // Inspector debug buttons
    [CustomEditor(typeof(SkeletonBossAttacks))]
    public class SkeletonBossAttacksEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SkeletonBossAttacks a = (SkeletonBossAttacks)target;

            GUILayout.Space(17);
            GUILayout.Label("Debug Attack Buttons", EditorStyles.boldLabel);

            if (GUILayout.Button("Test: Punch"))
                a.TelegraphedPunch();

            if (GUILayout.Button("Test: Double Slam"))
                a.DoubleHandSlam();

            if (GUILayout.Button("Test: Spike Burst"))
                a.SpikeBurst();

            if (GUILayout.Button("Test: Laser Sweep"))
                a.LaserSweep();

            if (GUILayout.Button("Test: Laser Blast"))
                a.LaserBlast();
        }
    }
#endif
}
