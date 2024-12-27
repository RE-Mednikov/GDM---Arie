using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed;
    public float jumpSpeed;
    public Transform groundCheck;
    public LayerMask ground;
    public bool onGround;
    public bool onWall;
    public float climbSpeed;
    private float inputX;
    private float direction;
    public float wallJumpSpeed;
    public GameObject wallJumpEffect;
    private float wallJumpTimer;
    public Transform playerSprite;
    private float wallRunTime;
    public float attackSpeed;
    public static float attacking;
    public GameObject attackEffect;
    public float bounceSpeed;
    public GameObject climbEffect;
    public static int wallSide;
    public float glideAmount;
    private SpriteRenderer sprite;
    public Sprite normal;
    public Sprite run;
    public Sprite attackingSprite;
    public Sprite slide;
    public Transform axe;
    public static bool canDash;
    public Transform aimer;
    public ParticleSystem hit;
    public Animation hitAnimation;
    public static Vector3 playerPosition;
    public float grappleSpeed;
    private LineRenderer grappleString;
    private float grappling;
    public Transform grapplePoint;

    // Start is called before the first frame update
    void Start()
    {
        grappleString = gameObject.GetComponent<LineRenderer>();
        sprite = playerSprite.GetComponent<SpriteRenderer>();
        StartCoroutine(runCycle());
    }

    IEnumerator runCycle(){
        while(true){
            if(inputX == 1){
                sprite.flipX = false;
            }
            else if(inputX == -1){
                sprite.flipX = true;
            }

            if(grappling > 0){
                sprite.sprite = run;
            }
            else if(attacking > -0.1f){
                //attacking
                sprite.sprite = attackingSprite;
            }
            else if(wallRunTime < 0 && onGround == false){
                if(onWall == true && inputX == wallSide){
                    //sliding down wall
                    if(inputX == 1){
                        sprite.flipX = true;
                    }
                    else if(inputX == -1){
                        sprite.flipX = false;
                    }
                    sprite.sprite = slide;
                }
                else{
                    //in air
                    sprite.sprite = run;
                }
            }
            else if(inputX != 0){
                //run
                switchSprite();
                yield return new WaitForSeconds(0.08f);
            }
            else{
                //stand
                sprite.sprite = normal;
            }

            yield return null;
        }
    }

    void switchSprite(){
        if(sprite.sprite == normal){
            sprite.sprite = run;
        }
        else{
            sprite.sprite = normal;
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = transform.position;
        //get horizontal input and find direction of movement
        inputX = Input.GetAxisRaw("Horizontal");
        direction = rb.velocity.x / Mathf.Abs(rb.velocity.x);

        //movement code (wall running, jumping, moving)
        if(Input.GetKey(KeyCode.LeftShift)){
            //grappling
            Vector3 aim = findClosest("Grapple Point");

            if(aim.y > transform.position.y && Vector3.Distance(transform.position, aim) < 15){
                playerSprite.transform.up = new Vector3(transform.position.x - aim.x, transform.position.y - aim.y) * -1;
                grapple(aim);
                if(grappleString.enabled == false){
                    rb.velocity *= 1.4f;
                    grappleString.enabled = true;
                }
            }
            else{
                grappling = -0.01f;
            }
        }
        
        if(grappling < 0){
            if(attacking < 0){
                attackEffect.SetActive(false);
                move();
                if(attacking < -0.1f){
                    gameObject.tag = "Player";
                }
                grappleString.enabled = false;
            }
            else{
                axe.eulerAngles = new Vector3(0, 0, -179);
                axe.transform.position = transform.position + new Vector3(0, -1);
                attackEffect.SetActive(true);
                gameObject.tag = "Attack";
                grappleString.enabled = false;
            }
        }

        //sprite code (rotations, sprite changes)
        setSprite();

        //attacking code(enemy targetting, attack movement)
        attack();
        
        //glide();
        grappling -= Time.deltaTime;
    }

    void grapple(Vector3 aim){
        //grapple
        grappling = 0.01f;
        float distConstant = 1 + (Vector3.Distance(transform.position, aim) / 30);

        rb.AddForce(playerSprite.up * grappleSpeed * Time.deltaTime * 100 * distConstant);
        rb.gravityScale = 2;
        rb.drag = 0;
        grappleString.SetPosition(0, grapplePoint.position);
        grappleString.SetPosition(1, aim);
        //grappleString.widthMultiplier = 0.5f + Vector3.Distance(transform.position, aim) / 10;
    }

    void attack(){
        Vector3 aim = findClosest("Enemy");

        if(aim != Vector3.zero && Vector3.Distance(transform.position, aim) < 15){
            aimer.position = aim;
            aimer.up = new Vector3(transform.position.x - aim.x, transform.position.y - aim.y, 0);
        }
        else{
            aimer.position = new Vector3(0, -1000, 0);
        }

        attacking -= Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Space) && attacking < 0 && canDash == true){
            //zero vector is the default return when there is no targetable enemies (dont attack)
            if(aim != Vector3.zero && Vector3.Distance(transform.position, aim) < 15){
                canDash = false;
                attacking = 0.15f;
                //aim towards target
                transform.up = new Vector3(aim.x - transform.position.x, aim.y - transform.position.y);
                //axe.eulerAngles = new Vector3(0, 0, 180);
            }
        }

        if(attacking > 0){
            rb.velocity = transform.up * attackSpeed * (1 - attacking * 0.25f);
            playerSprite.up = transform.up;
        }
        else if(attacking > -0.1f){
            rb.drag = 6;
        }
        else{
            transform.up = Vector3.zero;
        }
    }

    Vector3 findClosest(string tagName){
        //get enemies
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tagName);

        if(targets.Length != 0){  
            Vector3 closestPosition = targets[0].transform.position;
            float closest = Vector3.Distance(transform.position, targets[0].transform.position);

            //find the closest one
            foreach(GameObject check in targets){
                float d = Vector3.Distance(check.transform.position, transform.position);
                if(d < closest){
                    closest = d;
                    closestPosition = check.transform.position;
                }
            }

            //return the position of the closest enemy
            return closestPosition;
        }
        else{
            return new Vector3(0, 0, 0);
        }
    }

    void setSprite(){
        //rotate sprite when wallrunnning
        if(wallRunTime > 0){
            lerpRotate(playerSprite, 75 * inputX, 10);
            climbEffect.SetActive(true);
        }
        else{
            lerpRotate(playerSprite, rb.velocity.x * -2, 10);
            climbEffect.SetActive(false);
        }

        playerSprite.localScale = Vector3.Lerp(playerSprite.localScale, new Vector3(0.25f, 0.25f, 0), Time.deltaTime * 3);
    }

    void move(){
        wallSide = 0;
        
        //check for ground under player using raycast
        if(Physics2D.CircleCast(groundCheck.position, 0.5f, Vector2.zero, 0, ground) == true){
            onGround = true;
        }
        else{
            onGround = false;
        }

        //movement on ground vs. in air
        if(onGround == true){
            canDash = true;
            rb.drag = 3;
            rb.velocity += new Vector2(inputX * moveSpeed * Time.deltaTime, 0);

            //faster turning
            if(direction != inputX){
                rb.velocity += new Vector2(inputX * moveSpeed * Time.deltaTime, 0);
            }

            //jumping
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                playerSprite.localScale = new Vector3(0.2f, 0.3f);
            }

            onWall = false;
        }
        else{
            rb.drag = 4;
            
            //slower movement
            rb.velocity += new Vector2(inputX * moveSpeed * Time.deltaTime, 0);

            //wall run
            //check for walls
            RaycastHit2D wall = Physics2D.BoxCast(transform.position, new Vector2(2.5f, 1), 0, Vector2.zero, 0, ground);
            if(wall == true){
                canDash = true;
                onWall = true;

                if(wall.transform.position.x < transform.position.x){
                    wallSide = -1;
                }
                else{
                    wallSide = 1;
                }

                wallRun();
            }
            else{
                onWall = false;
            }
        }

        //nicer feeling jump
        if(rb.velocity.y > -1){
            rb.gravityScale = 4;
        }
        else{
            rb.gravityScale = 10;
        }

        wallJumpTimer -= Time.deltaTime;
        wallRunTime -= Time.deltaTime;
    }

    void wallRun(){
        if(onWall == true){
            //climb
            if(Input.GetKey(KeyCode.UpArrow) && inputX != 0){
                //test the rest of the code that the player is wall running
                wallRunTime = 0.05f;
                //makes sure the player doesnt jump off a wall twice
                if(wallJumpTimer < 0){
                    rb.velocity = new Vector2(rb.velocity.x, climbSpeed);

                    //if the player inputs the other direction of the wall, it jumps
                    if(inputX != wallSide && inputX != 0){
                        wallJumpTimer = 0.1f;
                        rb.velocity = new Vector2(inputX * wallJumpSpeed, 20);

                        //making effect
                        GameObject effect = Instantiate(wallJumpEffect, transform.position, Quaternion.identity);
                        //rotating effect: effect.transform.Rotate(0, 0, inputX * -60);
                    }
                }
            }
            else{
                //slow down if the player is not climbing (sliding down the wall)
                if(inputX == wallSide){
                    if(rb.velocity.y < 0){
                        rb.velocity *= new Vector2(1, 0.99f);
                    }
                }
            }
        }
    }

    void lerpRotate(Transform setter, float angle, float speed){
        //this function basically rotates the object smoothly to a new angle
        //rotation gets really complicated with quaternions, and when you lerp an angle,
        //it gets mixed up with 180 and -180. This function just uses quaternions in a weird
        //way to avoid it

        Vector3 originalAngle = setter.eulerAngles;
        setter.eulerAngles = new Vector3(0,0, angle);
        Quaternion to = setter.rotation;

        setter.eulerAngles = originalAngle;
        setter.rotation = Quaternion.SlerpUnclamped(setter.rotation, to, Time.deltaTime * speed);
    }

    void glide(){
        if(Input.GetKey(KeyCode.UpArrow)){
            if(rb.velocity.y < 0){
                rb.velocity *= new Vector3(1, 1- Time.deltaTime * glideAmount);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy Attack"){
            getHit(other.transform.position);
        }
    }

    void getHit(Vector3 other){
        Time.timeScale = 0.1f;
        playerSprite.up = new Vector3(transform.position.x - other.x, transform.position.y - other.y);
        hit.Play();
        CameraScript.shake(2);
        hitAnimation.Play();
        //rb.velocity = playerSprite.up * 40;
        //attacking = -0.2f;
    }
}
