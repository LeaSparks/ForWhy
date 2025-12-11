using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float playTime { get; private set; }
    public float movementDistanceFeet { get; private set; }

    private Vector3 lastPosition;
    private bool isTracking = true;

    private const float metersToFeet = 3.28084f;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (!isTracking) return;

        playTime += Time.deltaTime;

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        movementDistanceFeet += distanceMoved * metersToFeet;

        lastPosition = transform.position;
    }

    public void StopTracking()
    {
        isTracking = false;
    }
}