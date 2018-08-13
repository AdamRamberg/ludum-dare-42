using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ScriptableObjectVariables;

public class Character : MonoBehaviour
{
    private Rigidbody2D rb;
    public ParticleSystem smoke;
    public ParticleSystem blood;
    private SpriteRenderer spriteRenderer;

    public CharacterStateVariable state;
    private KeyCode lastKey = KeyCode.None;

    private float runSpeed = 100f;

    private Transform jumpingTriggerTransform;
    private Vector2 velocityWhenJumping;
    private float percentJumpAreaReached;

    public FloatVariable maxAngularVelocity;
    public FloatVariable torquesWhenFalling;
    public FloatVariable preJumpTime;
    public Transform floor;


    public GameObject polePrefab;
    public UnityEvent didDie;

    public StringVariable gameState;

    private List<Action> actionsNextFixedUpdate = new List<Action>();

    private Animator animator;

    private const string SPRITE = "Sprite";
    private const string STATE = "State";
    private const string _Y_POSITION = "_YPosition";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = transform.Find(SPRITE).GetComponent<SpriteRenderer>();

        Reset();

        state.Changed += SetGameStateBasedOnCharState;
        gameState.Changed += SetGameStateBasedOnCharState;
    }

    private void OnDestroy()
    {
        state.Changed -= SetGameStateBasedOnCharState;
        gameState.Changed -= SetGameStateBasedOnCharState;
    }

    void SetGameStateBasedOnCharState(CharacterState charState) { SetGameStateBasedOnCharState(); }
    void SetGameStateBasedOnCharState(string newGameState) { SetGameStateBasedOnCharState(); }
    void SetGameStateBasedOnCharState()
    {
        if (!GameStateUtils.IsPlaying(gameState)) return;

        if (state.Value == CharacterState.Idle)
        {
            gameState.Value = GameStateConstants.PLAYING_RUNNING;
        }
        else if (CharacterStateUtils.IsJumping(state.Value))
        {
            gameState.Value = GameStateConstants.PLAYING_JUMPING;
        }
        else
        {
            gameState.Value = GameStateConstants.PLAYING;
        }
    }

    void Update()
    {
        if (!GameStateUtils.IsPlaying(gameState)) return;

        animator.SetInteger(STATE, (int)state.Value);
        Shader.SetGlobalFloat(_Y_POSITION, this.transform.position.y * 0.03f);
        rb.gravityScale = rb.velocity.y < -1f ? 3.5f : 1f;

        // For animation
        if (state.Value == CharacterState.Idle && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            state.Value = CharacterState.Running;
        }

        if (state.Value == CharacterState.Running)
        {
            if ((lastKey == KeyCode.None || lastKey == KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                actionsNextFixedUpdate.Add(AddRunSpeed);
                lastKey = KeyCode.LeftArrow;
                smoke.Play();
            }
            else if ((lastKey == KeyCode.None || lastKey == KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.RightArrow))
            {
                actionsNextFixedUpdate.Add(AddRunSpeed);
                lastKey = KeyCode.RightArrow;
                smoke.Play();
            }
        }
        else if (state.Value == CharacterState.CanJump && Input.GetKeyDown(KeyCode.Space) && jumpingTriggerTransform != null)
        {
            state.Value = CharacterState.PreJumping;
            actionsNextFixedUpdate.Add(DoPreJump);
            StartCoroutine(DelayAndJump());

            percentJumpAreaReached = (transform.position.x - (jumpingTriggerTransform.position.x - jumpingTriggerTransform.localScale.x / 2)) / jumpingTriggerTransform.localScale.x;
        }
        else if (state.Value == CharacterState.Falling)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                actionsNextFixedUpdate.Add(AddTorqueNForceFallingLeft);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                actionsNextFixedUpdate.Add(AddTorqueNForceFallingRight);
            }
        }
    }

    void FixedUpdate()
    {
        if (!GameStateUtils.IsPlaying(gameState)) return;

        rb.gravityScale = rb.velocity.y < -1f ? 3.5f : 1f;

        for (int i = 0; i < actionsNextFixedUpdate.Count; ++i)
        {
            actionsNextFixedUpdate[i]();
        }
        actionsNextFixedUpdate.Clear();

        ClampAngularVelocity();
    }

    void DoJump()
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
        state.Value = CharacterState.Falling;
    }
    void AddRunSpeed() { rb.AddForce(Vector2.right * runSpeed); }
    void AddTorqueNForceFallingLeft() { AddTorqueNForceFalling(KeyCode.LeftArrow); }
    void AddTorqueNForceFallingRight() { AddTorqueNForceFalling(KeyCode.RightArrow); }
    void AddTorqueNForceFalling(KeyCode keyCode)
    {
        var right = keyCode == KeyCode.RightArrow;
        rb.AddTorque((right ? -1 : 1) * torquesWhenFalling.Value);
        rb.AddForce((right ? 1 : -1) * Vector2.right * 2f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!GameStateUtils.IsPlaying(gameState)) return;

        if (collider.tag == TagConstants.JUMPING_TRIGGER)
        {
            jumpingTriggerTransform = collider.transform;
            state.Value = CharacterState.CanJump;
        }
        else if ((collider.tag == TagConstants.POLE && collider.transform.parent == null)
            || collider.tag == TagConstants.HINDER)
        {
            Die();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!GameStateUtils.IsPlaying(gameState)) return;

        if (collider.tag == TagConstants.JUMPING_TRIGGER)
        {
            jumpingTriggerTransform = null;

            if (state.Value == CharacterState.CanJump)
            {
                Die();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameStateUtils.IsPlaying(gameState)) return;

        if ((collision.gameObject.tag == TagConstants.FLOOR || collision.gameObject.tag == TagConstants.MATTRESS) && state.Value == CharacterState.Falling)
        {
            smoke.Play();
            Camera.main.GetComponent<CameraShake>().Shake(0.3f);

            // Prevent character from moving sideways when landing
            actionsNextFixedUpdate.Add(StopXVelocity);

            if (collision.gameObject.tag == TagConstants.MATTRESS)
            {
                state.Value = CharacterState.Landed;
                StartCoroutine(LandedOnMattress());
            }
            else if (collision.gameObject.tag == TagConstants.FLOOR)
            {
                Die();
            }
        }
    }

    private void StopXVelocity()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public void Reset()
    {
        spriteRenderer.enabled = true;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        state.Value = CharacterState.Idle;
    }

    void DoPreJump()
    {
        velocityWhenJumping = rb.velocity;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(0.25f, 0.65f) * 1500f);
    }

    IEnumerator DelayAndJump()
    {
        yield return new WaitForSeconds(preJumpTime.Value);
        state.Value = CharacterState.Jumping;
        actionsNextFixedUpdate.Add(DoJump);

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

    void Die()
    {
        blood.Emit(300);
        spriteRenderer.enabled = false;

        state.Value = CharacterState.Dead;
        if (didDie != null) didDie.Invoke();
    }

    IEnumerator LandedOnMattress()
    {
        yield return new WaitForSeconds(1f);

        if (state.Value == CharacterState.Landed)
        {
            var myPole = transform.Find(TagConstants.POLE);
            Instantiate(polePrefab, new Vector3(myPole.position.x, myPole.position.y, -5f), myPole.rotation);

            Reset();
        }
    }

    public float GetDistanceToFloor()
    {
        return transform.position.y - floor.position.y;
    }
}
