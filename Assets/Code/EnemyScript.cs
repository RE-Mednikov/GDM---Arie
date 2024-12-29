using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
public class EnemyScript : MonoBehaviour
{
    public bool bodyPart;
    public GameObject killEffect;
    public int health;
    private CircleCollider2D hitBox;
    private GameObject player;
    public enemyAi myHolder;
    private SpriteRenderer mySprite;
    public Color normal;
    public Color vulnerable;
    public bool shootsProjectile;
    float projectileCounter = 1.5f;
    public GameObject projectile;
    public GameObject respawn;
    public GameObject respawnObject;
    public float moveSpeed;
    private float moveCounter;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        mySprite = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        hitBox = gameObject.GetComponent<CircleCollider2D>();

        //find respawn object
        if(respawnObject == null){
            respawnObject = gameObject;
        }

        //assigning rigidbody for movement
        if(bodyPart == false){
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        if(bodyPart == false){
            moveCounter += Time.deltaTime;

            //turn in other direction
            if(moveCounter > 2){
                moveSpeed *= -1;
                moveCounter = 0;
            }

            //apply velocity
            rb.velocity += new Vector2(0, moveSpeed * Time.deltaTime * 100);
        }

        //shoot projectiles
        projectileCounter += Time.deltaTime;

        if(shootsProjectile == true && projectileCounter > 1.5f/* && Vector3.Distance(transform.position, PlayerScript.playerPosition) < 25*/){
            GameObject shot = Instantiate(projectile, transform.position, Quaternion.identity);
            shot.GetComponent<projectileScript>().shooterTransform = transform;
            projectileCounter = 0;
        }

        if(bodyPart == true){
            //body part
            if(myHolder.vulnerability > 0){
                //only vulnerable after attack
                mySprite.color = vulnerable;
                gameObject.tag = "Enemy";
            }
            else{
                mySprite.color = normal;
                gameObject.tag = "Untagged";
            }
        }

        //rotating enemies
        //transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //if the player is attacking, kill the enemy
        if(other.gameObject.tag == "Attack"/* && hitBox.enabled == true*/){
            doHit(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Attack"/* && hitBox.enabled == true*/){
            doHit(true);
        }
    }

    void doHit(bool fromGround){
        health--;

        if(health <= 0){
            //die
            if(bodyPart == true){
                if(myHolder.vulnerability > 0){
                    myHolder.health -= 1;
                    die(fromGround);
                }
                else{
                    health++;
                }
            }
            else{
                die(fromGround);
            }
        }
        else{
            //just getting hit if the health is still greater than zero
            StartCoroutine(hit());
        }
    }

    IEnumerator hit(){
        //turn off hitbox for a bit to let the player phase through
        hitBox.enabled = false;
        CameraScript.shake(0.5f);
        Instantiate(killEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        hitBox.enabled = true;
    }

    void die(bool fromGround){
        hitBox.enabled = false;
        Vector3 originalDirection = transform.eulerAngles;
        
        GameObject effect = Instantiate(killEffect, transform.position, Quaternion.identity);
        transform.up = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y);
        effect.transform.up = transform.up;

        //crunchy effects
        CameraScript.shake(0.6f);
        Time.timeScale = 0.17f;

        //reset player variables
        if(fromGround == false){
            PlayerScript.canDash = true;
            PlayerScript.attacking = 0;
        }

        //swing axe
        //axeScript.chopCounter = 0.1f;
        axeHolder.swing = 0.2f;

        //respawn if not a body part
        if(bodyPart == true){
            //destroy limb
            Destroy(gameObject);
        }
        else{
            //start respawn
            hitBox.enabled = true;
            GameObject spawner = Instantiate(respawn, transform.position, Quaternion.identity);
            transform.eulerAngles = originalDirection;
            spawner.GetComponent<EnemyRespawn>().myRespawn = respawnObject;
        }
    }
}
