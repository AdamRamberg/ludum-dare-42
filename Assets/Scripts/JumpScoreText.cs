using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpScoreText : MonoBehaviour
{
    public ScoreManager scoreManager;
    private float jumpScoreLastFrame = 0f;
    private int jumpMultiplierLastFrame = 0;

    void Start()
    {
        scoreManager.DidResetJumpScore += Reset;
    }

    private void OnDestroy()
    {
        scoreManager.DidResetJumpScore -= Reset;
    }

    void Reset()
    {
        jumpScoreLastFrame = 0;
        GetComponent<Text>().text = "0 x 1";
    }

    // Update is called once per frame
    void Update()
    {
        var jumpScore = scoreManager.GetJumpScore();
        var jumpMultiplier = scoreManager.GetJumpMultiplier();

        if (jumpScore != jumpScoreLastFrame || jumpMultiplier != jumpMultiplierLastFrame)
        {
            GetComponent<Text>().text = jumpScore + " x " + jumpMultiplier;
        }
    }

    public void Pulse()
    {

    }
}
