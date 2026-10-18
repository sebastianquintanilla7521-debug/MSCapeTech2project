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

    public bool randomizeColorOrder = true;


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

    public GameColor currentColor =
        GameColor.Red;

    public GameColor nextColor;


    // =========================
    // TIMING
    // =========================

    [Header("Color Timing")]

    public float colorDuration = 4f;

    public float warningDuration = 2f;


    [Tooltip(
        "How many seconds before the background changes " +
        "matching platforms become visible and solid again."
    )]
    [Min(0f)]
    public float platformReappearEarly = 0.5f;


    // =========================
    // EDITABLE COLOR PALETTE
    // =========================

    public Color redColor =
        new Color32(
            255,
            75,
            75,
            255
        );

    public Color orangeColor =
        new Color32(
            255,
            138,
            61,
            255
        );

    public Color yellowColor =
        new Color32(
            255,
            216,
            77,
            255
        );

    public Color greenColor =
        new Color32(
            82,
            210,
            115,
            255
        );

    public Color blueColor =
        new Color32(
            77,
            124,
            255,
            255
        );

    public Color purpleColor =
        new Color32(
            166,
            107,
            255,
            255
        );


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
        get
        {
            return isWarning;
        }
    }


    public float WarningProgress
    {
        get
        {
            return warningProgress;
        }
    }


    // =========================
    // INTERNAL VARIABLES
    // =========================

    private float colorTimer = 0f;

    private Camera mainCamera;


    // =========================
    // PUBLIC TIMING INFO
    // =========================

    public float ColorTimer
    {
        get
        {
            return colorTimer;
        }
    }


    // =========================
    // START
    // =========================

    void Start()
    {
        mainCamera =
            Camera.main;


        if (
            colorRotation == null ||
            colorRotation.Count == 0
        )
        {
            colorRotation =
                new List<GameColor>()
                {
                    GameColor.Red
                };
        }


        currentColor =
            colorRotation[0];


        nextColor =
            GetNextColor(
                currentColor
            );


        UpdateBackgroundColor();
    }


    // =========================
    // TIMER
    // =========================

    void Update()
    {
        colorTimer +=
            Time.deltaTime;


        float warningStartTime =
            colorDuration -
            warningDuration;


        // =========================
        // WARNING PERIOD
        // =========================

        if (
            colorTimer >=
            warningStartTime
        )
        {
            isWarning =
                true;


            warningProgress =
                Mathf.InverseLerp(
                    warningStartTime,
                    colorDuration,
                    colorTimer
                );
        }

        else
        {
            isWarning =
                false;

            warningProgress =
                0f;
        }


        // =========================
        // ACTUAL COLOR CHANGE
        // =========================

        if (
            colorTimer >=
            colorDuration
        )
        {
            ChangeColor();
        }
    }


    // =========================
    // SHOULD PLATFORM BE HIDDEN
    // =========================

    public bool ShouldPlatformBeHidden(
        GameColor platformColor
    )
    {
        // Different color from background:
        // platform should be visible.
        if (
            platformColor !=
            currentColor
        )
        {
            return false;
        }


        // Matching platform normally disappears,
        // but we bring it back slightly before
        // the current background color ends.
        float safeReappearEarly =
            Mathf.Clamp(
                platformReappearEarly,
                0f,
                colorDuration
            );


        float reappearTime =
            colorDuration -
            safeReappearEarly;


        // Before the early-return point:
        // hide it.
        if (
            colorTimer <
            reappearTime
        )
        {
            return true;
        }


        // During the final X seconds:
        // bring it back early.
        return false;
    }


    // =========================
    // CHANGE COLOR
    // =========================

    void ChangeColor()
    {
        currentColor =
            nextColor;


        nextColor =
            GetNextColor(
                currentColor
            );


        colorTimer =
            0f;

        isWarning =
            false;

        warningProgress =
            0f;


        UpdateBackgroundColor();
    }


    // =========================
    // CHOOSE NEXT COLOR
    // =========================

    GameColor GetNextColor(
        GameColor color
    )
    {
        if (
            colorRotation == null ||
            colorRotation.Count == 0
        )
        {
            return GameColor.Red;
        }


        if (
            colorRotation.Count == 1
        )
        {
            return
                colorRotation[0];
        }


        // =========================
        // RANDOMIZED
        // =========================

        if (
            randomizeColorOrder
        )
        {
            bool differentColorExists =
                false;


            foreach (
                GameColor availableColor
                in colorRotation
            )
            {
                if (
                    availableColor !=
                    color
                )
                {
                    differentColorExists =
                        true;

                    break;
                }
            }


            if (
                !differentColorExists
            )
            {
                return color;
            }


            GameColor randomColor;


            do
            {
                int randomIndex =
                    Random.Range(
                        0,
                        colorRotation.Count
                    );


                randomColor =
                    colorRotation[
                        randomIndex
                    ];
            }

            while (
                randomColor ==
                color
            );


            return randomColor;
        }


        // =========================
        // FIXED ORDER
        // =========================

        int currentIndex =
            colorRotation.IndexOf(
                color
            );


        if (
            currentIndex == -1
        )
        {
            return
                colorRotation[0];
        }


        int nextIndex =
            currentIndex + 1;


        if (
            nextIndex >=
            colorRotation.Count
        )
        {
            nextIndex =
                0;
        }


        return
            colorRotation[
                nextIndex
            ];
    }


    // =========================
    // UPDATE BACKGROUND
    // =========================

    void UpdateBackgroundColor()
    {
        if (
            mainCamera != null
        )
        {
            mainCamera.backgroundColor =
                GetUnityColor(
                    currentColor
                );
        }
    }


    // =========================
    // GET UNITY COLOR
    // =========================

    public Color GetUnityColor(
        GameColor color
    )
    {
        if (
            color ==
            GameColor.Red
        )
        {
            return redColor;
        }

        else if (
            color ==
            GameColor.Orange
        )
        {
            return orangeColor;
        }

        else if (
            color ==
            GameColor.Yellow
        )
        {
            return yellowColor;
        }

        else if (
            color ==
            GameColor.Green
        )
        {
            return greenColor;
        }

        else if (
            color ==
            GameColor.Blue
        )
        {
            return blueColor;
        }

        else
        {
            return purpleColor;
        }
    }
}