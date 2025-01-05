using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{
    public string enemyType;
    public bool rotater;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask ground;
    private bool onGround;
    private float jumpCounter;
    float direction;
    public float jumpSpeed;
    public SpriteRenderer mySprite;
    public GameObject projectile;
    public GameObject projectile2;
    public Transform shotPoint;
    public Transform Player;
    public GameObject weapon;
    public float moveSpeed;
    private float aimPosition;
    private Vector2 originalPosition;
    private int attackCounter;
    private bool slamming;

    // Start is called before the first frame update
    void Start()
    {
        direction = 1;
        originalPosition = transform.position;
        aimPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics2D.CircleCast(groundCheck.position, 0.5f, Vector2.zero, 0, ground) == true){
            onGround = true;
        }
        else{
            onGround = false;
        }

        jumpCounter -= Time.deltaTime;

        //enemy attack
        if(enemyType == "Croc"){
            crocAttack();
        }
        else if(enemyType == "Whitch"){
            whitchAttack();
        }

        //aiming
        if(rotater == true){
            lerpRotate(transform, 90 * direction, 10);
        }
        else{
            if(direction == 1){
                mySprite.flipX = true;
            }
            else{
                mySprite.flipX = false;
            }
        }


        //deactivate weapon after getting hit for a split second
        if(gameObject.layer == 6){
            weapon.layer = 6;
        }
        else{
            weapon.layer = 5;
        }
    }

    void crocAttack(){
        if(onGround == true && jumpCounter < 0){
            jumpCounter = 2;
            //attack
            //aim
            aimAtPlayer();

            if(Random.Range(1, 4) == 1){
                //jump
                rb.velocity = new Vector3(-10 * direction, jumpSpeed);
                //jumpCounter = 1;
            }
            else if (Random.Range(1, 3) == 1){
                //small jump
                rb.velocity = new Vector3(15 * direction, jumpSpeed);
                StartCoroutine(shoot(0.3f));
                //jumpCounter = 1.5f;
            }
            else
            {
                rb.velocity = new Vector2(direction * -30, 0);
                CameraScript.shake(0.2f);
                //dash
            }
        }
    }

    void whitchAttack(){
        //move
        if(jumpCounter < 2.5f){
            rb.velocity += new Vector2(direction * -moveSpeed * Time.deltaTime, 0);
        }
        
        if(slamming == true){
            //slam attack
            rb.gravityScale = 10;
        }
        else{
            rb.gravityScale = 0;
            transform.position = Vector3.Lerp(transform.position, new Vector2(transform.position.x, aimPosition), Time.deltaTime * 1.5f);
        }

        //switch directions + change y position
        if(jumpCounter < 0){
            direction *= -1;
            jumpCounter = 3;

            aimPosition = Random.Range(-1, 1) * 8 + originalPosition.y;

            attackCounter++;
        }

        //attack
        if(attackCounter >= 2){
            attackCounter = Random.Range(0, 2);

            jumpCounter = 4;
            if(Random.Range(1, 3) == 1){
                //triple fire ball
                
                //transform.up = new Vector3(Player.position.x - transform.position.x, Player.position.y - transform.position.y);
                StartCoroutine(shoot(0.5f));
                StartCoroutine(shoot(0.7f));
                StartCoroutine(shoot(0.9f));
            }
            else{
                //ground slam??
                rb.velocity= new Vector3(direction * 5, 60);
                slamming = true;
            }
        }
    }

    IEnumerator shoot(float waitTime){
        yield return new WaitForSeconds(waitTime);
        Instantiate(projectile, shotPoint.position, Quaternion.identity);
    }

    void aimAtPlayer(){
        if(transform.position.x > Player.position.x){
            direction = 1;
        }
        else{
            direction = -1;
        }
    }

    void lerpRotate(Transform setter, float angle, float speed){

        Vector3 originalAngle = setter.eulerAngles;
        setter.eulerAngles = new Vector3(0,0, angle);
        Quaternion to = setter.rotation;

        setter.eulerAngles = originalAngle;
        setter.rotation = Quaternion.SlerpUnclamped(setter.rotation, to, Time.deltaTime * speed);
    }

    void OnCollisionEnter2D(Collision2D other){
        if(slamming == true){
            //end slam
            slamming = false;
            attackCounter = 0;
            CameraScript.shake(0.5f);

            //shoot projectiles
            transform.eulerAngles = new Vector3(0, 0, 90);
            int counter = 0;
            
            while(counter < 5){
                GameObject shot = Instantiate(projectile, shotPoint.position, Quaternion.identity);
                shot.transform.up = transform.up;

                transform.Rotate(0, 0, 36);
                Debug.Log(transform.eulerAngles);
                counter++;
            }
        }
    }
}
