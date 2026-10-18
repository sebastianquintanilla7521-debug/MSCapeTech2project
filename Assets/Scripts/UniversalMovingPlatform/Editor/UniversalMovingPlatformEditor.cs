using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UniversalMovingPlatform))]
public class UniversalMovingPlatformEditor : Editor
{
    // ==================================================
    // BASIC SETUP
    // ==================================================

    private SerializedProperty movementType;
    private SerializedProperty startMode;


    // ==================================================
    // MOVEMENT POINTS
    // ==================================================

    private SerializedProperty movementPointsParent;
    private SerializedProperty pointCount;


    // ==================================================
    // PATH MOVEMENT
    // ==================================================

    private SerializedProperty pathBehavior;
    private SerializedProperty moveSpeed;
    private SerializedProperty waitAtEachPoint;
    private SerializedProperty movementEasing;
    private SerializedProperty startDelay;
    private SerializedProperty startAtPoint1;
    private SerializedProperty startMovingForward;
    private SerializedProperty loopForever;
    private SerializedProperty numberOfLoops;


    // ==================================================
    // FERRIS WHEEL
    // ==================================================

    private SerializedProperty rotationSpeed;
    private SerializedProperty clockwise;
    private SerializedProperty keepPlatformUpright;


    // ==================================================
    // STARTING PLATFORM
    // ==================================================

    private SerializedProperty startingSegmentsParent;
    private SerializedProperty startingSegmentPrefab;
    private SerializedProperty startingSegmentCount;
    private SerializedProperty segmentSpacing;
    private SerializedProperty autoArrangeStartingSegments;


    // ==================================================
    // PLAYER INTERACTION
    // ==================================================

    private SerializedProperty carryPlayer;
    private SerializedProperty jumpThroughFromBelow;
    private SerializedProperty oneWayPlatformLayerName;
    private SerializedProperty oneWaySurfaceArc;
    private SerializedProperty dangerousPlatform;
    private SerializedProperty dangerousHorizontalPlatformSprite;
    private SerializedProperty dangerousVerticalPlatformSprite;


    // ==================================================
    // DIRECTIONAL PLATFORM SPRITES
    // ==================================================

    private SerializedProperty automaticDirectionalSprites;
    private SerializedProperty horizontalPlatformSprite;
    private SerializedProperty verticalPlatformSprite;
    private SerializedProperty horizontalSpritesFaceRight;
    private SerializedProperty verticalSpritesFaceUp;
    private SerializedProperty directionChangeThreshold;


    // ==================================================
    // GROWING PLATFORM
    // ==================================================

    private SerializedProperty enableGrowingPlatform;
    private SerializedProperty growDirection;
    private SerializedProperty maximumAddedSegments;


    // ==================================================
    // DURABILITY
    // ==================================================

    private SerializedProperty enableDurability;
    private SerializedProperty firstBreakDelay;
    private SerializedProperty timeBetweenBreaks;
    private SerializedProperty warningDuration;
    private SerializedProperty breakOrder;
    private SerializedProperty minimumSegmentsRemaining;

    private SerializedProperty shakeAmount;
    private SerializedProperty shakeRotation;
    private SerializedProperty shakeSpeed;

    private SerializedProperty brokenPieceGravity;
    private SerializedProperty brokenPieceLifetime;


    // ==================================================
    // EDITOR QUALITY OF LIFE
    // ==================================================

    private SerializedProperty snapPointsToGrid;
    private SerializedProperty gridSize;

    private SerializedProperty showPointMarkers;
    private SerializedProperty showPathLines;
    private SerializedProperty showDirectionArrows;
    private SerializedProperty showPointNumbers;

    private SerializedProperty markerSize;


    // ==================================================
    // ADVANCED
    // ==================================================

    private SerializedProperty pointReachDistance;


    // ==================================================
    // ON ENABLE
    // ==================================================

