using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    void Update()
    {
        // Find player if not yet assigned
        if (player == null && GameManager.Player != null)
        {
            player = GameManager.Player.transform;
        }

        // Follow player if found
        if (player != null)
        {
            transform.position = player.position + offset;
            transform.LookAt(player);
        }
    }
}