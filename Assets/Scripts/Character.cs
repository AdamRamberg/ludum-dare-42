using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public enum State
    {
        Running,
        CanJump,
        PreJumping,
        Jumping,

        Falling
    };

    public State state = State.Running;
    private KeyCode lastKey = KeyCode.None;

    private float runSpeed = 100f;

    private const string JUMPING_TRIGGER = "JumpingTrigger";
    private Transform jumpingTriggerTransform;
    private Vector2 velocityWhenJumping;
    private float percentJumpAreaReached;

    void Start()
    {

    }

    void Update()
    {
        if (state == State.Running)
        {
            if ((lastKey == KeyCode.None || lastKey == KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.right * runSpeed);
                lastKey = KeyCode.LeftArrow;
            }
            else if ((lastKey == KeyCode.None || lastKey == KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.RightArrow))
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.right * runSpeed);
                lastKey = KeyCode.RightArrow;
            }
        }
        else if (state == State.CanJump && Input.GetKeyDown(KeyCode.Space) && jumpingTriggerTransform != null)
        {
            state = State.PreJumping;
            velocityWhenJumping = GetComponent<Rigidbody2D>().velocity;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0.25f, 0.65f) * 1500f);
            StartCoroutine(PreJump());

            percentJumpAreaReached = (transform.position.x - (jumpingTriggerTransform.position.x - jumpingTriggerTransform.localScale.x / 2)) / jumpingTriggerTransform.localScale.x;
        }
    }

    IEnumerator PreJump()
    {
        yield return new WaitForSeconds(0.3f);
        state = State.Jumping;
    }

    void FixedUpdate()
    {
        if (state == State.Jumping)
        {
            // if between 0 and 0.6 => linear percent between 1 and 0.05
            // else if more than 0.6 => linear percent between 0.05 and -0.05
            // #mathgenius #mensa
            var yVeloMultiplier = percentJumpAreaReached <= 0.6f ?
                (1 - (percentJumpAreaReached / 0.6f)) * 0.95f + 0.05f
            :
                (1 - ((percentJumpAreaReached - 0.6f) / 0.4f)) * 0.1f - 0.05f
            ;

            var velocity = GetComponent<Rigidbody2D>().velocity;
            velocity.y = velocityWhenJumping.x * 1.4f;
            velocity.x = velocityWhenJumping.x * yVeloMultiplier * 0.5f;
            GetComponent<Rigidbody2D>().velocity = velocity;
            state = State.Falling;
        }
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == JUMPING_TRIGGER)
        {
            jumpingTriggerTransform = collider.transform;
            state = State.CanJump;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == JUMPING_TRIGGER && state == State.Running)
        {
            jumpingTriggerTransform = null;
            state = State.Falling;
        }
    }
}
