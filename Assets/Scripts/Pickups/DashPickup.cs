using UnityEngine;

public class DashPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player =
            other.GetComponent<PlayerMovement>();


        // If the thing touching us is the Player
        if (player != null)
        {
            // Try to give the Player one dash
            bool pickedUp =
                player.AddDashCharge();


            // Only destroy the pickup if the
            // Player actually had room for it
            if (pickedUp)
            {
                Destroy(gameObject);
            }
        }
    }
}