// ProceduralPunchAnimator.cs
using System.Collections;
using UnityEngine;

/// <summary>
/// Procedural punch animator with:
/// - 2-bone analytic IK (shoulder-elbow-hand)
/// - pole vector support
/// - spring-based snap/overshoot for punch motion
/// - Perlin-noise based idle wiggle
/// 
/// Usage:
/// Call PlayPunch(handTransform, targetWorldPos, shoulder, elbow, ...).
/// Shoulder and elbow are optional; if provided IK will be applied.
/// </summary>
public class ProceduralPunchAnimator : MonoBehaviour
{
    [Header("Spring Settings")]
    [Tooltip("Higher = snappier (damping computed as critical)")]
    public float springStiffness = 200f;
    [Tooltip("How much overshoot beyond target (meters)")]
    public float punchOvershoot = 0.15f;

    [Header("Idle Noise")]
    public float idleNoiseAmplitude = 0.03f;
    public float idleNoiseFrequency = 0.5f;

    // Small internal spring state (used by PlayPunch when requested)
    private Vector3 springVelocity = Vector3.zero;

    void Update()
    {
        // nothing here by default; idle noise is applied per-hand in routines
    }

    /// <summary>
    /// Start a procedural punch. Returns the running coroutine.
    /// </summary>
    public Coroutine PlayPunch(Transform hand,
                               Vector3 targetWorld,
                               Transform shoulder = null,
                               Transform elbow = null,
                               Vector3? poleVector = null,
                               float durationWind = 0.5f,
                               float durationPunch = 0.18f,
                               float punchPower = 0.15f)
    {
        return StartCoroutine(PunchRoutine(hand, targetWorld, shoulder, elbow, poleVector, durationWind, durationPunch, punchPower));
    }

    IEnumerator PunchRoutine(Transform hand,
                             Vector3 targetWorld,
                             Transform shoulder,
                             Transform elbow,
                             Vector3? poleVector,
                             float durationWind,
                             float durationPunch,
                             float punchPower)
    {
        // Record originals
        Transform handParent = hand.parent;
        Vector3 origLocalPos = hand.localPosition;
        Quaternion origLocalRot = hand.localRotation;

        // If there is a shoulder, capture its original rot so we can restore later
        Quaternion origShoulderRot = shoulder != null ? shoulder.localRotation : Quaternion.identity;
        Quaternion origElbowRot = elbow != null ? elbow.localRotation : Quaternion.identity;

        // Convert target to local space of hand's parent so we can move localPosition if needed
        Vector3 targetLocal = handParent == null ? targetWorld : handParent.InverseTransformPoint(targetWorld);

        // WINDUP: subtle pull-back (local)
        Vector3 windStart = origLocalPos;
        Vector3 windEnd = origLocalPos + (origLocalPos - targetLocal).normalized * 0.12f;

        float timer = 0f;
        while (timer < durationWind)
        {
            timer += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, timer / durationWind);
            hand.localPosition = Vector3.Lerp(windStart, windEnd, p) + IdleNoiseOffset();
            yield return null;
        }

        // PUNCH: we will drive the hand toward target using a spring for snappy feeling.
        // Compute overshoot target in world space
        Vector3 dir = (targetWorld - (handParent == null ? hand.position : handParent.TransformPoint(origLocalPos))).normalized;
        Vector3 overshootWorld = targetWorld + dir * punchPower;

        // For analytic IK computations we need root and lengths
        Vector3 shoulderWorld = shoulder != null ? shoulder.position : (handParent == null ? hand.position : handParent.TransformPoint(origLocalPos));
        Vector3 elbowWorld = elbow != null ? elbow.position : (shoulderWorld + (overshootWorld - shoulderWorld).normalized * 0.4f);

        float upperLen = (elbowWorld - shoulderWorld).magnitude;
        float lowerLen = (hand.position - elbowWorld).magnitude;

        // spring params
        float k = Mathf.Max(1f, springStiffness);
        // critical damping: c = 2 * sqrt(k)
        Vector3 velocity = Vector3.zero;
        Vector3 current = handParent == null ? hand.position : handParent.TransformPoint(origLocalPos);

        float punchTimer = 0f;
        while (punchTimer < durationPunch)
        {
            punchTimer += Time.deltaTime;
            float dt = Time.deltaTime;

            // spring integration toward overshootWorld (critically damped-ish)
            // using simple damped spring: v += (k*(target-current)) * dt; v *= dampingFactor
            // For stability calculate dampingFactor approximate to critical: damping = 2*sqrt(k)
            float damping = 2f * Mathf.Sqrt(k);
            Vector3 accel = k * (overshootWorld - current);
            velocity += accel * dt;
            velocity -= velocity * damping * dt * 0.02f; // small damping scaling

            current += velocity * dt;

            // If IK targets provided, solve two-bone IK for shoulder/elbow/hand to place hand at current
            if (shoulder != null && elbow != null)
            {
                SolveTwoBoneIK(shoulder, elbow, hand, current, poleVector ?? (shoulder.forward), upperLen, lowerLen);
            }
            else
            {
                // move hand in parent-local space
                if (handParent != null)
                {
                    hand.localPosition = handParent.InverseTransformPoint(current) + IdleNoiseOffset();
                }
                else
                {
                    hand.position = current + IdleNoiseOffset();
                }
            }

            yield return null;
        }

