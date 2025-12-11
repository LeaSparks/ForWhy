using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class MenuController : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 200f;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    [Header("Interaction Settings")]
    public float maxInteractionDistance = 10f;
    public LayerMask physicsInteractLayers = ~0;

    [Header("Cursor Settings")]
    public bool startLocked = true;

    [Header("Reticle")]
    public Image reticleImage;
    public Color reticleLockedColor = Color.white;
    public Color reticleHoverColor = Color.yellow;
    public float reticleSize = 8f;

    private float yaw;
    private float pitch;
    private Camera cam;
    private bool cursorLocked;

    private GameObject lastUIHover; 
    private Graphic lastUIGraphic;  
    private Color originalUIColor;

    void Awake()
    {
        cam = GetComponent<Camera>();
        yaw = transform.localEulerAngles.y;
        pitch = transform.localEulerAngles.x;

        if (reticleImage != null)
        {
            reticleImage.rectTransform.sizeDelta = new Vector2(reticleSize, reticleSize);
            reticleImage.color = reticleLockedColor;
        }
    }

    void Start()
    {
        SetCursorLock(startLocked);
    }

    void Update()
    {
        HandleLockToggle();

        if (cursorLocked)
        {
            UpdateLook();
            UpdateHover(); 
        }
        else
        {
            ClearHover();
        }

        if (Input.GetMouseButtonDown(0))
            TryInteract();
    }
    

    private void HandleLockToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursorLock(!cursorLocked);
    }

    private void SetCursorLock(bool locked)
    {
        cursorLocked = locked;

        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;

        if (reticleImage != null)
            reticleImage.enabled = locked;

        if (!locked)
            ClearHover();
    }


  

    private void UpdateLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localEulerAngles = new Vector3(pitch, yaw, 0f);
    }
    

    private void UpdateHover()
    {
        Vector2 pos = new Vector2(Screen.width / 2f, Screen.height / 2f);

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = pos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        if (results.Count > 0)
        {
            GameObject hovered = results[0].gameObject;

            if (hovered != lastUIHover)
            {
                ClearHover();

                lastUIHover = hovered;
                lastUIGraphic = hovered.GetComponent<Graphic>();

                if (lastUIGraphic != null)
                {
                    originalUIColor = lastUIGraphic.color;
                    lastUIGraphic.color = reticleHoverColor;
                }

                if (reticleImage != null)
                    reticleImage.color = reticleHoverColor;
            }
        }
        else
        {
            ClearHover();
        }
    }

    private void ClearHover()
    {
        if (lastUIGraphic != null)
        {
            lastUIGraphic.color = originalUIColor;
        }

        if (reticleImage != null)
            reticleImage.color = reticleLockedColor;

        lastUIHover = null;
        lastUIGraphic = null;
    }

 

    private void TryInteract()
    {
        Vector2 pointerPos = cursorLocked
            ? new Vector2(Screen.width / 2f, Screen.height / 2f)
            : (Vector2)Input.mousePosition;


        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = pointerPos,
            button = PointerEventData.InputButton.Left
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        if (results.Count > 0)
        {
            GameObject ui = results[0].gameObject;

            ExecuteEvents.ExecuteHierarchy(ui, data, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.ExecuteHierarchy(ui, data, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.ExecuteHierarchy(ui, data, ExecuteEvents.pointerUpHandler);

            return;
        }

   
        Ray ray = cam.ScreenPointToRay(pointerPos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxInteractionDistance, physicsInteractLayers))
        {
            GameObject target = hit.collider.gameObject;
            Debug.Log("Interacted with: " + target.name);

            var interactable = target.GetComponent<IInteractable>();
            if (interactable != null)
                interactable.OnInteract();
            else
                target.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
        }
    }
}

public interface IInteractable
{
    void OnInteract();
}
