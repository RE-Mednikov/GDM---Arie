using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float shotSpeed;
    public bool aimer;
    public Transform shooterTransform;
    public GameObject hit;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(hit, transform.position, Quaternion.identity);
        rb = gameObject.GetComponent<Rigidbody2D>();
        if(aimer == false){
            transform.up = shooterTransform.up;
        }
        else{
            transform.up = new Vector2(transform.position.x - PlayerScript.playerPosition.x, transform.position.y - PlayerScript.playerPosition.y) * -1;
        }
        rb.velocity = transform.up * shotSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Instantiate(hit, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
