using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float shotSpeed;
    public bool aimer;
    public GameObject hit;
    public GameObject deflect;
    private float startCounter;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(hit, transform.position, Quaternion.identity);
        rb = gameObject.GetComponent<Rigidbody2D>();
        if(aimer == true){
            transform.up = new Vector2(transform.position.x - PlayerScript.playerPosition.x, transform.position.y - PlayerScript.playerPosition.y) * -1;
        }
        rb.velocity = transform.up * shotSpeed;
    }

    void Update(){
        startCounter += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Attack"){
            //parry shot
            Instantiate(deflect, transform.position, Quaternion.identity);
            Time.timeScale = 0.1f;
            CameraScript.shake(1);
            rb.velocity *= -3;
            gameObject.tag = "Attack";
            gameObject.layer = 0;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other) {
        if(startCounter > 0.1f){
            Instantiate(hit, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