    private void OnEnable()
    {
        // ==================================================
        // BASIC
        // ==================================================

        movementType =
            serializedObject.FindProperty(
                "movementType"
            );

        startMode =
            serializedObject.FindProperty(
                "startMode"
            );


        // ==================================================
        // POINTS
        // ==================================================

        movementPointsParent =
            serializedObject.FindProperty(
                "movementPointsParent"
            );

        pointCount =
            serializedObject.FindProperty(
                "pointCount"
            );


        // ==================================================
        // PATH
        // ==================================================

        pathBehavior =
            serializedObject.FindProperty(
                "pathBehavior"
            );

        moveSpeed =
            serializedObject.FindProperty(
                "moveSpeed"
            );

        waitAtEachPoint =
            serializedObject.FindProperty(
                "waitAtEachPoint"
            );

        movementEasing =
            serializedObject.FindProperty(
                "movementEasing"
            );

        startDelay =
            serializedObject.FindProperty(
                "startDelay"
            );

        startAtPoint1 =
            serializedObject.FindProperty(
                "startAtPoint1"
            );

        startMovingForward =
            serializedObject.FindProperty(
                "startMovingForward"
            );

        loopForever =
            serializedObject.FindProperty(
                "loopForever"
            );

        numberOfLoops =
            serializedObject.FindProperty(
                "numberOfLoops"
            );


        // ==================================================
        // FERRIS WHEEL
        // ==================================================

        rotationSpeed =
            serializedObject.FindProperty(
                "rotationSpeed"
            );

        clockwise =
            serializedObject.FindProperty(
                "clockwise"
            );

        keepPlatformUpright =
            serializedObject.FindProperty(
                "keepPlatformUpright"
            );


        // ==================================================
        // STARTING PLATFORM
        // ==================================================

        startingSegmentsParent =
            serializedObject.FindProperty(
                "startingSegmentsParent"
            );

        startingSegmentPrefab =
            serializedObject.FindProperty(
                "startingSegmentPrefab"
            );

        startingSegmentCount =
            serializedObject.FindProperty(
                "startingSegmentCount"
            );

        segmentSpacing =
            serializedObject.FindProperty(
                "segmentSpacing"
            );

        autoArrangeStartingSegments =
            serializedObject.FindProperty(
                "autoArrangeStartingSegments"
            );


        // ==================================================
        // PLAYER INTERACTION
        // ==================================================

        carryPlayer =
            serializedObject.FindProperty(
                "carryPlayer"
            );

        jumpThroughFromBelow =
            serializedObject.FindProperty(
                "jumpThroughFromBelow"
            );

        oneWayPlatformLayerName =
            serializedObject.FindProperty(
                "oneWayPlatformLayerName"
            );

        oneWaySurfaceArc =
            serializedObject.FindProperty(
                "oneWaySurfaceArc"
            );

        dangerousPlatform =
            serializedObject.FindProperty(
                "dangerousPlatform"
            );

        dangerousHorizontalPlatformSprite =
            serializedObject.FindProperty(
                "dangerousHorizontalPlatformSprite"
            );

        dangerousVerticalPlatformSprite =
            serializedObject.FindProperty(
                "dangerousVerticalPlatformSprite"
            );


        // ==================================================
        // DIRECTIONAL PLATFORM SPRITES
        // ==================================================

        automaticDirectionalSprites =
            serializedObject.FindProperty(
                "automaticDirectionalSprites"
            );

        horizontalPlatformSprite =
            serializedObject.FindProperty(
                "horizontalPlatformSprite"
            );

        verticalPlatformSprite =
            serializedObject.FindProperty(
                "verticalPlatformSprite"
            );

        horizontalSpritesFaceRight =
            serializedObject.FindProperty(
                "horizontalSpritesFaceRight"
            );

        verticalSpritesFaceUp =
            serializedObject.FindProperty(
                "verticalSpritesFaceUp"
            );

        directionChangeThreshold =
            serializedObject.FindProperty(
                "directionChangeThreshold"
            );


        // ==================================================
        // GROWING
        // ==================================================

        enableGrowingPlatform =
            serializedObject.FindProperty(
                "enableGrowingPlatform"
            );

        growDirection =
            serializedObject.FindProperty(
                "growDirection"
            );

        maximumAddedSegments =
            serializedObject.FindProperty(
                "maximumAddedSegments"
            );


        // ==================================================
        // DURABILITY
        // ==================================================

        enableDurability =
            serializedObject.FindProperty(
                "enableDurability"
            );

        firstBreakDelay =
            serializedObject.FindProperty(
                "firstBreakDelay"
            );

        timeBetweenBreaks =
            serializedObject.FindProperty(
                "timeBetweenBreaks"
            );

        warningDuration =
            serializedObject.FindProperty(
                "warningDuration"
            );

        breakOrder =
            serializedObject.FindProperty(
                "breakOrder"
            );

        minimumSegmentsRemaining =
            serializedObject.FindProperty(
                "minimumSegmentsRemaining"
            );


        shakeAmount =
            serializedObject.FindProperty(
                "shakeAmount"
            );

        shakeRotation =
            serializedObject.FindProperty(
                "shakeRotation"
            );

        shakeSpeed =
            serializedObject.FindProperty(
                "shakeSpeed"
            );


        brokenPieceGravity =
            serializedObject.FindProperty(
                "brokenPieceGravity"
            );

        brokenPieceLifetime =
            serializedObject.FindProperty(
                "brokenPieceLifetime"
            );


        // ==================================================
        // EDITOR
        // ==================================================

        snapPointsToGrid =
            serializedObject.FindProperty(
                "snapPointsToGrid"
            );

        gridSize =
            serializedObject.FindProperty(
                "gridSize"
            );

        showPointMarkers =
            serializedObject.FindProperty(
                "showPointMarkers"
            );

        showPathLines =
            serializedObject.FindProperty(
                "showPathLines"
            );

        showDirectionArrows =
            serializedObject.FindProperty(
                "showDirectionArrows"
            );

        showPointNumbers =
            serializedObject.FindProperty(
                "showPointNumbers"
            );

        markerSize =
            serializedObject.FindProperty(
                "markerSize"
            );


        // ==================================================
        // ADVANCED
        // ==================================================

        pointReachDistance =
            serializedObject.FindProperty(
                "pointReachDistance"
            );
    }


    // ==================================================
    // INSPECTOR
    // ==================================================

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        UniversalMovingPlatform platform =
            (UniversalMovingPlatform)target;


        bool isFerrisWheel =
            movementType.enumValueIndex ==
            (int)
            UniversalMovingPlatform
            .MovementType.FerrisWheel;


