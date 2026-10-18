using UnityEngine;
using System.Collections.Generic;

public static class CheckpointSystem
{
    // ==================================================
    // CURRENT RESPAWN
    // ==================================================

    private static bool hasCheckpoint = false;

    private static string checkpointScene = "";

    private static Vector3 checkpointPosition;


    // ==================================================
    // COLLECTED CHECKPOINTS
    // ==================================================

    private static HashSet<string> collectedCheckpoints =
        new HashSet<string>();


    // ==================================================
    // RESET WHEN A NEW PLAY SESSION STARTS
    // ==================================================

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration
    )]
    private static void ResetOnGameStart()
    {
        hasCheckpoint = false;

        checkpointScene = "";

        checkpointPosition = Vector3.zero;

        collectedCheckpoints.Clear();
    }


    // ==================================================
    // SET CHECKPOINT
    // ==================================================

    public static void SetCheckpoint(
        string sceneName,
        string checkpointID,
        Vector3 position
    )
    {
        checkpointScene = sceneName;

        checkpointPosition = position;

        hasCheckpoint = true;


        string key =
            sceneName +
            "_" +
            checkpointID;


        collectedCheckpoints.Add(
            key
        );
    }


    // ==================================================
    // GET RESPAWN
    // ==================================================

    public static bool TryGetCheckpoint(
        string sceneName,
        out Vector3 position
    )
    {
        if (
            hasCheckpoint &&
            checkpointScene == sceneName
        )
        {
            position = checkpointPosition;

            return true;
        }


        position = Vector3.zero;

        return false;
    }


    // ==================================================
    // CHECK COLLECTION STATE
    // ==================================================

    public static bool IsCheckpointCollected(
        string sceneName,
        string checkpointID
    )
    {
        string key =
            sceneName +
            "_" +
            checkpointID;


        return collectedCheckpoints.Contains(
            key
        );
    }


    // ==================================================
    // CLEAR
    // ==================================================

    public static void ClearCheckpoint()
    {
        hasCheckpoint = false;

        checkpointScene = "";

        checkpointPosition = Vector3.zero;

        collectedCheckpoints.Clear();
    }
}