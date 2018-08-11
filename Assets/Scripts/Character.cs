using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ScriptableObjectVariables;

public class Character : MonoBehaviour
{

    public enum State
    {
        Running,
        CanJump,
        PreJumping,
        Jumping,

        Falling,
        Landed,
        Dead
    };

    public State state = State.Running;
    private KeyCode lastKey = KeyCode.None;

    private float runSpeed = 100f;

    private const string JUMPING_TRIGGER = "JumpingTrigger";
    private const string MATTRESS = "Mattress";
    private const string FLOOR = "Floor";
    private const string POLE = "Pole";

    private Transform jumpingTriggerTransform;
    private Vector2 velocityWhenJumping;
    private float percentJumpAreaReached;

    public FloatVariable maxAngularVelocity;
    public FloatVariable torquesWhenFalling;
    public FloatVariable preJumpTime;
    public Transform floor;

    private Rigidbody2D rb;

    public GameObject polePrefab;
    public UnityEvent didDie;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Reset()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        state = State.Running;
    }

    void Update()
    {
        rb.gravityScale = rb.velocity.y < -1f ? 3.5f : 1f;

        if (state == State.Running)
        {
            if ((lastKey == KeyCode.None || lastKey == KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                rb.AddForce(Vector2.right * runSpeed);
                lastKey = KeyCode.LeftArrow;
            }
            else if ((lastKey == KeyCode.None || lastKey == KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.RightArrow))
            {
                rb.AddForce(Vector2.right * runSpeed);
                lastKey = KeyCode.RightArrow;
            }
        }
        else if (state == State.CanJump && Input.GetKeyDown(KeyCode.Space) && jumpingTriggerTransform != null)
        {
            state = State.PreJumping;
            velocityWhenJumping = rb.velocity;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0.25f, 0.65f) * 1500f);
            StartCoroutine(PreJump());

            percentJumpAreaReached = (transform.position.x - (jumpingTriggerTransform.position.x - jumpingTriggerTransform.localScale.x / 2)) / jumpingTriggerTransform.localScale.x;
        }
        else if (state == State.Falling)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb.AddTorque(torquesWhenFalling.Value);
                rb.AddForce(-Vector2.right * 2f);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.AddTorque(-torquesWhenFalling.Value);
                rb.AddForce(Vector2.right * 2f);
            }
        }
    }

    IEnumerator PreJump()
    {
        yield return new WaitForSeconds(preJumpTime.Value);
        state = State.Jumping;
    }

    void ClampAngularVelocity()
    {
        if (rb.angularVelocity > maxAngularVelocity.Value)
        {
            rb.angularVelocity = maxAngularVelocity.Value;
        }
        else if (rb.angularVelocity < -maxAngularVelocity.Value)
        {
            rb.angularVelocity = -maxAngularVelocity.Value;
        }
    }

    void FixedUpdate()
    {
        ClampAngularVelocity();

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

            var velocity = rb.velocity;
            velocity.y = velocityWhenJumping.x * 1.4f;
            velocity.x = velocityWhenJumping.x * yVeloMultiplier * 0.5f;
            rb.velocity = velocity;
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
        else if (collider.tag == POLE && collider.transform.parent == null)
        {
            state = State.Dead;
            if (didDie != null) didDie.Invoke();
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

    public float GetDistanceToFloor()
    {
        return transform.position.y - floor.position.y;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == FLOOR || collision.gameObject.tag == MATTRESS) && state == State.Falling)
        {
            if (collision.gameObject.tag == MATTRESS)
            {
                state = State.Landed;
                StartCoroutine(LandedOnMattress());
            }
            else if (collision.gameObject.tag == FLOOR)
            {
                state = State.Dead;
                if (didDie != null) didDie.Invoke();
            }
        }
    }

    IEnumerator LandedOnMattress()
    {
        yield return new WaitForSeconds(1f);

        if (state == State.Landed)
        {
            var myPole = transform.Find(POLE);
            Instantiate(polePrefab, myPole.position, myPole.rotation);

            Reset();
        }
    }
}
