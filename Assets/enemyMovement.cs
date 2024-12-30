using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask ground;
    private bool onGround;
    private float jumpCounter;
    float direction;
    public float jumpSpeed;
    public SpriteRenderer mySprite;
    public GameObject projectile;
    public Transform shotPoint;
    public Transform Player;

    // Start is called before the first frame update
    void Start()
    {
        direction = 1;
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

        if(onGround == true && jumpCounter < 0){
            jumpCounter = 2;
            //attack
            
            if(Random.Range(1, 4) != 1){
                //jump
                direction *= -1;
                rb.velocity = new Vector3(10 * direction, jumpSpeed);
                StartCoroutine(shoot());
            }
            else{
                //dash
                if(transform.position.x > Player.position.x){
                    direction = 1;
                }
                else{
                    direction = -1;
                }

                rb.velocity = new Vector2(direction * -30, 0);
            }
        }

        jumpCounter -= Time.deltaTime;

        if(direction == 1){
            mySprite.flipX = true;
        }
        else{
            mySprite.flipX = false;
        }
    }

    IEnumerator shoot(){
        yield return new WaitForSeconds(0.3f);
        Instantiate(projectile, shotPoint.position, Quaternion.identity);
    }
}
