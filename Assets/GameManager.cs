// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform startPosition;
    public static GameObject Player { get; private set; } // Static reference

    void Start()
    {
        Player = Instantiate(playerPrefab, startPosition.position, Quaternion.identity);
    }
}