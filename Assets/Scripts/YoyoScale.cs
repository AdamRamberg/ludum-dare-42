using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ScriptableObjectVariables;

public class YoyoScale : MonoBehaviour
{
    public FloatVariable targetScale;
    public FloatVariable duration;
    void Start()
    {
        transform.DOScale(targetScale.Value, duration.Value).SetLoops(-1, LoopType.Yoyo);
    }
}
