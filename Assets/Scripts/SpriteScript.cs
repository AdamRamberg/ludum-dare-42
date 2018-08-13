using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ScriptableObjectVariables;

public class SpriteScript : MonoBehaviour
{
    public FloatVariable characterPreJumpTime;
    public FloatVariable rotation;
    public CharacterStateVariable characterState;

    void Start()
    {
        characterState.Changed += RotateOnPreJump;
    }

    private void OnDestroy()
    {
        characterState.Changed -= RotateOnPreJump;
    }

    void RotateOnPreJump(CharacterState state)
    {
        if (state == CharacterState.PreJumping)
        {
            transform.DORotate(new Vector3(0f, 0f, rotation.Value), characterPreJumpTime.Value / 2).OnComplete(CreateTweenBack);
        }
    }

    void CreateTweenBack()
    {
        transform.DORotate(new Vector3(0f, 0f, 0f), characterPreJumpTime.Value / 2);
    }
}
