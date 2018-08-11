using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour
{
    public LineRenderer lineRenderer;

    Vector3 originalLastLinePos;
    private Character character;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        character = transform.parent.GetComponent<Character>();

        var v = lineRenderer.GetPosition(1);
        originalLastLinePos = new Vector3(v.x, v.y, v.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (character.state == Character.State.PreJumping)
        {
            var v = lineRenderer.GetPosition(1);
            v.y = -character.GetDistanceToFloor();
            lineRenderer.SetPosition(1, v);
        }
        else if (lineRenderer.GetPosition(1) != originalLastLinePos)
        {
            lineRenderer.SetPosition(1, originalLastLinePos);
        }
    }

    // void UpdatePositions()
    // {
    //     int xOffset = -2;
    //     for (int i = 0; i < linePositions.Length; ++i)
    //     {
    //         linePositions[i] = new Vector3(transform.position.x + xOffset, transform.position.y, 0f);
    //         xOffset++;
    //     }
    // }

    // void SetPositions()
    // {
    //     for (int i = 0; i < lineRenderer.positionCount; ++i)
    //     {
    //         lineRenderer.SetPosition(i, linePositions[i]);
    //     }
    // }
}
