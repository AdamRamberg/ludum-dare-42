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
        if (transform.parent == null) return;

        lineRenderer = GetComponent<LineRenderer>();
        character = transform.parent.GetComponent<Character>();

        var v = lineRenderer.GetPosition(1);
        originalLastLinePos = new Vector3(v.x, v.y, v.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null) return;

        if (character.state.Value == Character.CharacterState.PreJumping)
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
}
