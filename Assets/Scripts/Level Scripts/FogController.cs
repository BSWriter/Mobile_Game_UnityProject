using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogController : MonoBehaviour
{
    public GameObject fogUI;
    [SerializeField] GameObject chaser;

    void Start()
    {
        StartCoroutine(fogUIUpdate());
    }

    void Update()
    {
        
    }

    // Move to LevelManager script
    IEnumerator fogUIUpdate(){
        while(true){
            // print(-150 - (chaser.transform.position.z - gameObject.transform.position.z));
            fogUI.transform.position = new Vector3(540, -150 - (chaser.transform.position.z - gameObject.transform.position.z), 0);
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Move to SphereCollision script
    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player")){
            // transform.GetComponentInParent<LevelManager>().ResetLevel();
            transform.GetComponentInParent<LevelManager>().GameOver();
        }
    }
}
