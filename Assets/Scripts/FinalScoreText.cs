using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreText : MonoBehaviour
{
    public ScoreManager scoreManager;
    public void SetText()
    {
        GetComponent<Text>().text = "Your score: " + scoreManager.GetTotalScore();
    }
}
