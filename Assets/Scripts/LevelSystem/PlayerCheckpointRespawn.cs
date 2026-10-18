using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCheckpointRespawn : MonoBehaviour
{
    private Rigidbody2D rb;


    private void Awake()
    {
        rb =
            GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        Vector3 checkpointPosition;


        bool foundCheckpoint =
            CheckpointSystem.TryGetCheckpoint(
                SceneManager
                    .GetActiveScene()
                    .name,

                out checkpointPosition
            );


        if (!foundCheckpoint)
        {
            return;
        }


        transform.position =
            checkpointPosition;


        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;
        }
    }
}