        // Small impact pause
        yield return new WaitForSeconds(0.04f);

        // RETRACT to original using smooth damp from current local to origLocalPos
        float retractDur = 0.18f;
        float rT = 0f;
        Vector3 startPosLocal = hand.localPosition;
        while (rT < retractDur)
        {
            rT += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, rT / retractDur);
            if (handParent != null)
                hand.localPosition = Vector3.Lerp(startPosLocal, origLocalPos, p) + IdleNoiseOffset();
            else
                hand.position = Vector3.Lerp(hand.position, handParent.TransformPoint(origLocalPos), p) + IdleNoiseOffset();

            // restore rotations gradually
            if (shoulder != null) shoulder.localRotation = Quaternion.Slerp(shoulder.localRotation, origShoulderRot, p);
            if (elbow != null) elbow.localRotation = Quaternion.Slerp(elbow.localRotation, origElbowRot, p);

            yield return null;
        }

        // final restore
        hand.localPosition = origLocalPos;
        hand.localRotation = origLocalRot;
        if (shoulder != null) shoulder.localRotation = origShoulderRot;
        if (elbow != null) elbow.localRotation = origElbowRot;
    }

    Vector3 IdleNoiseOffset()
    {
        float t = Time.time * idleNoiseFrequency;
        float x = (Mathf.PerlinNoise(t, 0f) - 0.5f) * 2f * idleNoiseAmplitude;
        float y = (Mathf.PerlinNoise(0f, t) - 0.5f) * 2f * idleNoiseAmplitude;
        float z = (Mathf.PerlinNoise(t + 10f, t + 10f) - 0.5f) * 2f * idleNoiseAmplitude;
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Analytic 2-bone IK solver (shoulder -> elbow -> hand).
    /// Places hand at targetWorld and positions/rotates shoulder & elbow to match.
    /// poleVector controls elbow bend direction.
    /// upperLen & lowerLen are lengths of bone segments (from initial pose).
    /// </summary>
    void SolveTwoBoneIK(Transform shoulder, Transform elbow, Transform hand, Vector3 targetWorld, Vector3 poleVector, float upperLen, float lowerLen)
    {
        Vector3 rootPos = shoulder.position;

        // direction to target
        Vector3 toTarget = targetWorld - rootPos;
        float dist = toTarget.magnitude;
        dist = Mathf.Max(Mathf.Epsilon, dist);

        // clamp reach
        float maxReach = upperLen + lowerLen * 0.999f;
        float minReach = Mathf.Abs(upperLen - lowerLen) + 0.001f;
        float clampedDist = Mathf.Clamp(dist, minReach, maxReach);

        // Law of cosines to get angle at shoulder
        float cosAngle0 = (upperLen * upperLen + clampedDist * clampedDist - lowerLen * lowerLen) / (2f * upperLen * clampedDist);
        cosAngle0 = Mathf.Clamp(cosAngle0, -1f, 1f);
        float angle0 = Mathf.Acos(cosAngle0); // angle between upper bone and direction to target

        // direction basis
        Vector3 forward = toTarget.normalized;
        Vector3 poleDir = (poleVector - rootPos).normalized;
        // build orthonormal frame: forward, right, up (right is perpendicular to forward & poleDir)
        Vector3 right = Vector3.Cross(forward, poleDir).normalized;
        if (right.sqrMagnitude < 0.0001f) // fallback
            right = Vector3.Cross(forward, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, forward).normalized;

        // point for elbow (in world)
        // rotate forward by angle0 toward up to find elbow direction
        Quaternion rot = Quaternion.AngleAxis(Mathf.Rad2Deg * angle0, right);
        Vector3 elbowDir = rot * forward;
        Vector3 elbowPos = rootPos + elbowDir * upperLen;

        // To ensure the elbow bends toward the pole vector, project pole onto plane and adjust sign
        Vector3 poleProjected = Vector3.ProjectOnPlane(poleVector - rootPos, forward).normalized;
        float sign = Mathf.Sign(Vector3.Dot(poleProjected, up));
        elbowPos = rootPos + (Quaternion.AngleAxis(Mathf.Rad2Deg * angle0 * sign, right) * forward) * upperLen;

        // place elbow and hand
        elbow.position = elbowPos;
        hand.position = targetWorld;

        // rotate shoulder to look at elbow
        shoulder.rotation = Quaternion.LookRotation((elbow.position - shoulder.position).normalized, poleProjected);
        // rotate elbow to look at hand
        elbow.rotation = Quaternion.LookRotation((hand.position - elbow.position).normalized, poleProjected);
    }
}
