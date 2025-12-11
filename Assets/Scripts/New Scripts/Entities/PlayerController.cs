using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;         
    [SerializeField] private float freezeDuration = 0.5f; 

    private CharacterController controller;
    private Vector3 velocity;

    private bool isGrounded;
    private bool isMovementFrozen = false;  
    private float freezeTimer = 0f;         

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isMovementFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0)
            {
                isMovementFrozen = false;  
            }
        }

        if (!isMovementFrozen)
        {
            HandleMovement();
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;  
        }

        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);
    }

    // Method to freeze movement when placing a bomb
    public void FreezeMovement()
    {
        isMovementFrozen = true;
        freezeTimer = freezeDuration; 
    }
}
