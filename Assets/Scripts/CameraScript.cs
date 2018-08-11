using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;

    void FixedUpdate()
    {
        var newPos = Vector2.Lerp(transform.position, target.position, 0.125f);
        var cameraPos = transform.position;
        cameraPos.x = newPos.x;
        cameraPos.y = newPos.y;
        transform.position = cameraPos;
    }
}
