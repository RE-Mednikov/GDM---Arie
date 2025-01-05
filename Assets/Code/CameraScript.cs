using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    static float myShake;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position + new Vector3(-(float)PlayerScript.wallSide / 4, 0), new Vector3(player.position.x, player.position.y + 2, -10), Time.deltaTime * 5);
        transform.position += new Vector3(Random.Range(-myShake, myShake), Random.Range(-myShake, myShake));
        myShake *= 0.9f;

        Time.timeScale = Mathf.Lerp(Time.timeScale, 1, Time.deltaTime * 300);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public static void shake(float amount){
        if(amount > myShake){
            myShake = amount;
        }
    }
}
