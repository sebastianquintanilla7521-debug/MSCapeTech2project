using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // The different colors our game can use
    public enum GameColor
    {
        Red,
        Yellow,
        Blue
    }

    // Which color is currently active
    public GameColor currentColor = GameColor.Red;

    // How long each background color stays active
    public float colorDuration = 2f;

    private float colorTimer = 0f;

    private Camera mainCamera;


    void Start()
    {
        // Find the Main Camera
        mainCamera = Camera.main;

        // Set the starting background color
        UpdateBackgroundColor();
    }


    void Update()
    {
        // Count upward every frame
        colorTimer += Time.deltaTime;


        // If enough time has passed,
        // switch to the next color
        if (colorTimer >= colorDuration)
        {
            colorTimer = 0f;

            NextColor();
        }
    }


    void NextColor()
    {
        // Red -> Yellow
        if (currentColor == GameColor.Red)
        {
            currentColor = GameColor.Yellow;
        }

        // Yellow -> Blue
        else if (currentColor == GameColor.Yellow)
        {
            currentColor = GameColor.Blue;
        }

        // Blue -> Red
        else
        {
            currentColor = GameColor.Red;
        }


        UpdateBackgroundColor();
    }


    void UpdateBackgroundColor()
    {
        if (currentColor == GameColor.Red)
        {
            mainCamera.backgroundColor = Color.red;
        }

        else if (currentColor == GameColor.Yellow)
        {
            mainCamera.backgroundColor = Color.yellow;
        }

        else if (currentColor == GameColor.Blue)
        {
            mainCamera.backgroundColor = Color.blue;
        }
    }
}