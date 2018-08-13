using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Score Manager")]
public class ScoreManager : ScriptableObject
{
    public int timesLanded = 0;
    public float totalScore = 0f;
    public Action DidReset;

    public void AddWhenInAir(float deltaTime)
    {
        totalScore += deltaTime * 100f;
    }

    public void DidLandOnMattress()
    {
        timesLanded++;
        totalScore += (timesLanded * 1000f);
    }

    public void AddToTotalScore(float value)
    {
        totalScore += value;
    }

    // public void SetMultipler(float multiplier)
    // {
    //     this.multiplier = multiplier;
    // }

    public void Reset()
    {
        totalScore = 0f;
        timesLanded = 0;
        if (DidReset != null) DidReset.Invoke();
    }

    public int GetTotalScore()
    {
        return Mathf.FloorToInt(totalScore);
    }
}
