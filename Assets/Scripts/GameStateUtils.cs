using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectVariables;

public static class GameStateUtils
{
    public static bool IsPlaying(StringVariable state)
    {
        return state.Value.Contains(GameStateConstants.PLAYING);
    }

    public static bool IsPlaying(string state)
    {
        return state.Contains(GameStateConstants.PLAYING);
    }
}
