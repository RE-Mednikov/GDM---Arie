using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    public GameObject myRespawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(doRespawn());
    }


    IEnumerator doRespawn(){
        myRespawn.SetActive(false);
        yield return new WaitForSeconds(3.5f);
        myRespawn.SetActive(true);
        Destroy(gameObject);
    }
}
