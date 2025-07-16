// CameraController.cs
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    void Start()
    {
        player = GameManager.Player.transform; // Access player from GameManager
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
            transform.LookAt(player);
        }
    }
}