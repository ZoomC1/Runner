using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10f; // Скорость движения вперёд
    public float jumpForce = 5f; // Сила прыжка
    public float laneDistance = 2f; // Расстояние между линиями
    public float slideDuration = 1f; // Длительность скольжения
    public Vector3 slideScale = new Vector3(1f, 0.5f, 1f); // Масштаб при скольжении
    public float slideColliderHeight = 1f; // Высота коллайдера при скольжении

    private int currentLane = 1; // 0 - левая, 1 - средняя, 2 - правая линия
    private Vector3 targetPosition; // Целевая позиция для смены линии
    private bool isJumping = false; // Флаг прыжка
    private bool isSliding = false; // Флаг скольжения
    private float slideTimer = 0f; // Таймер скольжения
    private Rigidbody rb; // Компонент Rigidbody
    private CapsuleCollider collider; // Компонент CapsuleCollider
    private Vector3 originalScale; // Исходный масштаб
    private float originalColliderHeight; // Исходная высота коллайдера
    private Vector3 originalColliderCenter; // Исходный центр коллайдера

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        if (rb == null || collider == null)
        {
            Debug.LogError("Rigidbody или CapsuleCollider не найдены на игроке!");
            return;
        }

        originalScale = transform.localScale;
        originalColliderHeight = collider.height;
        originalColliderCenter = collider.center;
        targetPosition = transform.position;

        // Проверяем, касается ли игрок земли
        if (!IsGrounded())
        {
            AscendPlayerToGround();
        }
    }

    void FixedUpdate()
    {
        if (rb == null || collider == null) return;

        // Постоянное движение вперёд
        Vector3 velocity = rb.velocity;
        velocity.z = forwardSpeed; // Устанавливаем постоянную скорость по Z
        rb.velocity = velocity;

        // Плавное перемещение между линиями
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 10f);
        rb.MovePosition(new Vector3(newPosition.x, transform.position.y, newPosition.z));

        // Обработка скольжения
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

        // Проверка земли и корректировка только при необходимости
        if (!IsGrounded() && !isJumping)
        {
            Debug.LogWarning($"Игрок не касается земли. Позиция: {transform.position}, Высота: {transform.position.y}");
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10f))
            {
                Debug.Log($"Под игроком найдена поверхность на: {hit.point}, расстояние: {hit.distance}");
                if (hit.distance > 0.5f) // Корректируем только если расстояние значительное
                {
                    AscendPlayerToGround();
                }
            }
            else
            {
                Debug.LogWarning("Под игроком нет поверхности!");
            }
        }
        else if (IsGrounded())
        {
            isJumping = false; // Сбрасываем флаг прыжка
            Debug.Log("Игрок касается земли. Позиция: " + transform.position); // Дополнительная отладка
        }
    }

    void Update()
    {
        if (rb == null || collider == null) return;
        // Обработка свайпов
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 50f && currentLane < 2) // Свайп вправо
                    {
                        currentLane++;
                        UpdateLane();
                    }
                    else if (delta.x < -50f && currentLane > 0) // Свайп влево
                    {
                        currentLane--;
                        UpdateLane();
                    }
                }
                else
                {
                    if (delta.y > 50f && IsGrounded()) // Свайп вверх - прыжок
                    {
                        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                        isJumping = true;
                    }
                    else if (delta.y < -50f && !isSliding) // Свайп вниз - скольжение
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
        // Улучшенная проверка с CapsuleCast для точного соответствия коллайдеру
        Vector3 p1 = transform.position + collider.center + Vector3.up * (collider.height * 0.5f - collider.radius);
        Vector3 p2 = transform.position + collider.center - Vector3.up * (collider.height * 0.5f - collider.radius);
        float distanceToGround = 0.2f; // Запас для гравитации
        return Physics.CapsuleCast(p1, p2, collider.radius, Vector3.down, distanceToGround);
    }

    void AscendPlayerToGround()
    {
        RaycastHit hit;
        Vector3 start = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(start, Vector3.down, out hit, 10f))
        {
            // Устанавливаем позицию так, чтобы нижняя часть капсулы была на поверхности
            float heightOffset = collider.height * 0.5f + 0.05f; // Минимальный запас для контакта
            transform.position = hit.point + Vector3.up * heightOffset;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Сбрасываем вертикальную скорость
            Debug.Log($"Игрок перемещён на землю: {transform.position}");
        }
        else
        {
            Debug.LogWarning("Не удалось найти землю под игроком!");
        }
    }
}