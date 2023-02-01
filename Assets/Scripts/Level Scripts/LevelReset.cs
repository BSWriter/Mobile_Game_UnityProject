using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class LevelReset : MonoBehaviour
{
    Transform _player;
    Transform _cam; 
    CinemachineFreeLook _groundCine;
    CinemachineFreeLook _airCine;


    public GameObject thresholdText;



    void Start()
    {
        _cam = Camera.main.transform;
        _player = transform.GetComponentInParent<LevelManager>().getPlayer();
        _groundCine = transform.GetComponentInParent<LevelManager>().getGroundCine();
        _airCine = transform.GetComponentInParent<LevelManager>().getAirCine();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // Rigidbody otherRb = other.GetComponent<Rigidbody>();
        // float colVel = otherRb.velocity.z;
        
        // if(colVel > _speedThreshold){
        //     _speedThreshold += 5f;
        //     transform.GetComponentInParent<LevelManager>().NextLevel();    
        //     thresholdText.GetComponent<TextMeshProUGUI>().text = "" + _speedThreshold;
        // }
        // else{
        //     otherRb.velocity = new Vector3(otherRb.velocity.x, otherRb.velocity.y, -(otherRb.velocity.z/2));

        //     other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, 269.2f);
        // }
        
        
    }

}