        // ==================================================
        // BASIC SETUP
        // ==================================================

        Section(
            "BASIC SETUP"
        );


        EditorGUILayout.PropertyField(
            movementType,
            new GUIContent(
                "Movement Type"
            )
        );


        EditorGUILayout.PropertyField(
            startMode,
            new GUIContent(
                "Start Mode"
            )
        );


        Space();


        // ==================================================
        // PATH SETUP
        // ==================================================

        if (!isFerrisWheel)
        {
            Section(
                "PATH SETUP"
            );


            EditorGUILayout.PropertyField(
                movementPointsParent,
                new GUIContent(
                    "Point Container"
                )
            );


            EditorGUILayout.PropertyField(
                pointCount,
                new GUIContent(
                    "Number Of Points"
                )
            );


            if (
                GUILayout.Button(
                    "CREATE / UPDATE PATH POINTS",
                    GUILayout.Height(28)
                )
            )
            {
                serializedObject
                    .ApplyModifiedProperties();


                CreateOrUpdatePathPoints(
                    platform
                );


                serializedObject.Update();
            }


            EditorGUILayout.BeginHorizontal();


            if (
                GUILayout.Button(
                    "ADD POINT"
                )
            )
            {
                serializedObject
                    .ApplyModifiedProperties();


                AddPoint(
                    platform
                );


                serializedObject.Update();
            }


            if (
                GUILayout.Button(
                    "REMOVE LAST"
                )
            )
            {
                serializedObject
                    .ApplyModifiedProperties();


                RemoveLastPoint(
                    platform
                );


                serializedObject.Update();
            }


            EditorGUILayout.EndHorizontal();


            if (
                GUILayout.Button(
                    "REVERSE POINT ORDER"
                )
            )
            {
                serializedObject
                    .ApplyModifiedProperties();


                ReversePointOrder(
                    platform
                );


                serializedObject.Update();
            }
        }


        // ==================================================
        // FERRIS PIVOT
        // ==================================================

        else
        {
            Section(
                "FERRIS WHEEL PIVOT"
            );


            EditorGUILayout.PropertyField(
                movementPointsParent,
                new GUIContent(
                    "Point Container"
                )
            );


            EditorGUILayout.HelpBox(
                "Ferris Wheel uses exactly one point. " +
                "That point is the center of rotation.",
                MessageType.Info
            );


            if (
                GUILayout.Button(
                    "CREATE / RESET PIVOT POINT",
                    GUILayout.Height(28)
                )
            )
            {
                serializedObject
                    .ApplyModifiedProperties();


                CreateFerrisWheelPivot(
                    platform
                );


                serializedObject.Update();
            }
        }


        Space();


        // ==================================================
        // PATH MOVEMENT
        // ==================================================

        Section(
            "PATH MOVEMENT"
        );


        EditorGUI.BeginDisabledGroup(
            isFerrisWheel
        );


        EditorGUILayout.PropertyField(
            pathBehavior,
            new GUIContent(
                "Path Behavior"
            )
        );


        EditorGUILayout.PropertyField(
            moveSpeed,
            new GUIContent(
                "Move Speed"
            )
        );


        EditorGUILayout.PropertyField(
            waitAtEachPoint,
            new GUIContent(
                "Wait At Each Point"
            )
        );


        EditorGUILayout.PropertyField(
            movementEasing,
            new GUIContent(
                "Movement Easing"
            )
        );


        EditorGUILayout.PropertyField(
            startDelay,
            new GUIContent(
                "Start Delay"
            )
        );


        EditorGUILayout.PropertyField(
            startAtPoint1,
            new GUIContent(
                "Start At Point 1"
            )
        );


        EditorGUILayout.PropertyField(
            startMovingForward,
            new GUIContent(
                "Start Moving Forward"
            )
        );


        bool pathCanLoop =
            pathBehavior.enumValueIndex ==
            (int)
            UniversalMovingPlatform
            .PathBehavior.Loop
            ||
            pathBehavior.enumValueIndex ==
            (int)
            UniversalMovingPlatform
            .PathBehavior.PingPong;


        EditorGUI.BeginDisabledGroup(
            !pathCanLoop
        );


        EditorGUILayout.PropertyField(
            loopForever,
            new GUIContent(
                "Loop Forever"
            )
        );


        if (
            pathCanLoop &&
            !loopForever.boolValue
        )
        {
            EditorGUILayout.PropertyField(
                numberOfLoops,
                new GUIContent(
                    "Number Of Loops"
                )
            );
        }


        EditorGUI.EndDisabledGroup();


        EditorGUI.EndDisabledGroup();


        if (isFerrisWheel)
        {
            EditorGUILayout.HelpBox(
                "Path Movement settings do not apply to Ferris Wheel.",
                MessageType.None
            );
        }


        Space();


        // ==================================================
        // FERRIS WHEEL
        // ==================================================

        Section(
            "FERRIS WHEEL"
        );


        EditorGUI.BeginDisabledGroup(
            !isFerrisWheel
        );


        EditorGUILayout.PropertyField(
            rotationSpeed,
            new GUIContent(
                "Rotation Speed"
            )
        );


        EditorGUILayout.PropertyField(
            clockwise,
            new GUIContent(
                "Clockwise"
            )
        );


