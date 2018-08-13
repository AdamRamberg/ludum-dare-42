using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Score Manager")]
public class ScoreManager : ScriptableObject
{
    public int timesLanded = 0;
    public float totalScore = 0f;
    public float jumpScore = 0f;
    public int jumpMultiplier = 1;
    public Action DidReset;

    public Action DidResetJumpScore;

    public void AddWhenFalling(float deltaTime)
    {
        jumpScore += deltaTime * 100f;
    }

    public void DidLandOnMattress()
    {
        timesLanded++;
        jumpScore += (timesLanded * 1000f);
    }

    public void AddToTotalScore(float value)
    {
        totalScore += value;
    }

    public void AddJumpScoreToTotal()
    {
        totalScore += jumpScore * jumpMultiplier;
        ResetJumpScore();
    }

    public void Reset()
    {
        totalScore = 0f;
        timesLanded = 0;
        ResetJumpScore();
        if (DidReset != null) DidReset.Invoke();
    }

    public void ResetJumpScore()
    {
        jumpScore = 0f;
        jumpMultiplier = 1;
        if (DidResetJumpScore != null) DidResetJumpScore.Invoke();
    }

    public int GetTotalScore()
    {
        return Mathf.FloorToInt(totalScore);
    }

    public int GetJumpScore()
    {
        return Mathf.FloorToInt(jumpScore);
    }

    public int GetJumpMultiplier()
    {
        return jumpMultiplier;
    }
}
