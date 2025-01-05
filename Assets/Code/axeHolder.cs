using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class axeHolder : MonoBehaviour
{
    public Transform axe;
    public static float swing;
    private Vector3 axeAim;
    public SpriteRenderer mySprite;
    private float direction;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        if(swing > 0){
            axeAim = new Vector3(0, 2);
            lerpRotate(transform, -120, 30);
        }
        else{
            lerpRotate(transform, 30, 10);
            axeAim = new Vector3(0, 0.5f);
        }

        axe.localPosition = Vector3.Lerp(axe.localPosition, axeAim, Time.deltaTime * 20);
        swing -= Time.deltaTime;

        //flipping sprite
        if(inputX == 1){
            mySprite.flipX = false;
            direction = 1;
        }
        else if(inputX == -1){
            mySprite.flipX = true;
            direction = - 1;
        }
    }

    void lerpRotate(Transform setter, float angle, float speed){
        Vector3 originalAngle = setter.eulerAngles;
        setter.eulerAngles = new Vector3(0,0, angle * direction);
        Quaternion to = setter.rotation;

        setter.eulerAngles = originalAngle;
        setter.rotation = Quaternion.SlerpUnclamped(setter.rotation, to, Time.deltaTime * speed);
    }
}
