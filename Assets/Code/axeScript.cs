using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class axeScript : MonoBehaviour
{
    public Transform player;
    private SpriteRenderer mySprite;

    // Start is called before the first frame update
    void Start()
    {
        mySprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        Vector3 aimPosition = new Vector3(player.position.x - inputX * 0.2f, player.position.y + 0.5f, 0);
        transform.position = Vector3.Lerp(transform.position, aimPosition, Time.deltaTime * 20);
        lerpRotate(transform, inputX * 30, 10);
        if(inputX == 1){
            mySprite.flipX = false;
        }
        else if(inputX == -1){
            mySprite.flipX = true;
        }
    }

    void lerpRotate(Transform setter, float angle, float speed){
        Vector3 originalAngle = setter.eulerAngles;
        setter.eulerAngles = new Vector3(0,0, angle);
        Quaternion to = setter.rotation;

        setter.eulerAngles = originalAngle;
        setter.rotation = Quaternion.SlerpUnclamped(setter.rotation, to, Time.deltaTime * speed);
    }
}
