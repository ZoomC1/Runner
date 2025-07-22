using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject[] platformPrefabs; // ������ �������� ��������
    private Transform playerTransform; // ��������� ������
    public float platformLength = 10f; // ����� ��������� �� ��� Z (��������� ��� ���� �������)
    public float spawnOffset = 10f; // ���������� �� ������ ��� ������ ����� ���������
    public int initialPlatforms = 3; // ���������� �������� ��� ������

    private List<GameObject> activePlatforms = new List<GameObject>(); // ������ �������� ��������
    private float lastSpawnZ = 0f; // ������� ��������� ��������� ���������

    void Start()
    {
        // �������� ����� ������ ��� ������
        playerTransform = GameManager.Player?.transform;
        if (playerTransform == null)
        {
            Debug.LogWarning("����� �� ������ ��� ������. ������� ��� ��������.");
        }

        // ������ ��������� ���������
        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        // ���� ����� ��� �� ������, �������� ����� ���
        if (playerTransform == null && GameManager.Player != null)
        {
            playerTransform = GameManager.Player.transform;
            Debug.Log("����� ������ � Update: " + playerTransform.position);
        }

        // ������� ����� ���������, ���� ����� ������ � ����� �������
        if (playerTransform != null && playerTransform.position.z > lastSpawnZ - spawnOffset)
        {
            SpawnPlatform();
            DeleteOldPlatform();
        }
    }

    void SpawnPlatform()
    {
        // �������� ��������� ������ ���������
        GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        // ������ ��������� � ������� lastSpawnZ
        GameObject platform = Instantiate(prefab, new Vector3(0, -1, lastSpawnZ), Quaternion.identity);
        activePlatforms.Add(platform);
        Debug.Log($"������� ��������� �� Z: {lastSpawnZ}");
        lastSpawnZ += platformLength; // �������� ����� ������ �� ����� ���������
    }

    void DeleteOldPlatform()
    {
        // ������� ������ ���������, ���� �� ������, ��� �����
        if (activePlatforms.Count > initialPlatforms)
        {
            Destroy(activePlatforms[0]);
            activePlatforms.RemoveAt(0);
        }
    }
}