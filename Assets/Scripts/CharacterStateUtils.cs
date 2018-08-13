using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectVariables;

public static class CharacterStateUtils
{
    private const string JUMP = "Jump";
    private const string LANDED = "Landed";
    private const string FALLING = "Falling";
    public static bool IsJumping(CharacterState state)
    {
        return state.ToString().Contains(JUMP);
    }

    public static bool IsFalling(CharacterState state)
    {
        return state.ToString().Equals(FALLING);
    }

    public static bool HasLanded(CharacterState state)
    {
        return state.ToString().Equals(LANDED);
    }
}
