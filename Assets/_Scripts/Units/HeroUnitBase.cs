using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;

public class HeroUnitBase : UnitBase
{
    private bool _canMove;
    private bool _isAiming;
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float jumpCooldownLength = 2f;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private LayerMask groundLayer;

    private float jumpCooldownCounter = -1f;
    private Vector2 leftJoystick, rightJoystick, lastFacedDirection;
    private float movementEuler, rotationEuler;

    private Rigidbody rb;
    private Animator animator;
    private string currentAnimation = "";

    // these subscribe the 'OnStateChanged' method to the OnBeforeStateChanged event. Whenever it triggers, so will the OnStateChanged method (I presume). 
    private void Awake() => ExampleGameManager.OnBeforeStateChanged += OnStateChanged;



    private void Start()
    {
        SetStats(ResourceSystem.Instance.GetExampleHero(0).BaseStats);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _canMove = true;
        _isAiming = false;


    }

    private void FixedUpdate()
    {
        if (jumpCooldownCounter > 0)
        {
            jumpCooldownCounter -= Time.deltaTime;
        }

        Movement();
        CheckAnimation();
    }

    private void Movement()
    {
        if (_canMove)
        {
            movementEuler = Mathf.Atan2(leftJoystick.x, leftJoystick.y) * Mathf.Rad2Deg;
            rotationEuler = Mathf.Atan2(rightJoystick.x, rightJoystick.y) * Mathf.Rad2Deg;

            rb.linearVelocity = new Vector3(leftJoystick.x * Stats.TravelDistance, rb.linearVelocity.y, leftJoystick.y * Stats.TravelDistance);

            if (!_isAiming && leftJoystick.magnitude > 0)
            {
                transform.rotation = Quaternion.Euler(0, movementEuler, 0);
                lastFacedDirection = leftJoystick;
            }
            else if (rightJoystick.magnitude > 0) 
            {
                transform.rotation = Quaternion.Euler(0, rotationEuler, 0);
                lastFacedDirection = rightJoystick;
            }
        }
    }

    private void CheckAnimation()
    {
        if (currentAnimation == "JumpBegin" || currentAnimation == "JumpLand" || currentAnimation.StartsWith("Dance"))
            return;

        if (currentAnimation == "JumpFall")
        {
            if (IsGrounded())
                ChangeAnimation("JumpLand", 0.2f);

                return;
        }

        if (!_isAiming) 
        {
            // ALWAYS Forward
            StrafeMovingAnimation("StrafeJog_F", "StrafeWalk_F");
            ChangeAnimationWeight(1);
        }
        else
        {
            ChangeAnimationWeight(1, 1);
            // It works!

            if (Mathf.Abs(Vector2.SignedAngle(leftJoystick, lastFacedDirection)) <= 22.5)
                {
                    // Forward strafe
                    StrafeMovingAnimation("StrafeJog_F", "StrafeWalk_F");
                }
                else if (Mathf.Abs(Vector2.SignedAngle(leftJoystick, lastFacedDirection)) <= 67.5f)
                {
                    if (Vector2.SignedAngle(leftJoystick, lastFacedDirection) > 0)
                    {
                        // Right forward strafe
                        StrafeMovingAnimation("StrafeJog_R45", "StrafeWalk_R45");
                    }
                    else
                    {
                        // Left forward strafe
                        StrafeMovingAnimation("StrafeJog_L45", "StrafeWalk_L45");
                    }
                }
                else if (Mathf.Abs(Vector2.SignedAngle(leftJoystick, lastFacedDirection)) <= 112.5f)
                {
                    if (Vector2.SignedAngle(leftJoystick, lastFacedDirection) > 0)
                    {
                        // Right strafe
                        StrafeMovingAnimation("StrafeJog_FR", "StrafeWalk_FR");
                    }
                    else
                    {
                        // Left strafe
                        StrafeMovingAnimation("StrafeJog_FL", "StrafeWalk_FL");
                    }
                }
                else if (Mathf.Abs(Vector2.SignedAngle(leftJoystick, lastFacedDirection)) <= 157.5f)
                {
                    if (Vector2.SignedAngle(leftJoystick, lastFacedDirection) > 0)
                    {
                        // Right backwards strafe
                        StrafeMovingAnimation("StrafeJog_R135", "StrafeWalk_R135");
                    }
                    else
                    {
                        // Left backwards strafe
                        StrafeMovingAnimation("StrafeJog_L135", "StrafeWalk_L135");
                    }
                }
                else
                {
                    // Backwards Strafe
                    StrafeMovingAnimation("StrafeJog_B", "StrafeWalk_B");
                }   
        }
    }

    private void StrafeMovingAnimation(string strafeJogDirection, string strafeWalkDirection)
    {
        if (leftJoystick.magnitude > 0.5f)
        {
            ChangeAnimation(strafeJogDirection);
        }
        else if (leftJoystick.magnitude > 0.1f)
        {
            ChangeAnimation(strafeWalkDirection);
        }
        else
        {
            if (!currentAnimation.StartsWith("Idle")) 
            {
                string animation = "Idle" + Random.Range(0, 7);
                ChangeAnimation(animation, 0.05f);
            }
        }
    }

    public void ChangeAnimation(string animation, float crossfade = 0.1f, float time = 0, int layer = 0)
    {
        if (time > 0) StartCoroutine(Wait());
        else Validate();

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time - crossfade);
            Validate();
        }

        void Validate()
        {
            if (animation != currentAnimation)
            {
                currentAnimation = animation;

                if (currentAnimation == "")
                    CheckAnimation();
                else
                    animator.CrossFade(animation, crossfade, layer);
            }
        }
    }

    private void ChangeAnimationWeight(int layerIndex, float weight = 0.01f)
    {
        if (animator.GetLayerWeight(layerIndex) != weight)
        {
            animator.SetLayerWeight(layerIndex, weight);
        }
    }

    private void OnDestroy() => ExampleGameManager.OnBeforeStateChanged -= OnStateChanged;

    private void OnStateChanged(GameState newState)
    {
        // turn based example.
        if (newState == GameState.PlayerDeath)
        { 
            _canMove = false;
            rb.linearVelocity = Vector3.zero;
            string animation = "Dance" + Random.Range(0, 6);
            ChangeAnimation(animation, 0.05f);
        }
        else _canMove = true;
    }
    public void RightJoyStick(InputAction.CallbackContext ctx) => rightJoystick = ctx.ReadValue<Vector2>();

    public void LeftJoyStick(InputAction.CallbackContext ctx1) => leftJoystick = ctx1.ReadValue<Vector2>();

    public void Aim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isAiming)
            {
                _isAiming = false;
                Debug.Log("IsAiming OFF");
            }
            else
            {
                _isAiming = true;
                Debug.Log("IsAiming ON");
            }
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Hold?");
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && jumpCooldownCounter < 0)
        {
            ChangeAnimation("JumpBegin");
            Debug.Log("Jump");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(new Vector3(0, jumpForce, 0));
            jumpCooldownCounter = jumpCooldownLength;
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

    }


}
