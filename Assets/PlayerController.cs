using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10f; // �������� �������� �����
    public float jumpForce = 5f; // ���� ������
    public float laneDistance = 2f; // ���������� ����� �������
    public float slideDuration = 1f; // ������������ ����������
    public Vector3 slideScale = new Vector3(1f, 0.5f, 1f); // ������� ��� ����������
    public float slideColliderHeight = 1f; // ������ ���������� ��� ����������

    private int currentLane = 1; // 0 - �����, 1 - �������, 2 - ������ �����
    private Vector3 targetPosition; // ������� ������� ��� ����� �����
    private bool isJumping = false; // ���� ������
    private bool isSliding = false; // ���� ����������
    private float slideTimer = 0f; // ������ ����������
    private Rigidbody rb; // ��������� Rigidbody
    private CapsuleCollider collider; // ��������� CapsuleCollider
    private Vector3 originalScale; // �������� �������
    private float originalColliderHeight; // �������� ������ ����������
    private Vector3 originalColliderCenter; // �������� ����� ����������

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        if (rb == null || collider == null)
        {
            Debug.LogError("Rigidbody ��� CapsuleCollider �� ������� �� ������!");
            return;
        }

        originalScale = transform.localScale;
        originalColliderHeight = collider.height;
        originalColliderCenter = collider.center;
        targetPosition = transform.position;

        // ���������, �������� �� ����� �����
        if (!IsGrounded())
        {
            AscendPlayerToGround();
        }
    }

    void FixedUpdate()
    {
        if (rb == null || collider == null) return;

        // ���������� �������� �����
        Vector3 velocity = rb.velocity;
        velocity.z = forwardSpeed; // ������������� ���������� �������� �� Z
        rb.velocity = velocity;

        // ������� ����������� ����� �������
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 10f);
        rb.MovePosition(new Vector3(newPosition.x, transform.position.y, newPosition.z));

        // ��������� ����������
        if (isSliding)
        {
            slideTimer += Time.fixedDeltaTime;
            if (slideTimer >= slideDuration)
            {
                isSliding = false;
                transform.localScale = originalScale;
                collider.height = originalColliderHeight;
                collider.center = originalColliderCenter;
                slideTimer = 0f;
            }
        }

        // �������� ����� � ������������� ������ ��� �������������
        if (!IsGrounded() && !isJumping)
        {
            Debug.LogWarning($"����� �� �������� �����. �������: {transform.position}, ������: {transform.position.y}");
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10f))
            {
                Debug.Log($"��� ������� ������� ����������� ��: {hit.point}, ����������: {hit.distance}");
                if (hit.distance > 0.5f) // ������������ ������ ���� ���������� ������������
                {
                    AscendPlayerToGround();
                }
            }
            else
            {
                Debug.LogWarning("��� ������� ��� �����������!");
            }
        }
        else if (IsGrounded())
        {
            isJumping = false; // ���������� ���� ������
            Debug.Log("����� �������� �����. �������: " + transform.position); // �������������� �������
        }
    }

    void Update()
    {
        if (rb == null || collider == null) return;
        // ��������� �������
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 50f && currentLane < 2) // ����� ������
                    {
                        currentLane++;
                        UpdateLane();
                    }
                    else if (delta.x < -50f && currentLane > 0) // ����� �����
                    {
                        currentLane--;
                        UpdateLane();
                    }
                }
                else
                {
                    if (delta.y > 50f && IsGrounded()) // ����� ����� - ������
                    {
                        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                        isJumping = true;
                    }
                    else if (delta.y < -50f && !isSliding) // ����� ���� - ����������
                    {
                        isSliding = true;
                        transform.localScale = slideScale;
                        collider.height = slideColliderHeight;
                        collider.center = new Vector3(0, slideColliderHeight * 0.5f, 0);
                    }
                }
            }
        }
    }

    void UpdateLane()
    {
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }

    bool IsGrounded()
    {
        // ���������� �������� � CapsuleCast ��� ������� ������������ ����������
        Vector3 p1 = transform.position + collider.center + Vector3.up * (collider.height * 0.5f - collider.radius);
        Vector3 p2 = transform.position + collider.center - Vector3.up * (collider.height * 0.5f - collider.radius);
        float distanceToGround = 0.2f; // ����� ��� ����������
        return Physics.CapsuleCast(p1, p2, collider.radius, Vector3.down, distanceToGround);
    }

    void AscendPlayerToGround()
    {
        RaycastHit hit;
        Vector3 start = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(start, Vector3.down, out hit, 10f))
        {
            // ������������� ������� ���, ����� ������ ����� ������� ���� �� �����������
            float heightOffset = collider.height * 0.5f + 0.05f; // ����������� ����� ��� ��������
            transform.position = hit.point + Vector3.up * heightOffset;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // ���������� ������������ ��������
            Debug.Log($"����� ��������� �� �����: {transform.position}");
        }
        else
        {
            Debug.LogWarning("�� ������� ����� ����� ��� �������!");
        }
    }
}