using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectVariables;

public enum CharacterState
{
    Idle,
    Running,
    CanJump,
    PreJumping,
    Jumping,

    Falling,
    Landed,
    Dead
};

[CreateAssetMenu(menuName = "Character State Variable")]
public class CharacterStateVariable : ScriptableObjectVariable<CharacterState>
{
}
