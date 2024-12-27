using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class henScript : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = findClosest();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 closestPoint = findClosest();
        transform.position = Vector3.Lerp(transform.position, closestPoint, Time.deltaTime * 2);
    }

    Vector3 findClosest(){
        //get points
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Hen Point");

        Vector3 closestPosition = targets[0].transform.position;
        float closest = Vector3.Distance(player.position, targets[0].transform.position);

        //find the closest one
        foreach(GameObject check in targets){
            float d = Vector3.Distance(check.transform.position, player.position);
            if(d < closest){
                closest = d;
                closestPosition = check.transform.position;
            }
        }

        //return the position of the closest enemy
        return closestPosition;
    }
}
