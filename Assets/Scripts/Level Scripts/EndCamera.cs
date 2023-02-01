using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCamera : MonoBehaviour
{
    
    void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
            
        }
    }

    void OnTriggerExit(Collider other){
        print("Exit");
    }

}
