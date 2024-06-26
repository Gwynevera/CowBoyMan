using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    bool keyLeft, keyRight, keyDown, keyUp;
    bool keyJump, keyDash, keyShoot;

    bool releasedJump, releasedDash;

    Rigidbody2D rb;
    BoxCollider2D box;
    Animator anim;
    SpriteRenderer sprite;

    int dir = -1;

    float speed = 0;
    float maxSpeed = 9;
    float acceleration = 0.70f;
    float deceleration = 0.95f;
    float airDeceleration = 0.95f;

    float gravity = 1.25f;
    bool grounded = false;
    bool wall = false;
    bool roof = false;
    [SerializeField]
    LayerMask groundCollisionLayer;

    bool jumpUp;
    float jumpEnd;
    float jumpHeight = 1.25f;
    float jumpSpeed = 16.5f;

    bool airDash, groundDash;
    bool canAirDash;
    float dashSpeed = 12;
    float dashTime = 0.35f;
    float dashTimer;
    bool keepDashSpeed;

    float spawnAfterimageTime = 0.035f;
    float spawnAfterimageTimer;

    bool coyote = false;
    float coyoteTime = 0.55f;
    float coyoteTimer;

    bool jumpBuffer = false;
    float jBufferTime = 0.35f;
    float jBufferTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wall = WallCheck();
        grounded = GroundCheck();
        anim.SetBool("Ground", grounded);
        roof = RoofCheck();

        if (coyote)
        {
            coyoteTimer += Time.fixedDeltaTime;
            if (coyoteTimer > coyoteTime)
            {
                coyote = false;
            }
        }

        if (jumpBuffer)
        {
            releasedJump = true;
            jBufferTimer += Time.fixedDeltaTime;
            if (jBufferTimer > jBufferTime)
            {
                jumpBuffer = false;
                if (!coyote)
                {
                    releasedJump = false;
                }
            }
        }

        #region Inputs
        keyUp = Input.GetKey(KeyCode.W);
        keyLeft = Input.GetKey(KeyCode.A);
        keyDown = Input.GetKey(KeyCode.S);
        keyRight = Input.GetKey(KeyCode.D);
        keyJump = Input.GetKey(KeyCode.K);
        keyDash = Input.GetKey(KeyCode.LeftShift);

        if (keyJump)
        {
            if ((grounded || coyote) && releasedJump)
            {
                releasedJump = false;
                grounded = false;

                coyote = false;
                jumpBuffer = false;

                jumpUp = true;
                jumpEnd = transform.position.y + jumpHeight;
                anim.SetBool("Jump", true);

                if (groundDash)
                {
                    groundDash = false;
                    anim.SetBool("Dash", false);
                    dashTimer = dashTime;
                    keepDashSpeed = true;
                }
            }

            if (!grounded && !jumpBuffer && jBufferTimer == 0)
            {
                jumpBuffer = true;
            }
        }
        else
        {
            releasedJump = false;
            jBufferTimer = 0;

            if (grounded)
            {
                releasedJump = true;
            }

            if (jumpUp)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y/2);
            }
            jumpUp = false;
        }

        if (keyDash)
        {
            if (releasedDash)
            {
                releasedDash = false;
                if (grounded)
                {
                    groundDash = true;
                }
                else if (canAirDash)
                {
                    airDash = true;
                    canAirDash = false;
                    keepDashSpeed = true;
                }

                if (groundDash || airDash)
                {
                    dashTimer = 0;
                    spawnAfterimageTimer = 0;
                    anim.SetBool("Dash", true);
                }
            }
        }
        else
        {
            releasedDash = true;
            groundDash = airDash = false;
            anim.SetBool("Dash", false);
            dashTimer = dashTime;
        }
        #endregion

        #region Movement
        if (keyLeft && !keyRight)
        {
            if (!airDash || dir == -1)
            {
                dir = -1;
            }
        }
        if (!keyLeft && keyRight)
        {
            if (!airDash || dir == 1)
            {
                dir = 1;
            }
        }

        if (!keyLeft && !keyRight)
        {
            if (speed > 0)
            {
                if (grounded)
                {
                    speed -= deceleration;
                }
                else
                {
                    speed -= airDeceleration;
                }
            }
            else if (speed < 0)
            {
                speed = 0;
            }
            anim.SetBool("Run", false);
        }
        else
        {
            if (speed < maxSpeed)
            {
                speed += acceleration;
            }
            else if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            anim.SetBool("Run", true);
        }
        #endregion

        GetComponent<SpriteRenderer>().flipX = dir == 1;

        #region Jump
        if (roof)
        {
            jumpUp = false;
        }

        if (!jumpUp)
        {
            float extraGravity = rb.velocity.y > 0 ? gravity/2 : 0;
            rb.velocity -= new Vector2(0, gravity /*+ extraGravity*/);
        }
        else
        {
            if (jumpUp)
            {
                if (transform.position.y > jumpEnd)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                    jumpUp = false;
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                }
            }
        }
        anim.SetBool("Jump", jumpUp);
        #endregion

        #region Dash
        if (groundDash || airDash)
        {
            speed = dashSpeed;

            if (airDash)
            {
                rb.velocity *= new Vector2(1, 0);
            }

            dashTimer += Time.fixedDeltaTime;
            if (dashTimer >= dashTime)
            {
                groundDash = airDash = false;
                anim.SetBool("Dash", false);
            }
        }
        if (keepDashSpeed && (keyLeft || keyRight))
        {
            speed = dashSpeed;
        }
        if (groundDash || airDash || keepDashSpeed)
        {

            spawnAfterimageTimer += Time.fixedDeltaTime;
            if (spawnAfterimageTimer > spawnAfterimageTime)
            {
                spawnAfterimageTimer = 0;

                GameObject afterImage = new GameObject();
                afterImage.transform.position = transform.position;
                afterImage.AddComponent<SpriteRenderer>();
                afterImage.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
                afterImage.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
                afterImage.GetComponent<SpriteRenderer>().color = Color.red;
                afterImage.GetComponent<SpriteRenderer>().sortingOrder = -1;
                afterImage.AddComponent<DestroyAfter>();
                afterImage.GetComponent<DestroyAfter>().timeToDie = 0.5f;
            }
        }
        #endregion

        if (wall)
        {
            speed = 0;
        }

        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
    }

    private bool GroundCheck()
    {
        bool prevGround = grounded;

        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, new Vector2(box.size.x, box.size.y/2), 0, Vector2.down, 1, groundCollisionLayer);

        bool yeah = (r.collider != null && r.collider.tag == "Ground" && r.normal.y > 0);
        
        if (yeah)
        {
            if (!prevGround)
            {
                canAirDash = true;
                if (!jumpUp)
                {
                    keepDashSpeed = false;
                }

                if (rb.velocity.y == 0)
                {
                    anim.SetBool("Jump", false);
                }
            }
        }
        else
        {
            if (prevGround)
            {
                coyote = true;
                coyoteTimer = 0;
            }
        }
        return yeah;
    }

    private bool WallCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.right*dir, 1, groundCollisionLayer);
        return (r.collider != null && r.normal.x != dir && r.normal.x != 0);
    }

    private bool RoofCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, new Vector2(box.size.x, box.size.y / 2), 0, Vector2.up, 1, groundCollisionLayer);

        return r.collider != null;
    }
}
