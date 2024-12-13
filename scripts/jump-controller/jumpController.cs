// Adapted from Platformer ToolKit: https://gmtk.itch.io/platformer-toolkit

using UnityEngine;

//This script handles moving the character on the Y axis, for jumping and gravity

public class jumpController : MonoBehaviour
{
    [Header("Components")]
    [HideInInspector] public Rigidbody2D body;
    private characterGround ground;
    [HideInInspector] public Vector2 velocity;

    [Header("Jumping Stats")]
    [SerializeField, Range(2f, 5.5f)][Tooltip("Maximum jump height")] public float jumpHeight = 7.3f;


//If you're using your stats from Platformer Toolkit with this character controller, please note that the number on the Jump Duration handle does not match this stat
//It is re-scaled, from 0.2f - 1.25f, to 1 - 10.
//You can transform the number on screen to the stat here, using the function at the bottom of this script



    [SerializeField, Range(0.2f, 1.25f)][Tooltip("How long it takes to reach that height before coming back down")] public float timeToJumpApex;
    [SerializeField, Range(0f, 5f)][Tooltip("Gravity multiplier to apply when going up")] public float upwardMovementMultiplier = 1f;
    [SerializeField, Range(1f, 10f)][Tooltip("Gravity multiplier to apply when coming down")] public float downwardMovementMultiplier = 6.17f;
    [SerializeField, Range(0, 1)][Tooltip("How many times can you jump in the air?")] public int maxAirJumps = 0;

    [Header("Options")]
    [Tooltip("Should the character drop when you let go of jump?")] public bool variablejumpHeight = true;
    [SerializeField, Range(1f, 10f)][Tooltip("Gravity multiplier when you let go of jump")] public float jumpCutOff;
    [SerializeField][Tooltip("The fastest speed the character can fall")] public float speedLimit;
    [SerializeField, Range(0f, 0.3f)][Tooltip("How long should coyote time last?")] public float coyoteTime = 0.15f;
    [SerializeField, Range(0f, 0.3f)][Tooltip("How far from ground should we cache your jump?")] public float jumpBuffer = 0.15f;

    [Header("Calculations")]
    public float jumpSpeed;
    private float defaultGravityScale;
    public float gravMultiplier;

    [Header("Current State")]
    public bool canJumpAgain = false;
    private bool desiredJump;
    private float coyoteTimeCounter = 0;
    private bool pressingJump;
    public bool onGround;
    private bool isJumping;

    void Awake()
    {
        //Find the character's Rigidbody and ground detection and juice scripts

        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<characterGround>();
        defaultGravityScale = 1f;
       // timeToJumpApex = scale(1, 10, 0.2f, 2.5f, numberFromPlatformerToolkit)
        
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            desiredJump = true;
            pressingJump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space) && velocity.y > 0f)
        {
             pressingJump = false;
        }
        
        setPhysics();

        //Check if we're on ground, using Kit's Ground script
        onGround = ground.GetOnGround();
        

        //If we're not on the ground and we're not currently jumping, that means we've stepped off the edge of a platform.
        //So, start the coyote time counter...
        if (!isJumping && !onGround)
        {
            coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            //Reset it when we touch the ground, or jump
            coyoteTimeCounter = 0;
        }
    }

    private void setPhysics()
    {
        //Determine the character's gravity scale, using the stats provided. Multiply it by a gravMultiplier, used later
        Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
        body.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravMultiplier;
    }

    private void FixedUpdate()
    {
        //Get velocity from Kit's Rigidbody 
        velocity = body.velocity;

        //Keep trying to do a jump, for as long as desiredJump is true
        if (desiredJump)
        {
            DoAJump();
            body.velocity = velocity;

            //Skip gravity calculations this frame, so isJumping doesn't turn off
            //This makes sure you can't do the coyote time double jump bug
            return;
        }
        calculateGravity();
    }

    private void calculateGravity()
    {
        if (onGround)
        {
            gravMultiplier = defaultGravityScale;
            isJumping = false;
        }
      
        
        if (body.velocity.y > 0.01f)
        {
            //If we're using variable jump height...)
            if (variablejumpHeight)
            {
                //Apply upward multiplier if player is rising and holding jump
                if (pressingJump && isJumping)
                {
                    gravMultiplier = upwardMovementMultiplier;
                }
                //But apply a special downward multiplier if the player lets go of jump
                else
                {
                    gravMultiplier = jumpCutOff;
                }
            }
            else
            {
                gravMultiplier = upwardMovementMultiplier;
            }
        }
        //Else if going down...
        else if (body.velocity.y < -0.01f)
        {

            //Otherwise, apply the downward gravity multiplier as Kit comes back to Earth
            gravMultiplier = downwardMovementMultiplier;

        }
        
        
        body.velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -speedLimit, 100));
    }

    private void DoAJump()
    {

        //Create the jump, provided we are on the ground, in coyote time, or have a double jump available
        if (onGround || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < coyoteTime) || canJumpAgain)
        {
            desiredJump = false;
            coyoteTimeCounter = 0;

            //If we have double jump on, allow us to jump again (but only once)
            canJumpAgain = (maxAirJumps == 1 && canJumpAgain == false);

            //Determine the power of the jump, based on our gravity and stats
            jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * body.gravityScale * jumpHeight);

            //If Kit is moving up or down when she jumps (such as when doing a double jump), change the jumpSpeed;
            //This will ensure the jump is the exact same strength, no matter your velocity.
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            else if (velocity.y < 0f)
            {
                jumpSpeed += Mathf.Abs(body.velocity.y);
            }

            //Apply the new jumpSpeed to the velocity. It will be sent to the Rigidbody in FixedUpdate;
            velocity.y += jumpSpeed;
            isJumping = true;
            
        }

        if (jumpBuffer == 0)
        {
            //If we don't have a jump buffer, then turn off desiredJump immediately after hitting jumping
            desiredJump = false;
        }
    }

    public void BounceUp(float bounceAmount)
    {
        //Used by the springy pad
        body.AddForce(Vector2.up * bounceAmount, ForceMode2D.Impulse);
    }
    

  public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }






}