        EditorGUILayout.PropertyField(
            keepPlatformUpright,
            new GUIContent(
                "Keep Platform Upright"
            )
        );


        EditorGUI.EndDisabledGroup();


        if (!isFerrisWheel)
        {
            EditorGUILayout.HelpBox(
                "Ferris Wheel settings only apply when Movement Type is Ferris Wheel.",
                MessageType.None
            );
        }


        Space();


        // ==================================================
        // STARTING PLATFORM
        // ==================================================

        Section(
            "STARTING PLATFORM"
        );


        EditorGUILayout.PropertyField(
            startingSegmentsParent,
            new GUIContent(
                "Starting Segments"
            )
        );


        // If the runtime script contains the
        // new startingSegmentPrefab field,
        // show the prefab slot.
        if (
            startingSegmentPrefab != null
        )
        {
            EditorGUILayout.PropertyField(
                startingSegmentPrefab,
                new GUIContent(
                    "Segment Prefab"
                )
            );
        }

        else
        {
            EditorGUILayout.HelpBox(
                "The UniversalMovingPlatform script does not currently contain a 'startingSegmentPrefab' field. Add that field before using automatic platform building.",
                MessageType.Warning
            );
        }


        EditorGUILayout.PropertyField(
            startingSegmentCount,
            new GUIContent(
                "Starting Segment Count"
            )
        );


        EditorGUILayout.PropertyField(
            segmentSpacing,
            new GUIContent(
                "Segment Spacing"
            )
        );


        EditorGUILayout.PropertyField(
            autoArrangeStartingSegments,
            new GUIContent(
                "Auto Arrange Segments"
            )
        );


        EditorGUI.BeginDisabledGroup(
            startingSegmentPrefab == null
        );


        if (
            GUILayout.Button(
                "BUILD / UPDATE STARTING PLATFORM",
                GUILayout.Height(28)
            )
        )
        {
            serializedObject
                .ApplyModifiedProperties();


            BuildStartingPlatform(
                platform
            );


            serializedObject.Update();
        }


        EditorGUI.EndDisabledGroup();


        EditorGUILayout.HelpBox(
            "Choose one platform segment prefab, set how many starting pieces you want, then press Build / Update.",
            MessageType.Info
        );


        Space();


        // ==================================================
        // PLAYER INTERACTION
        // ==================================================

        Section(
            "PLAYER INTERACTION"
        );


        if (carryPlayer != null)
        {
            EditorGUILayout.PropertyField(
                carryPlayer,
                new GUIContent(
                    "Carry Player"
                )
            );
        }


        if (jumpThroughFromBelow != null)
        {
            EditorGUILayout.PropertyField(
                jumpThroughFromBelow,
                new GUIContent(
                    "Jump Through From Below"
                )
            );


            EditorGUI.BeginDisabledGroup(
                !jumpThroughFromBelow.boolValue
            );


            if (oneWayPlatformLayerName != null)
            {
                EditorGUILayout.PropertyField(
                    oneWayPlatformLayerName,
                    new GUIContent(
                        "One Way Platform Layer"
                    )
                );
            }


            if (oneWaySurfaceArc != null)
            {
                EditorGUILayout.PropertyField(
                    oneWaySurfaceArc,
                    new GUIContent(
                        "One Way Surface Arc"
                    )
                );
            }


            EditorGUI.EndDisabledGroup();
        }


        if (dangerousPlatform != null)
        {
            EditorGUILayout.Space(4);

            EditorGUILayout.PropertyField(
                dangerousPlatform,
                new GUIContent(
                    "Dangerous Platform"
                )
            );


            if (dangerousPlatform.boolValue)
            {
                EditorGUI.indentLevel++;


                if (dangerousHorizontalPlatformSprite != null)
                {
                    EditorGUILayout.PropertyField(
                        dangerousHorizontalPlatformSprite,
                        new GUIContent(
                            "Dangerous Horizontal Sprite"
                        )
                    );
                }


                if (dangerousVerticalPlatformSprite != null)
                {
                    EditorGUILayout.PropertyField(
                        dangerousVerticalPlatformSprite,
                        new GUIContent(
                            "Dangerous Vertical Sprite"
                        )
                    );
                }


                EditorGUILayout.HelpBox(
                    "Optional overrides. Leave either empty to use the normal " +
                    "Horizontal / Vertical Sprite. Existing color tint is preserved.",
                    MessageType.None
                );


                EditorGUI.indentLevel--;
            }
        }


        EditorGUILayout.HelpBox(
            "Carry Player keeps the player moving with the platform. " +
            "Jump Through From Below makes the platform one-way and also " +
            "uses the ColorPlatform layer so the player's S + Right Click " +
            "down-dash can pass through it. Dangerous Platform resets the " +
            "current level when the player collides with it. Dangerous sprite slots " +
            "let you override the normal platform art for lethal variants.",
            MessageType.Info
        );


        Space();


        // ==================================================
        // DIRECTIONAL PLATFORM SPRITES
        // ==================================================

        Section(
            "DIRECTIONAL PLATFORM SPRITES"
        );


