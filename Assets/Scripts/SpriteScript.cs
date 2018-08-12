using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScript : MonoBehaviour
{
    private Character character;

    void Start()
    {
        character = transform.parent.GetComponent<Character>();
    }

    void Update()
    {
        if (character.state.Value == CharacterState.PreJumping)
        {
            transform.Rotate(new Vector3(0f, 0f, 2f));
        }
        else if (transform.localRotation != Quaternion.identity)
        {
            // Debug.Log(123);
            // var rot = transform.rotation;
            // rot.eulerAngles = Vector3.zero;
            // transform.rotation = rot;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.time * 0.1f);
        }
    }
}
