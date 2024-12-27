using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class enemyAi : MonoBehaviour
{
    public int health;
    public Rigidbody2D rb;
    private float attackCounter;
    public float jumpSpeed;
    int direction;
    private Transform Player;
    public GameObject groundSlam;
    public GameObject shoulderCheck;
    float fallCounter;
    public GameObject body;
    public float moveSpeed;
    float moveCounter;
    int moveDirection;
    public GameObject death;
    public float vulnerability;

    // Start is called before the first frame update
    void Start()
    {
        moveDirection = 1;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, Player.position) < 40){
            attack();
            move();
        }

        moveCounter -= Time.deltaTime;

        if(health <= 0){
            Time.timeScale = 0.05f;
            Instantiate(death, transform.position, Quaternion.identity);
            CameraScript.shake(1);
            Destroy(gameObject);
        }

        vulnerability -= Time.deltaTime;
    }

    void move(){
        if(moveCounter < 0){
            rb.velocity += new Vector2(moveDirection * moveSpeed * Time.deltaTime, 0);
        }
    }

    void attack(){

        attackCounter += Time.deltaTime;

        if(attackCounter > 2.5f){
            attackCounter = Random.Range(-2f, 0);
            moveCounter = 2;
            moveDirection *= -1;


            if(Random.Range(1, 3) == 1){
                StartCoroutine(jumpAttack());
            }
            else{
                //StartCoroutine(fallAttack());
                fallCounter = 2;
                if(transform.position.x < Player.position.x){
                    direction = 1;
                }
                else{
                    direction = -1;
                }
            }
        }

        fallCounter -= Time.deltaTime;

        //fall attack
        if(fallCounter > 1.5f){
            lerpRotate(transform, direction * 10, 3);
        }
        else if(fallCounter > 1){
            lerpRotate(transform, -direction * 20, 12);
            rb.velocity = new Vector2(20 * direction, rb.velocity.y);
            shoulderCheck.SetActive(true);
            CameraScript.shake(0.2f);
        }
        else if(fallCounter > 0.5f){
            vulnerability = 0.5f;
            shoulderCheck.SetActive(false);
            lerpRotate(transform, 0, 12);
            rb.velocity *= new Vector2(1 - Time.deltaTime * 5, 1);
        }
    }

    IEnumerator jumpAttack(){
        //windup
        rb.gravityScale = 2;
        rb.velocity += new Vector2(0, jumpSpeed);

        yield return new WaitForSeconds(0.4f);

        rb.gravityScale = 20;

        while(rb.gravityScale == 20){
            yield return null;
        }
        vulnerability = 1;
        groundSlam.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        groundSlam.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //smash when landing from jump
        if(rb.gravityScale == 20){
            CameraScript.shake(0.5f);
            rb.gravityScale = 19;
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
}
