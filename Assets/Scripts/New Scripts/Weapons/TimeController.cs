using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 
using System.Collections;

public class TimeController : MonoBehaviour
{
    private Coroutine timeCoroutine;

    [Header("Time Settings")]
    [SerializeField] private float transitionSpeed = 1f;
    [Range(0.0f, 2.0f)]
    [SerializeField] private float targetSpeed = 0f;
    private bool isSlowingTime = false;

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    public float fuelDrainRate = 15f;    
    public float fuelRegenRate = 10f;     
    public float minFuelToActivate = 5f;  

    [Header("UI")]
    public Slider fuelSlider;              

    [Header("Tint Effect")]
    [SerializeField] private Image screenTintImage;
    [SerializeField] private Color slowTimeTintColor = new Color(0.5f, 0f, 0.5f, 0.5f);

    private InputAction slowTimeAction;

    void OnEnable()
    {
        slowTimeAction = new InputAction("SlowTime", binding: "<Keyboard>/rightShift");
        slowTimeAction.performed += OnSlowTimeInput;
        slowTimeAction.canceled += OnSlowTimeInputReleased;
        slowTimeAction.Enable();
    }

    void OnDisable()
    {
        slowTimeAction.Disable();
    }

    void Start()
    {
        if (screenTintImage != null)
            screenTintImage.color = Color.clear;

        if (fuelSlider != null)
        {
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = currentFuel;
        }
    }

    void Update()
    {
        HandleFuel();
        if (fuelSlider != null)
            fuelSlider.value = currentFuel;
    }

    private void HandleFuel()
    {
        if (isSlowingTime)
        {
            currentFuel -= fuelDrainRate * Time.unscaledDeltaTime;

            if (currentFuel <= 0)
            {
                currentFuel = 0;
                StopSlowTime();
            }
        }
        else
        {
            currentFuel += fuelRegenRate * Time.unscaledDeltaTime;
            if (currentFuel > maxFuel)
                currentFuel = maxFuel;
        }
    }

    private void OnSlowTimeInput(InputAction.CallbackContext context)
    {
        if (currentFuel > minFuelToActivate && !isSlowingTime)
        {
            StartSlowTime();
        }
    }

    private void OnSlowTimeInputReleased(InputAction.CallbackContext context)
    {
        if (isSlowingTime)
        {
            StopSlowTime();
        }
    }

    private void StartSlowTime()
    {
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        timeCoroutine = StartCoroutine(ChangeTimeScale(Time.timeScale, targetSpeed, transitionSpeed));
        isSlowingTime = true;

        if (screenTintImage != null)
            StartCoroutine(ChangeTintColor(screenTintImage.color, slowTimeTintColor, transitionSpeed));
    }

    private void StopSlowTime()
    {
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        timeCoroutine = StartCoroutine(ChangeTimeScale(Time.timeScale, 1f, transitionSpeed));
        isSlowingTime = false;

        if (screenTintImage != null)
            StartCoroutine(ChangeTintColor(screenTintImage.color, Color.clear, transitionSpeed));
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
    }

    IEnumerator ChangeTintColor(Color startColor, Color endColor, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            screenTintImage.color = Color.Lerp(startColor, endColor, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        screenTintImage.color = endColor;
    }
}