        if (automaticDirectionalSprites != null)
        {
            EditorGUILayout.PropertyField(
                automaticDirectionalSprites,
                new GUIContent(
                    "Automatic Directional Sprites"
                )
            );


            EditorGUI.BeginDisabledGroup(
                !automaticDirectionalSprites.boolValue
            );


            if (horizontalPlatformSprite != null)
            {
                EditorGUILayout.PropertyField(
                    horizontalPlatformSprite,
                    new GUIContent(
                        "Horizontal Sprite"
                    )
                );
            }


            if (verticalPlatformSprite != null)
            {
                EditorGUILayout.PropertyField(
                    verticalPlatformSprite,
                    new GUIContent(
                        "Vertical Sprite"
                    )
                );
            }


            if (horizontalSpritesFaceRight != null)
            {
                EditorGUILayout.PropertyField(
                    horizontalSpritesFaceRight,
                    new GUIContent(
                        "Horizontal Sprites Face Right"
                    )
                );
            }


            if (verticalSpritesFaceUp != null)
            {
                EditorGUILayout.PropertyField(
                    verticalSpritesFaceUp,
                    new GUIContent(
                        "Vertical Sprites Face Up"
                    )
                );
            }


            if (directionChangeThreshold != null)
            {
                EditorGUILayout.PropertyField(
                    directionChangeThreshold,
                    new GUIContent(
                        "Direction Change Threshold"
                    )
                );
            }


            EditorGUI.EndDisabledGroup();
        }


        EditorGUILayout.HelpBox(
            "Assign one WHITE/base RIGHT-facing horizontal sprite and one " +
            "WHITE/base UP-facing vertical sprite. Every starting or grown segment " +
            "keeps its existing SpriteRenderer color tint. Left movement flips X " +
            "and downward movement flips Y automatically.",
            MessageType.Info
        );


        Space();


        // ==================================================
        // GROWING PLATFORM
        // ==================================================

        Section(
            "GROWING PLATFORM"
        );


        EditorGUI.BeginDisabledGroup(
            isFerrisWheel
        );


        EditorGUILayout.PropertyField(
            enableGrowingPlatform,
            new GUIContent(
                "Enable Growing Platform"
            )
        );


        if (
            enableGrowingPlatform.boolValue &&
            !isFerrisWheel
        )
        {
            EditorGUILayout.PropertyField(
                growDirection,
                new GUIContent(
                    "Attach New Segments"
                )
            );


            EditorGUILayout.PropertyField(
                maximumAddedSegments,
                new GUIContent(
                    "Maximum Added Segments"
                )
            );


            EditorGUILayout.HelpBox(
                "No manual attachment slots are required. " +
                "Bonked pieces are positioned automatically.",
                MessageType.Info
            );
        }


        EditorGUI.EndDisabledGroup();


        if (isFerrisWheel)
        {
            EditorGUILayout.HelpBox(
                "Growing Platform is disabled for Ferris Wheel platforms.",
                MessageType.Info
            );
        }


        Space();


        // ==================================================
        // DURABILITY
        // ==================================================

        Section(
            "DURABILITY"
        );


        EditorGUILayout.PropertyField(
            enableDurability,
            new GUIContent(
                "Enable Durability"
            )
        );


        if (enableDurability.boolValue)
        {
            EditorGUILayout.Space(4);


            SubSection(
                "Break Timing"
            );


            EditorGUILayout.PropertyField(
                firstBreakDelay,
                new GUIContent(
                    "First Break Delay"
                )
            );


            EditorGUILayout.PropertyField(
                timeBetweenBreaks,
                new GUIContent(
                    "Time Between Breaks"
                )
            );


            EditorGUILayout.PropertyField(
                warningDuration,
                new GUIContent(
                    "Warning Duration"
                )
            );


            EditorGUILayout.Space(5);


            SubSection(
                "Break Behavior"
            );


            if (breakOrder != null)
            {
                EditorGUILayout.PropertyField(
                    breakOrder,
                    new GUIContent(
                        "Break Order"
                    )
                );
            }


            EditorGUILayout.PropertyField(
                minimumSegmentsRemaining,
                new GUIContent(
                    "Minimum Segments Remaining"
                )
            );


            EditorGUILayout.Space(5);


            SubSection(
                "Warning Effect"
            );


            if (shakeAmount != null)
            {
                EditorGUILayout.PropertyField(
                    shakeAmount,
                    new GUIContent(
                        "Shake Amount"
                    )
                );
            }


            if (shakeRotation != null)
            {
                EditorGUILayout.PropertyField(
                    shakeRotation,
                    new GUIContent(
                        "Shake Rotation"
                    )
                );
            }


            if (shakeSpeed != null)
            {
                EditorGUILayout.PropertyField(
                    shakeSpeed,
                    new GUIContent(
                        "Shake Speed"
                    )
                );
            }


            EditorGUILayout.Space(5);


            SubSection(
                "Broken Piece"
            );


            if (brokenPieceGravity != null)
            {
                EditorGUILayout.PropertyField(
                    brokenPieceGravity,
                    new GUIContent(
                        "Gravity"
                    )
                );
            }


            if (brokenPieceLifetime != null)
            {
                EditorGUILayout.PropertyField(
                    brokenPieceLifetime,
                    new GUIContent(
                        "Lifetime"
                    )
                );
            }
        }


        Space();


