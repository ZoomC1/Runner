using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject[] platformPrefabs; // Массив префабов платформ
    private Transform playerTransform; // Трансформ игрока
    public float platformLength = 10f; // Длина платформы по оси Z (настройте под ваши префабы)
    public float spawnOffset = 10f; // Расстояние от игрока для спавна новой платформы
    public int initialPlatforms = 3; // Количество платформ при старте

    private List<GameObject> activePlatforms = new List<GameObject>(); // Список активных платформ
    private float lastSpawnZ = 0f; // Позиция последней созданной платформы

    void Start()
    {
        // Пытаемся найти игрока при старте
        playerTransform = GameManager.Player?.transform;
        if (playerTransform == null)
        {
            Debug.LogWarning("Игрок не найден при старте. Ожидаем его создания.");
        }

        // Создаём начальные платформы
        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        // Если игрок ещё не найден, пытаемся найти его
        if (playerTransform == null && GameManager.Player != null)
        {
            playerTransform = GameManager.Player.transform;
            Debug.Log("Игрок найден в Update: " + playerTransform.position);
        }

        // Спавним новую платформу, если игрок близко к концу текущей
        if (playerTransform != null && playerTransform.position.z > lastSpawnZ - spawnOffset)
        {
            SpawnPlatform();
            DeleteOldPlatform();
        }
    }

    void SpawnPlatform()
    {
        // Выбираем случайный префаб платформы
        GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        // Создаём платформу в позиции lastSpawnZ
        GameObject platform = Instantiate(prefab, new Vector3(0, -1, lastSpawnZ), Quaternion.identity);
        activePlatforms.Add(platform);
        Debug.Log($"Создана платформа на Z: {lastSpawnZ}");
        lastSpawnZ += platformLength; // Сдвигаем точку спавна на длину платформы
    }

    void DeleteOldPlatform()
    {
        // Удаляем старую платформу, если их больше, чем нужно
        if (activePlatforms.Count > initialPlatforms)
        {
            Destroy(activePlatforms[0]);
            activePlatforms.RemoveAt(0);
        }
    }
}