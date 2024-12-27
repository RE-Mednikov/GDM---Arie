using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;
public class EnemyScript : MonoBehaviour
{
    public bool cantMove;
    public GameObject killEffect;
    public bool rotater;
    private float moveCounter;
    private Rigidbody2D rb;
    public int health;
    private CircleCollider2D hitBox;
    private GameObject player;
    public enemyAi myHolder;
    private SpriteRenderer mySprite;
    public Color normal;
    public Color vulnerable;
    public bool shootsProjectile;
    float projectileCounter;
    public GameObject projectile;
    public GameObject respawn;

    // Start is called before the first frame update
    void Start()
    {
        mySprite = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        hitBox = gameObject.GetComponent<CircleCollider2D>();

        if(cantMove == false){
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //shoot projectiles
        projectileCounter += Time.deltaTime;

        if(shootsProjectile == true && projectileCounter > 1.5f && Vector3.Distance(transform.position, PlayerScript.playerPosition) < 25){
            Instantiate(projectile, transform.position, Quaternion.identity);
            projectileCounter = 0;
        }

        if(cantMove == true){
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
        if(rotater == true){
            transform.Rotate(0, 0, 100 * Time.deltaTime);
        }

        //movement
        if(cantMove == false){
            moveCounter -= Time.deltaTime;
        }

        if(Vector3.Distance(transform.position, player.transform.position) < hitBox.radius * 3){
            //if the player is attacking, kill the enemy
            if(player.tag == "Attack" && hitBox.enabled == true){
                health--;
                if(health <= 0){
                    //die
                    if(cantMove == true){
                        if(myHolder.vulnerability > 0){
                            myHolder.health -= 1;
                            die();
                        }
                        else{
                            health++;
                        }
                    }
                    else{
                        die();
                    }
                }
                else{
                    //just getting hit if the health is still greater than zero
                    StartCoroutine(hit());
                }
            }
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

    void die(){
        hitBox.enabled = false;
        GameObject effect = Instantiate(killEffect, transform.position, Quaternion.identity);
        transform.up = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y);
        effect.transform.up = transform.up;
        //effect.GetComponent<ParticleSystem>().startRotation = transform.eulerAngles.z;
        CameraScript.shake(0.5f);
        Time.timeScale = 0.25f;
        //maybe respawn??
        PlayerScript.canDash = true;
        PlayerScript.attacking = -0.2f;

        if(cantMove == true){
            Destroy(gameObject);
        }
        else{
            hitBox.enabled = true;
            GameObject spawner = Instantiate(respawn, transform.position, Quaternion.identity);
            spawner.GetComponent<EnemyRespawn>().myRespawn = gameObject;
        }
    }
}