        // ==================================================
        // EDITOR QUALITY OF LIFE
        // ==================================================

        Section(
            "EDITOR QUALITY OF LIFE"
        );


        EditorGUILayout.PropertyField(
            snapPointsToGrid,
            new GUIContent(
                "Snap Points To Grid"
            )
        );


        if (snapPointsToGrid.boolValue)
        {
            EditorGUILayout.PropertyField(
                gridSize,
                new GUIContent(
                    "Grid Size"
                )
            );
        }


        EditorGUILayout.PropertyField(
            showPointMarkers,
            new GUIContent(
                "Show Point Markers"
            )
        );


        EditorGUILayout.PropertyField(
            showPathLines,
            new GUIContent(
                "Show Path Lines"
            )
        );


        EditorGUILayout.PropertyField(
            showDirectionArrows,
            new GUIContent(
                "Show Direction Arrows"
            )
        );


        EditorGUILayout.PropertyField(
            showPointNumbers,
            new GUIContent(
                "Show Point Numbers"
            )
        );


        EditorGUILayout.PropertyField(
            markerSize,
            new GUIContent(
                "Marker Size"
            )
        );


        Space();


        // ==================================================
        // ADVANCED
        // ==================================================

        Section(
            "ADVANCED"
        );


        EditorGUILayout.PropertyField(
            pointReachDistance,
            new GUIContent(
                "Point Reach Distance"
            )
        );


