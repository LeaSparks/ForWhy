using UnityEngine;
using System.Collections;

public class TimeController : MonoBehaviour
{
    private Coroutine timeCoroutine;
    [SerializeField] private KeyCode slowTimeKey = KeyCode.RightShift;
    [SerializeField] private float transitionSpeed = 1f;
    [Range(0.0f, 2.0f)]
    [SerializeField] private float targetSpeed = 0f;



    void Update()
    {
        if (Input.GetKeyDown(slowTimeKey))
        {
            if (timeCoroutine != null)
                StopCoroutine(timeCoroutine);

            if (Mathf.Approximately(Time.timeScale, 1f))
                timeCoroutine = StartCoroutine(ChangeTimeScale(1f, targetSpeed, transitionSpeed)); 
            else
                timeCoroutine = StartCoroutine(ChangeTimeScale(Time.timeScale, 1f, transitionSpeed)); 
        }
    }

    IEnumerator ChangeTimeScale(float start, float end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Time.timeScale = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.unscaledDeltaTime; 
            yield return null;
        }

        Time.timeScale = end;
        Debug.Log($"Time scale set to {end}.");
    }
}