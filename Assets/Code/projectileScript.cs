using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float shotSpeed;
    public bool aimer;
    public float shooterAngle;
    public GameObject hit;
    public GameObject deflect;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(hit, transform.position, Quaternion.identity);
        rb = gameObject.GetComponent<Rigidbody2D>();
        if(aimer == false){
            transform.eulerAngles = new Vector3(0, 0, shooterAngle);
        }
        else{
            transform.up = new Vector2(transform.position.x - PlayerScript.playerPosition.x, transform.position.y - PlayerScript.playerPosition.y) * -1;
        }
        rb.velocity = transform.up * shotSpeed;
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
        Instantiate(hit, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
