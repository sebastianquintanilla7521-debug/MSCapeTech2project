using UnityEngine;
using System.Collections.Generic;

public class ColorManager : MonoBehaviour
{
    // =========================
    // AVAILABLE COLORS
    // =========================

    public enum GameColor
    {
        Red,
        Yellow,
        Blue,
        Orange,
        Green,
        Purple
    }


    // =========================
    // LEVEL COLOR SETTINGS
    // =========================

    // Level designer can choose whether
    // the order is random or follows the list.
    public bool randomizeColorOrder = true;


    // Level designer chooses:
    //
    // - how many colors are used
    // - which colors are used
    // - their order when randomization is OFF
    public List<GameColor> colorRotation =
        new List<GameColor>()
        {
            GameColor.Red,
            GameColor.Yellow,
            GameColor.Blue
        };


    // =========================
    // CURRENT COLOR
    // =========================

    public GameColor currentColor = GameColor.Red;

    public GameColor nextColor;


    // =========================
    // TIMING
    // =========================

    public float colorDuration = 4f;

    public float warningDuration = 2f;


    // =========================
    // EDITABLE COLOR PALETTE
    // =========================

    public Color redColor =
        new Color32(255, 75, 75, 255);

    public Color orangeColor =
        new Color32(255, 138, 61, 255);

    public Color yellowColor =
        new Color32(255, 216, 77, 255);

    public Color greenColor =
        new Color32(82, 210, 115, 255);

    public Color blueColor =
        new Color32(77, 124, 255, 255);

    public Color purpleColor =
        new Color32(166, 107, 255, 255);


    // =========================
    // WARNING INFORMATION
    // =========================

    [SerializeField]
    private bool isWarning = false;


    [SerializeField]
    [Range(0f, 1f)]
    private float warningProgress = 0f;


    public bool IsWarning
    {
        get { return isWarning; }
    }


    public float WarningProgress
    {
        get { return warningProgress; }
    }


    // =========================
    // INTERNAL VARIABLES
    // =========================

    private float colorTimer = 0f;

    private Camera mainCamera;


    // =========================
    // START
    // =========================

    void Start()
    {
        mainCamera = Camera.main;


        // Safety check:
        // Make sure the list has at least one color.
        if (colorRotation == null ||
            colorRotation.Count == 0)
        {
            colorRotation =
                new List<GameColor>()
                {
                    GameColor.Red
                };
        }


        // Start on the first color chosen
        // by the level designer.
        currentColor =
            colorRotation[0];


        // Decide what comes next.
        nextColor =
            GetNextColor(currentColor);


        UpdateBackgroundColor();
    }


    // =========================
    // TIMER
    // =========================

    void Update()
    {
        colorTimer += Time.deltaTime;


        float warningStartTime =
            colorDuration - warningDuration;


        // =========================
        // WARNING PERIOD
        // =========================

        if (colorTimer >= warningStartTime)
        {
            isWarning = true;


            warningProgress =
                Mathf.InverseLerp(
                    warningStartTime,
                    colorDuration,
                    colorTimer
                );
        }

        else
        {
            isWarning = false;

            warningProgress = 0f;
        }


        // =========================
        // ACTUAL COLOR CHANGE
        // =========================

        if (colorTimer >= colorDuration)
        {
            ChangeColor();
        }
    }


    // =========================
    // CHANGE COLOR
    // =========================

    void ChangeColor()
    {
        // The color that was warned about
        // now becomes active.
        currentColor = nextColor;


        // Choose the color after that.
        nextColor =
            GetNextColor(currentColor);


        colorTimer = 0f;

        isWarning = false;

        warningProgress = 0f;


        UpdateBackgroundColor();
    }


    // =========================
    // CHOOSE NEXT COLOR
    // =========================

    GameColor GetNextColor(GameColor color)
    {
        // Safety
        if (colorRotation == null ||
            colorRotation.Count == 0)
        {
            return GameColor.Red;
        }


        // If only one color is in the level,
        // there is nowhere else to go.
        if (colorRotation.Count == 1)
        {
            return colorRotation[0];
        }


        // =========================
        // RANDOMIZED
        // =========================

        if (randomizeColorOrder)
        {
            // First check whether there actually
            // IS another different color available.
            bool differentColorExists = false;


            foreach (GameColor availableColor
                     in colorRotation)
            {
                if (availableColor != color)
                {
                    differentColorExists = true;

                    break;
                }
            }


            // If every entry is somehow the
            // same color, just return it.
            if (!differentColorExists)
            {
                return color;
            }


            GameColor randomColor;


            // Keep choosing until we get
            // something different from
            // the current color.
            do
            {
                int randomIndex =
                    Random.Range(
                        0,
                        colorRotation.Count
                    );


                randomColor =
                    colorRotation[randomIndex];
            }

            while (randomColor == color);


            return randomColor;
        }


        // =========================
        // FIXED ORDER
        // =========================

        int currentIndex =
            colorRotation.IndexOf(color);


        // If the current color somehow isn't
        // in the list, start at the beginning.
        if (currentIndex == -1)
        {
            return colorRotation[0];
        }


        int nextIndex =
            currentIndex + 1;


        // If we reached the end,
        // loop back to the beginning.
        if (nextIndex >= colorRotation.Count)
        {
            nextIndex = 0;
        }


        return colorRotation[nextIndex];
    }


    // =========================
    // UPDATE BACKGROUND
    // =========================

    void UpdateBackgroundColor()
    {
        if (mainCamera != null)
        {
            mainCamera.backgroundColor =
                GetUnityColor(currentColor);
        }
    }


    // =========================
    // GET UNITY COLOR
    // =========================

    public Color GetUnityColor(GameColor color)
    {
        if (color == GameColor.Red)
        {
            return redColor;
        }

        else if (color == GameColor.Orange)
        {
            return orangeColor;
        }

        else if (color == GameColor.Yellow)
        {
            return yellowColor;
        }

        else if (color == GameColor.Green)
        {
            return greenColor;
        }

        else if (color == GameColor.Blue)
        {
            return blueColor;
        }

        else
        {
            return purpleColor;
        }
    }
}