        serializedObject
            .ApplyModifiedProperties();
    }


    // ==================================================
    // BUILD STARTING PLATFORM
    // ==================================================

    private void BuildStartingPlatform(
        UniversalMovingPlatform platform
    )
    {
        if (startingSegmentPrefab == null)
        {
            return;
        }


        GameObject segmentPrefab =
            startingSegmentPrefab
            .objectReferenceValue
            as GameObject;


        if (segmentPrefab == null)
        {
            EditorUtility.DisplayDialog(
                "Missing Segment Prefab",
                "Assign a Segment Prefab before building the platform.",
                "OK"
            );


            return;
        }


        // ==================================================
        // FIND / CREATE PARENT
        // ==================================================

        Transform parent =
            platform.StartingSegmentsParent;


        if (parent == null)
        {
            GameObject parentObject =
                new GameObject(
                    "StartingSegments"
                );


            Undo.RegisterCreatedObjectUndo(
                parentObject,
                "Create Starting Segments"
            );


            parentObject.transform.SetParent(
                platform.transform
            );


            parentObject.transform.localPosition =
                Vector3.zero;


            parentObject.transform.localRotation =
                Quaternion.identity;


            parentObject.transform.localScale =
                Vector3.one;


            parent =
                parentObject.transform;


            Undo.RecordObject(
                platform,
                "Assign Starting Segments"
            );


            platform.StartingSegmentsParent =
                parent;


            EditorUtility.SetDirty(
                platform
            );
        }


        int desiredCount =
            Mathf.Max(
                1,
                startingSegmentCount.intValue
            );


        float spacing =
            Mathf.Max(
                0.01f,
                segmentSpacing.floatValue
            );


        // ==================================================
        // CREATE MISSING SEGMENTS
        // ==================================================

        while (
            parent.childCount <
            desiredCount
        )
        {
            GameObject newSegment = null;


            Object createdObject =
                PrefabUtility.InstantiatePrefab(
                    segmentPrefab,
                    parent
                );


            if (createdObject != null)
            {
                newSegment =
                    createdObject
                    as GameObject;
            }


            // Fallback if the supplied object
            // is not recognized as a normal prefab.
            if (newSegment == null)
            {
                newSegment =
                    Object.Instantiate(
                        segmentPrefab,
                        parent
                    );


                newSegment.name =
                    segmentPrefab.name;
            }


            Undo.RegisterCreatedObjectUndo(
                newSegment,
                "Create Platform Segment"
            );
        }


        // ==================================================
        // REMOVE EXTRA SEGMENTS
        // ==================================================

        while (
            parent.childCount >
            desiredCount
        )
        {
            Transform lastSegment =
                parent.GetChild(
                    parent.childCount - 1
                );


            Undo.DestroyObjectImmediate(
                lastSegment.gameObject
            );
        }


        // ==================================================
        // ARRANGE
        // ==================================================

        for (
            int i = 0;
            i < parent.childCount;
            i++
        )
        {
            Transform segment =
                parent.GetChild(i);


            Undo.RecordObject(
                segment,
                "Arrange Platform Segment"
            );


            segment.name =
                "Segment" +
                (i + 1);


            segment.localPosition =
                new Vector3(
                    i * spacing,
                    0f,
                    0f
                );


            segment.localRotation =
                Quaternion.identity;


            segment.localScale =
                Vector3.one;
        }


        EditorUtility.SetDirty(
            parent
        );


        SceneView.RepaintAll();
    }


    // ==================================================
    // CREATE / UPDATE PATH
    // ==================================================

    private void CreateOrUpdatePathPoints(
        UniversalMovingPlatform platform
    )
    {
        Transform container =
            GetOrCreatePointContainer(
                platform
            );


        int desiredCount =
            Mathf.Max(
                2,
                platform.PointCount
            );


        // CREATE MISSING POINTS
        while (
            container.childCount <
            desiredCount
        )
        {
            int index =
                container.childCount;


            GameObject point =
                new GameObject(
                    "Point " +
                    (index + 1)
                );


            Undo.RegisterCreatedObjectUndo(
                point,
                "Create Movement Point"
            );


            point.transform.SetParent(
                container
            );


            Vector3 position;


            if (index == 0)
            {
                position =
                    platform
                    .transform
                    .position;
            }

            else
            {
                position =
                    container
                    .GetChild(
                        index - 1
                    )
                    .position
                    +
                    Vector3.right *
                    3f;
            }


            point.transform.position =
                SnapPosition(
                    platform,
                    position
                );
        }


        // REMOVE EXTRA POINTS
        while (
            container.childCount >
            desiredCount
        )
        {
            Transform last =
                container.GetChild(
                    container.childCount -
                    1
                );


            Undo.DestroyObjectImmediate(
                last.gameObject
            );
        }


        RenamePoints(
            container
        );


        EditorUtility.SetDirty(
            platform
        );


        SceneView.RepaintAll();
    }


    // ==================================================
    // ADD POINT
    // ==================================================

    private void AddPoint(
        UniversalMovingPlatform platform
    )
    {
        platform.PointCount =
            Mathf.Max(
                2,
                platform.PointCount +
                1
            );


        EditorUtility.SetDirty(
            platform
        );


        CreateOrUpdatePathPoints(
            platform
        );
    }


    // ==================================================
    // REMOVE LAST POINT
    // ==================================================

    private void RemoveLastPoint(
        UniversalMovingPlatform platform
    )
    {
        platform.PointCount =
            Mathf.Max(
                2,
                platform.PointCount -
                1
            );


        EditorUtility.SetDirty(
            platform
        );


        CreateOrUpdatePathPoints(
            platform
        );
    }


    // ==================================================
    // REVERSE POINT ORDER
    // ==================================================

    private void ReversePointOrder(
        UniversalMovingPlatform platform
    )
    {
        Transform container =
            platform.MovementPointsParent;


        if (
            container == null ||
            container.childCount < 2
        )
        {
            return;
        }


        int count =
            container.childCount;


        Transform[] points =
            new Transform[count];


        for (
            int i = 0;
            i < count;
            i++
        )
        {
            points[i] =
                container.GetChild(i);
        }


        Undo.RecordObject(
            container,
            "Reverse Movement Points"
        );


        for (
            int i = 0;
            i < count;
            i++
        )
        {
            points[
                count - 1 - i
            ]
            .SetSiblingIndex(i);
        }


        RenamePoints(
            container
        );


        SceneView.RepaintAll();
    }


    // ==================================================
    // FERRIS PIVOT
    // ==================================================

    private void CreateFerrisWheelPivot(
        UniversalMovingPlatform platform
    )
    {
        Transform container =
            GetOrCreatePointContainer(
                platform
            );


        // Ferris Wheel only uses one point.
        while (
            container.childCount > 1
        )
        {
            Transform extra =
                container.GetChild(
                    container.childCount -
                    1
                );


            Undo.DestroyObjectImmediate(
                extra.gameObject
            );
        }


        Transform pivot;


        if (container.childCount == 0)
        {
            GameObject pivotObject =
                new GameObject(
                    "Pivot Point"
                );


            Undo.RegisterCreatedObjectUndo(
                pivotObject,
                "Create Ferris Wheel Pivot"
            );


            pivotObject
                .transform
                .SetParent(
                    container
                );


            pivot =
                pivotObject.transform;
        }

        else
        {
            pivot =
                container.GetChild(0);


            pivot.name =
                "Pivot Point";
        }


        Undo.RecordObject(
            pivot,
            "Position Ferris Wheel Pivot"
        );


        pivot.position =
            SnapPosition(
                platform,
                platform.transform.position +
                Vector3.down * 3f
            );


        Selection.activeTransform =
            pivot;


        SceneView.RepaintAll();
    }


    // ==================================================
    // GET / CREATE POINT CONTAINER
    // ==================================================

    private Transform GetOrCreatePointContainer(
        UniversalMovingPlatform platform
    )
    {
        if (
            platform.MovementPointsParent !=
            null
        )
        {
            return
                platform
                .MovementPointsParent;
        }


        GameObject container =
            new GameObject(
                platform.gameObject.name +
                "_MovementPoints"
            );


        Undo.RegisterCreatedObjectUndo(
            container,
            "Create Movement Points"
        );


        // Keep path points OUTSIDE the moving
        // platform so they do not move with it.
        if (
            platform.transform.parent !=
            null
        )
        {
            container
                .transform
                .SetParent(
                    platform.transform.parent
                );
        }


        platform.MovementPointsParent =
            container.transform;


        EditorUtility.SetDirty(
            platform
        );


        return container.transform;
    }


    // ==================================================
    // RENAME POINTS
    // ==================================================

    private void RenamePoints(
        Transform container
    )
    {
        for (
            int i = 0;
            i < container.childCount;
            i++
        )
        {
            container
                .GetChild(i)
                .name =
                "Point " +
                (i + 1);
        }
    }


    // ==================================================
    // GRID SNAP
    // ==================================================

    private Vector3 SnapPosition(
        UniversalMovingPlatform platform,
        Vector3 position
    )
    {
        if (!platform.SnapPointsToGrid)
        {
            return position;
        }


        float size =
            Mathf.Max(
                0.01f,
                platform.GridSize
            );


        position.x =
            Mathf.Round(
                position.x / size
            )
            *
            size;


        position.y =
            Mathf.Round(
                position.y / size
            )
            *
            size;


        return position;
    }


    // ==================================================
    // SCENE VIEW
    // ==================================================

    private void OnSceneGUI()
    {
        UniversalMovingPlatform platform =
            (UniversalMovingPlatform)target;


        Transform container =
            platform.MovementPointsParent;


        if (
            container == null ||
            container.childCount == 0
        )
        {
            return;
        }


        serializedObject.Update();


        bool showMarkers =
            showPointMarkers.boolValue;


        bool showLines =
            showPathLines.boolValue;


        bool showNumbers =
            showPointNumbers.boolValue;


        bool showArrows =
            showDirectionArrows.boolValue;


        int count =
            container.childCount;


        bool isFerrisWheel =
            platform.PlatformMovementType ==
            UniversalMovingPlatform
            .MovementType.FerrisWheel;


        // ==================================================
        // POINT HANDLES
        // ==================================================

        for (
            int i = 0;
            i < count;
            i++
        )
        {
            Transform point =
                container.GetChild(i);


            EditorGUI.BeginChangeCheck();


            Vector3 newPosition =
                Handles.PositionHandle(
                    point.position,
                    Quaternion.identity
                );


            if (
                EditorGUI.EndChangeCheck()
            )
            {
                Undo.RecordObject(
                    point,
                    "Move Platform Point"
                );


                point.position =
                    SnapPosition(
                        platform,
                        newPosition
                    );


                EditorUtility.SetDirty(
                    point
                );


                platform.RefreshDirectionalSpritePreview();
            }


            if (showMarkers)
            {
                float size =
                    markerSize.floatValue;


                if (isFerrisWheel)
                {
                    Handles.color =
                        Color.yellow;
                }

                else if (i == 0)
                {
                    Handles.color =
                        Color.green;
                }

                else if (
                    i == count - 1
                )
                {
                    Handles.color =
                        Color.red;
                }

                else
                {
                    Handles.color =
                        Color.yellow;
                }


                Handles.SphereHandleCap(
                    0,
                    point.position,
                    Quaternion.identity,
                    size,
                    EventType.Repaint
                );
            }


            if (showNumbers)
            {
                Handles.color =
                    Color.white;


                Handles.Label(
                    point.position +
                    Vector3.up *
                    0.5f,

                    isFerrisWheel
                    ? "Pivot"
                    : "Point " +
                      (i + 1)
                );
            }
        }


        // ==================================================
        // FERRIS PREVIEW
        // ==================================================

        if (isFerrisWheel)
        {
            Transform pivot =
                container.GetChild(0);


            float radius =
                Vector2.Distance(
                    pivot.position,
                    platform
                    .transform
                    .position
                );


            Handles.color =
                Color.white;


            if (showLines)
            {
                Handles.DrawDottedLine(
                    pivot.position,
                    platform
                    .transform
                    .position,
                    4f
                );


                Handles.DrawWireDisc(
                    pivot.position,
                    Vector3.forward,
                    radius
                );
            }


            return;
        }


        // ==================================================
        // PATH LINES
        // ==================================================

        if (showLines)
        {
            Handles.color =
                Color.white;


            for (
                int i = 0;
                i < count - 1;
                i++
            )
            {
                Vector3 a =
                    container
                    .GetChild(i)
                    .position;


                Vector3 b =
                    container
                    .GetChild(i + 1)
                    .position;


                Handles.DrawLine(
                    a,
                    b
                );


                if (showArrows)
                {
                    DrawDirectionArrow(
                        a,
                        b
                    );
                }
            }
        }
    }


    // ==================================================
    // DIRECTION ARROW
    // ==================================================

    private void DrawDirectionArrow(
        Vector3 start,
        Vector3 end
    )
    {
        Vector3 direction =
            (end - start)
            .normalized;


        if (
            direction.sqrMagnitude <
            0.001f
        )
        {
            return;
        }


        Vector3 middle =
            Vector3.Lerp(
                start,
                end,
                0.5f
            );


        Vector3 perpendicular =
            new Vector3(
                -direction.y,
                direction.x,
                0f
            );


        float size =
            0.2f;


        Vector3 tip =
            middle +
            direction * size;


        Vector3 left =
            middle -
            direction * size +
            perpendicular * size;


        Vector3 right =
            middle -
            direction * size -
            perpendicular * size;


        Handles.DrawAAConvexPolygon(
            tip,
            left,
            right
        );
    }


    // ==================================================
    // UI HELPERS
    // ==================================================

    private void Section(
        string title
    )
    {
        EditorGUILayout.Space(4);


        EditorGUILayout.LabelField(
            title,
            EditorStyles.boldLabel
        );
    }


    private void SubSection(
        string title
    )
    {
        EditorGUILayout.LabelField(
            title,
            EditorStyles.miniBoldLabel
        );
    }


    private void Space()
    {
        EditorGUILayout.Space(10);
    }
}