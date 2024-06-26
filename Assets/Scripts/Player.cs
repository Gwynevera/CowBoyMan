using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerKeyType
    {
        Keyboard,
        Controller
    }
    public PlayerKeyType pKeys;

    /*
    X        = Joystick Button 0
    Circle   = Joystick Button 1
    Square   = Joystick Button 2
    Triangle = Joystick Button 3
    L1       = Joystick Button 4
    R1       = Joystick Button 5
    L2       = Joystick Button 6
    R2       = Joystick Button 7
    Share    = Joystick Button 8
    Options  = Joystick Button 9
    L3       = Joystick Button 10
    R3       = Joystick Button 11
    PS       = Joystick Button 12
    */

    public class PlayerKeys
    {
        public KeyCode leftKey;
        public KeyCode rightKey;
        public KeyCode downKey;
        public KeyCode upKey;

        public KeyCode jumpKey;
        public KeyCode shootKey;

        public PlayerKeys()
        {

        }

        public PlayerKeys(PlayerKeyType p)
        {
            if (p == PlayerKeyType.Keyboard)
            {
                leftKey = KeyCode.A;
                rightKey = KeyCode.D;
                downKey = KeyCode.S;
                upKey = KeyCode.W;
                jumpKey = KeyCode.K;
                shootKey = KeyCode.J;
            }
            else if (p == PlayerKeyType.Controller)
            {
                jumpKey = KeyCode.Joystick1Button0;
                shootKey = KeyCode.Joystick1Button2;
            }
        }
    }
    PlayerKeys myKeys;

    [Header("Keys")]
    public bool keyLeft;
    public bool keyRight;
    public bool keyDown;
    public bool keyUp;

    string DPad_X = "DPad_X";
    string DPad_Y = "DPad_Y";
    string LStick_X = "LStick_X";
    string LStick_Y = "LStick_Y";
    string RStick_X = "RStick_X";
    string RStick_Y = "RStick_Y";

    public bool keyJump;
    public bool keyDash;
    public bool keyShoot;

    public bool prevJump;
    public bool prevDash;
    public bool prevShoot;

    public float minStickValue = 0.5f;

    Rigidbody2D rb;
    BoxCollider2D box;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Life & Death")]
    public int health = 20;
    public bool damage = false;
    public GameObject damageParticle;
    float damageTime = 0.45f;
    float damageTimer;
    float damageForce = 10;
    float damageGravity = 20;
    public Vector2 damageDir = Vector2.one;
    public bool invencible = false;
    float invTime = 1.5f;
    float invTimer;
    [Header("Blink")]
    float blinkTime = 0.025f;
    float blinkTimer;

    [Header("Shoot")]
    public GameObject shot;
    public GameObject shootFlash;
    public int shots;
    int maxShots = 3;
    public Transform shotSpawn;
    float shotFlashOffset = 0.25f;
    float originalX;
    float shootingTime = 1;
    float shootingTimer;
    public bool shooting;

    [Header("Volley")]
    public bool volley;
    public bool willVolley;
    public int volleyCount;
    int volleyLimit = 5;
    float keepVolleyTime = 0.35f;
    float keepVolleyTimer;
    float volleySpeedMult = 1.15f;

    [Header("Arms")]
    public GameObject normalArm;
    public GameObject shootArm;

    [Header("Direction")]
    public Vector2 dir = new Vector2();
    public int lastXdir;

    [Header("Speed")]
    float speed;
    float minSpeed = 1;
    float maxSpeed = 8;

    [Header("Accelerate")]
    float acceleration = 0.75f;

    [Header("Gravity")]
    float gravity = 1.55f;
    float holdJumpMult = 0.75f;

    [Header("Collisions")]
    public bool grounded = false;
    public bool wall = false;
    public bool roof = false;
    [SerializeField]
    LayerMask groundCollisionLayer;
    Vector2 normalHitboxOffset = new Vector2(0, -0.15f);
    Vector2 slideHitboxOffset = new Vector2(0, -0.575f);
    Vector2 normalHitboxSize = new Vector2(1.15f, 1.85f);
    Vector2 slideHitboxSize = new Vector2(1.35f, 1);

    [Header("Jump")]
    public bool jump;
    public bool endJump;
    float jumpSpeed = 15;
    float jumpHeight = 2.25f;
    float jumpTop;

    [Header("Slide")]
    public bool slide;
    bool keepSlideSpeed;
    float slideSpeed = 14;
    float slideTime = 0.5f;
    float slideTimer;

    [Header("Coyote")]
    public bool coyote = false;
    float coyoteTime = 0.15f;
    float coyoteTimer;

    [Header("Jump Buffer")]
    public bool jumpBuffer = false;
    float jBufferTime = 0.15f;
    float jBufferTimer = 0;

    // Start
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        myKeys = new PlayerKeys(pKeys);

        originalX = shotSpawn.transform.localPosition.x;
    }

    // Update
    void FixedUpdate()
    {
        // Check collisions
        roof = RoofCheck();

        #region Coyote
        // Can jump briefly after leaving the ground
        if (coyote)
        {
            coyoteTimer += Time.fixedDeltaTime;
            if (coyoteTimer > coyoteTime)
            {
                coyote = false;
            }
        }
        #endregion
        #region Jump Buffer
        // Can jump if press the button a bit after touching ground
        if (jumpBuffer)
        {
            jBufferTimer += Time.fixedDeltaTime;
            if (jBufferTimer > jBufferTime)
            {
                jumpBuffer = false;
            }
        }
        #endregion
        
        #region Inputs
        if (health > 0 && !damage)
        {
            prevDash = keyDash;
            prevJump = keyJump;
            prevShoot = keyShoot;

            // Input
            if (pKeys == PlayerKeyType.Keyboard)
            {
                keyRight = Input.GetKey(myKeys.rightKey);
                keyLeft = Input.GetKey(myKeys.leftKey);
                keyUp = Input.GetKey(myKeys.upKey);
                keyDown = Input.GetKey(myKeys.downKey);
            }
            else if (pKeys == PlayerKeyType.Controller)
            {
                keyRight = Input.GetAxis(DPad_X) >= minStickValue || Input.GetAxis(LStick_X) >= minStickValue;
                keyLeft = Input.GetAxis(DPad_X) <= -minStickValue || Input.GetAxis(LStick_X) <= -minStickValue;
                keyUp = Input.GetAxis(DPad_Y) >= minStickValue || Input.GetAxis(LStick_Y) >= minStickValue;
                keyDown = Input.GetAxis(DPad_Y) <= -minStickValue || Input.GetAxis(LStick_Y) <= -minStickValue;
            }

            keyJump = Input.GetKey(myKeys.jumpKey);
            keyShoot = Input.GetKey(myKeys.shootKey);
        }
        #endregion

        // Left & Right
        if (keyLeft && !keyRight)
        {
            dir.x = -1;
        }
        else if (!keyLeft && keyRight)
        {
            dir.x = 1;
        }
        else
        {
            dir.x = 0;
        }
        anim.SetBool("Run", dir.x != 0);

        // Turn Around
        if (lastXdir != dir.x)
        {
            anim.SetBool("Run", false);

            if (grounded)
            {
                speed /= 2;
            }

            if (speed < minSpeed)
            {
                speed = minSpeed;
            }
        }

        wall = WallCheck();
        if (wall)
        {
            // Stop player
            dir.x = 0;
        }

        // Up & Down
        if (keyUp && !keyDown)
        {
            dir.y = 1;
        }
        else if (!keyUp && keyDown)
        {
            dir.y = -1;
        }
        else
        {
            dir.y = 0;
        }
        
        // Sprite flip
        if (dir.x == 1)
        {
            sprite.flipX = true;
        }
        else if (dir.x == -1)
        {
            sprite.flipX = false;
        }
        lastXdir = sprite.flipX ? 1 : -1;
        shotSpawn.transform.localPosition = new Vector3(lastXdir * originalX, shotSpawn.transform.localPosition.y);

        #region Jump / Slide
        if (keyJump)
        {
            // Just pressed jump, not in the ground yet
            if (!prevJump && !grounded)
            {
                jumpBuffer = true;
                jBufferTimer = 0;
            }

            // Jump event                    // On the ground        // Slide situation
            if (!roof && (!prevJump || jumpBuffer) && (grounded || coyote) && (!keyDown || slide))
            {
                jump = true;
                // Setup max jump height
                jumpTop = transform.position.y + jumpHeight;
                jumpBuffer = false;

                // Speedy jump
                if (slide)
                {
                    slide = false;
                    keepSlideSpeed = true;
                }
            }
            // Slide event
            else if ((!prevJump || jumpBuffer) && grounded && keyDown)
            {
                slide = true;
                slideTimer = 0;
                shooting = false;
            }
        }
        // Jumping
        if (jump)
        {
            // Go up
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

            // Reached the max jump height
            if (transform.position.y >= jumpTop)
            {
                jump = false;
                endJump = keyJump;
                rb.velocity *= new Vector2(1, 0.75f);
            }

            // Key is not pressed anymore
            if (!keyJump || roof)
            {
                jump = false;
                rb.velocity *= new Vector2(1, 0.5f);
            }
        }
        // Jump reached max height, if key is pressed, lower gravity
        if (endJump)
        {
            if (!keyJump)
            {
                endJump = false;
            }
        }
        anim.SetBool("Jump", jump);

        // Slide
        if (slide)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            dir.x = lastXdir;

            slideTimer += Time.fixedDeltaTime;
            if (slideTimer >= slideTime && !roof)
            {
                slide = false;
            }

            // Lower Hitbox
            box.offset = slideHitboxOffset;
            box.size = slideHitboxSize;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // Normal Hitbox
            box.offset = normalHitboxOffset;
            box.size = normalHitboxSize;
        }

        anim.SetBool("Slide", slide);
        #endregion

        // Velocity
        if (dir.x != 0)
        {
            if (slide || keepSlideSpeed)
            {
                speed = slideSpeed;
            }
            else
            {
                if (speed < maxSpeed)
                {
                    speed += acceleration;
                }
                else
                {
                    speed = maxSpeed;
                }
            }
        }
        else
        {
            if (grounded && !slide)
            {
                speed = minSpeed;
            }
        }
        if (!damage)
        {
            rb.velocity = new Vector2(speed * dir.x, rb.velocity.y);
        }

        // Apply gravity
        if (!grounded && !jump && !damage)
        {
            if (endJump)
            {
                rb.velocity -= new Vector2(0, gravity*holdJumpMult);
            }
            else
            {
                rb.velocity -= new Vector2(0, gravity);
            }
        }

        #region Shoot
        // Show arm shooting
        if (shooting)
        {
            shootingTimer += Time.fixedDeltaTime;
            if (shootingTimer >= shootingTime)
            {
                shooting = false;
                volley = false;
            }

            // Not mashed lo suficientement rapid
            keepVolleyTimer += Time.fixedDeltaTime;
            if (keepVolleyTimer > keepVolleyTime)
            {
                willVolley = false;
                volleyCount = 0;
            }
        }

        // Shoot
        if (keyShoot)
        {
            if (!prevShoot && !slide)
            {
                // Shoot when can Shoot
                if (shots < maxShots)
                {
                    Shoot();
                }
            }
        }

        anim.SetBool("Shooting", shooting);

        if (dir.x != 0 || jump)
        {
            volley = false;
            willVolley = false;
            volleyCount = 0;
        }
        anim.SetBool("Volley", volley);
        #endregion

        grounded = GroundCheck();
        anim.SetBool("Ground", grounded);

        #region Damage
        if (damage)
        {
            damageTimer += Time.fixedDeltaTime;
            if (damageTimer >= damageTime)
            {
                damage = false;
                invencible = true;
                rb.gravityScale = 0;
            }
        }

        if (invencible)
        {
            invTimer += Time.fixedDeltaTime;

            blinkTimer += Time.fixedDeltaTime;
            if (blinkTimer >= blinkTime)
            {
                blinkTimer = 0;
                sprite.enabled = !sprite.enabled;
            }

            if (invTimer >= invTime)
            {
                invencible = false;
                sprite.enabled = true;
            }
        }
        anim.SetBool("Damage", damage);

        // 9 (Invencible) - 0 (Default)
        this.gameObject.layer = damage || invencible ? 9 : 0;
        #endregion

        #region Arm Animators
        normalArm.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
        shootArm.GetComponent<SpriteRenderer>().flipX = sprite.flipX;

        if (!shooting)
        {
            normalArm.GetComponent<SpriteRenderer>().enabled = sprite.enabled;
            shootArm.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            shootArm.GetComponent<SpriteRenderer>().enabled = sprite.enabled;
            normalArm.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (damage || slide)
        {
            shootArm.GetComponent<SpriteRenderer>().enabled = false;
            normalArm.GetComponent<SpriteRenderer>().enabled = false;
        }

        normalArm.GetComponent<Animator>().SetBool("Run", anim.GetBool("Run") && !anim.GetBool("Slide"));
        normalArm.GetComponent<Animator>().SetBool("Jump", anim.GetBool("Jump"));
        normalArm.GetComponent<Animator>().SetBool("Ground", anim.GetBool("Ground"));
        shootArm.GetComponent<Animator>().SetBool("Run", anim.GetBool("Run") && !anim.GetBool("Slide"));
        shootArm.GetComponent<Animator>().SetBool("Jump", anim.GetBool("Jump"));
        shootArm.GetComponent<Animator>().SetBool("Ground", anim.GetBool("Ground"));
        #endregion
    }

    public void Damage(int d, int x)
    {
        health -= d;
        sprite.flipX = x == 1 ? false : true;

        GameObject.Instantiate(damageParticle, this.transform);

        damage = true;
        damageDir.x = x;
        damageDir.y = 1;

        if (!slide && !roof)
        {
            rb.gravityScale = damageGravity;
            rb.AddForce(damageDir * damageForce, ForceMode2D.Impulse);
        }

        if (slide && !roof)
        {
            slide = false;
        }

        keyLeft = keyRight = false;
        jump = false;
        endJump = false;
        keepSlideSpeed = false;
        shooting = false;

        damageTimer = 0;
        invTimer = 0;
        blinkTimer = 0;

        if (health <= 0)
        {
            health = 0;
        }
    }

    void Shoot()
    {
        if (shots >= maxShots)
        {
            return;
        }

        shots++;
        GameObject s = Instantiate(shot, shotSpawn.position, transform.rotation, null);
        s.GetComponent<Shot>().dir = lastXdir;

        float rRange = 0.15f;
        float rX = Random.Range(-rRange, rRange);
        float rY = Random.Range(-rRange, rRange);
        GameObject f = Instantiate(shootFlash, shotSpawn.position - new Vector3(shotFlashOffset*lastXdir, 0) + new Vector3(rX, rY), transform.rotation, transform);
        f.GetComponent<SpriteRenderer>().flipX = sprite.flipX;

        shooting = true;
        shootingTimer = 0;

        volleyCount++;
        if (volleyCount >= volleyLimit)
        {
            volley = true;
        }

        willVolley = true;
        keepVolleyTimer = 0;

        anim.ResetTrigger("Shoot");
        anim.SetTrigger("Shoot");

        if (volley)
        {
            s.GetComponent<Shot>().vel *= volleySpeedMult;
        }
    }

    private bool GroundCheck()
    {
        bool prevGround = grounded;

        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, new Vector2(box.size.x, box.size.y/2), 0, Vector2.down, box.size.y/3.5f, groundCollisionLayer);

        bool ground = (r.collider != null && r.collider.tag == "Ground" && r.normal.y > 0);

        if (r.point.x != 0 && r.point.y != 0) Debug.DrawRay(r.point, Vector2.down, Color.red);

        if (ground)
        {
            endJump = false;
            
            if (!slide && !jump)
            {
                keepSlideSpeed = false;
            }
        }
        else
        {
            // Just left the ground
            if (prevGround)
            {
                if (!jump)
                {
                    coyote = true;
                    coyoteTimer = 0;
                    if (slide)
                    {
                        coyoteTimer += coyoteTime/2;
                    }

                    // Fall animation
                    anim.SetBool("Jump", true);
                }
            }

            if (slide)
            { 
                keepSlideSpeed = true;
            }
            slide = false;
        }
        return ground;
    }

    private bool WallCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size * new Vector2(0.5f, 1), 0, Vector2.right*dir.x, box.size.x/2, groundCollisionLayer);

        if (r.point.x != 0 && r.point.y != 0) Debug.DrawRay(r.point, Vector2.right * dir, Color.blue);

        return (r.collider != null && r.normal.x != dir.x && r.normal.x != 0);
    }

    private bool RoofCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size * new Vector2(1, 0.5f), 0, Vector2.up, box.size.y/2, groundCollisionLayer);

        if (r.point.x != 0 && r.point.y != 0) Debug.DrawRay(r.point, Vector2.up, Color.green);

        return r.collider != null;
    }
}
