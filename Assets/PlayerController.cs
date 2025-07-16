using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float jumpForce = 5f;
    public float laneDistance = 2f;
    public float slideDuration = 1f;
    public Vector3 slideScale = new Vector3(1f, 0.5f, 1f);

    private int currentLane = 1; // 0 left, 1 middle, 2 right
    private Vector3 targetPosition;
    private bool isJumping = false;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private CharacterController controller;
    private Vector3 originalScale;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalScale = transform.localScale;
        targetPosition = transform.position;
    }

    void Update()
    {
        // Forward movement
        Vector3 move = Vector3.forward * forwardSpeed * Time.deltaTime;
        controller.Move(move);

        // Gravity and jumping
        if (controller.isGrounded)
        {
            if (isJumping) isJumping = false;
        }
        else
        {
            controller.Move(Physics.gravity * Time.deltaTime);
        }

        // Sliding
        if (isSliding)
        {
            slideTimer += Time.deltaTime;
            if (slideTimer >= slideDuration)
            {
                isSliding = false;
                transform.localScale = originalScale;
                slideTimer = 0f;
            }
        }

        // Lane movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);

        // Swipe detection
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 50f && currentLane < 2) // Right
                    {
                        currentLane++;
                        UpdateLane();
                    }
                    else if (delta.x < -50f && currentLane > 0) // Left
                    {
                        currentLane--;
                        UpdateLane();
                    }
                }
                else
                {
                    if (delta.y > 50f && controller.isGrounded) // Up - Jump
                    {
                        controller.Move(Vector3.up * jumpForce);
                        isJumping = true;
                    }
                    else if (delta.y < -50f && !isSliding) // Down - Slide
                    {
                        isSliding = true;
                        transform.localScale = slideScale;
                    }
                }
            }
        }
    }

    void UpdateLane()
    {
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }
}