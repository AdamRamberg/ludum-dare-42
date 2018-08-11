using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectVariables;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public Vector2Variable sizeRange;
    public FloatVariable maxZoomCharacterVelocity;

    public float smoothTime = 0.1f;
    private float sizeVelocity = 0.0f;

    void Start()
    {
        GetComponent<Camera>().orthographicSize = sizeRange.Value.x;
    }

    void FixedUpdate()
    {
        var newPos = Vector2.Lerp(transform.position, target.position, 0.125f);
        var cameraPos = transform.position;
        cameraPos.x = newPos.x;
        cameraPos.y = newPos.y;
        transform.position = cameraPos;

        CalculateCameraSize();
    }

    float GetCameraSizeTarget()
    {
        if (target.position.y > 15f)
        {
            return sizeRange.Value.y;
        }
        var targetVelocityMagnitude = target.GetComponent<Rigidbody2D>().velocity.magnitude;
        var velocityPercent = Mathf.Clamp(targetVelocityMagnitude / maxZoomCharacterVelocity.Value, 0, 1);
        return ((sizeRange.Value.y - sizeRange.Value.x) * velocityPercent) + sizeRange.Value.x;
    }

    void CalculateCameraSize()
    {

        GetComponent<Camera>().orthographicSize = Mathf.SmoothDamp(GetComponent<Camera>().orthographicSize, GetCameraSizeTarget(), ref sizeVelocity, smoothTime); ;
    }
}
