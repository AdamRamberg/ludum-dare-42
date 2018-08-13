using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    public ScoreManager scoreManager;
    int lastScore = 0;

    void Start()
    {
        scoreManager.DidReset += Reset;
    }

    private void OnDestroy()
    {
        scoreManager.DidReset -= Reset;
    }

    void Reset()
    {
        lastScore = 0;
        GetComponent<Text>().text = "Score: 0";
    }

    // Update is called once per frame
    void Update()
    {
        var totalScore = scoreManager.GetTotalScore();

        if (lastScore != totalScore)
        {
            GetComponent<Text>().text = "Score: " + totalScore;
        }
    }
}
