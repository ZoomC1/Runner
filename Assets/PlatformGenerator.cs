// PlatformGenerator.cs
using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    private Transform playerTransform;
    public float platformLength = 50f;
    public float spawnOffset = 100f;
    public int initialPlatforms = 3;

    private List<GameObject> activePlatforms = new List<GameObject>();
    private float lastSpawnZ = 0f;

    void Start()
    {
        playerTransform = GameManager.Player.transform; // Access player from GameManager
        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        if (playerTransform != null && playerTransform.position.z > lastSpawnZ - spawnOffset)
        {
            SpawnPlatform();
            DeleteOldPlatform();
        }
    }

    void SpawnPlatform()
    {
        GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        GameObject platform = Instantiate(prefab, new Vector3(0, 0, lastSpawnZ), Quaternion.identity);
        activePlatforms.Add(platform);
        lastSpawnZ += platformLength;
    }

    void DeleteOldPlatform()
    {
        if (activePlatforms.Count > initialPlatforms)
        {
            Destroy(activePlatforms[0]);
            activePlatforms.RemoveAt(0);
        }
    }
}