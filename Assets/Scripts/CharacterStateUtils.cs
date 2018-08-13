using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectVariables;

public static class CharacterStateUtils
{
    private const string JUMP = "Jump";
    public static bool IsJumping(CharacterState state)
    {
        return state.ToString().Contains(JUMP);
    }
}
