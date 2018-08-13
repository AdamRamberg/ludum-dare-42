using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ScriptableObjectVariables;

public class JumpScoreText : MonoBehaviour
{
    public ScoreManager scoreManager;
    private float jumpScoreLastFrame = 0f;
    private int jumpMultiplierLastFrame = 0;

    public FloatVariable pulseTargetScale;
    public FloatVariable pulseDuration;

    float scaleBeforePulse = 0f;

    void Start()
    {
        scoreManager.DidResetJumpScore += Reset;
        scoreManager.DidSetMultiplier += Pulse;
    }

    private void OnDestroy()
    {
        scoreManager.DidResetJumpScore -= Reset;
        scoreManager.DidSetMultiplier -= Pulse;
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
        scaleBeforePulse = transform.localScale.x;
        transform.DOScale(pulseTargetScale.Value, pulseDuration.Value / 2f).OnComplete(PulseBack);
    }

    void PulseBack()
    {
        transform.DOScale(scaleBeforePulse, pulseDuration.Value / 2f);
    }
}
