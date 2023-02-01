using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentController : MonoBehaviour
{
        Transform _player;
        Rigidbody _playerRb;
        Transform _thisObj;
        Transform _cam;
        bool _movingBack;
        float _deltaPos;
        public LayerMask mask;

        Quaternion _actualRotation;




    void Start(){
        _player = transform.parent.Find("Body");
        _playerRb = transform.parent.Find("Body").GetComponent<Rigidbody>();
        _cam = Camera.main.transform;
        _thisObj = this.transform;
        _actualRotation = transform.rotation;

        _movingBack = false;
    }



    void Update()
    {
        float yRotation = _cam.eulerAngles.y;

        // if(_movingBack){
        //     yRotation -= 180;
        // }

        transform.eulerAngles = new Vector3( transform.eulerAngles.x, yRotation, transform.eulerAngles.z );
        // Constantly have the GameObject make the angle of the terrain the player is rolling on.
        RaycastHit hit;
        if (Physics.Raycast(_player.position, -_thisObj.up, out hit, 1f))
        {
            var slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);
            // transform.Rotate(0,0,90,Space.World);
            
        }
        _actualRotation = transform.rotation;
        // Rotate transform by 90 on global z axis
        // Continues to rotate infinitly 
        // Compare prev player position with current to see if rotation should occur
        


        // Maybe do this instead
        // var lookPos = target.position - transform.position;
        // lookPos.y = 0;
        // var rotation = Quaternion.LookRotation(lookPos);
        // rotation *= Quaternion.Euler(0, 90, 0); // this adds a 90 degrees Y rotation
        // var adjustRotation = transform.rotation.y + rotationAdjust; //<- this is wrong!
        // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);


        // transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z * Mathf.Sign(_player.position.x)); 

        


        // float yRotation = _cam.eulerAngles.y;
        // Vector3 rot = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);

        // if(-8f <= _player.position.x && _player.position.x <= 8f){
        //     transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, _player.position.x * 5.63f);
        // }
        // else{
        //     transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, 80 * Mathf.Sign(_player.position.x));
        // }

        // print(transform.position.x + " " + _player.position.x);
        // _deltaPos = _deltaPos - _player.position.x;
        // if(transform.position.x > _player.position.x){
        //     transform.RotateAround(new Vector3(0, 8, 0), Vector3.forward, _deltaPos);
        // }
        // else if(transform.position.x < _player.position.x){
        //     transform.RotateAround(new Vector3(0, 8, 0), Vector3.forward, _deltaPos);
        // }


        

        // transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, _player.position.x * 5.63f);
        // transform.up = new Vector3();
        // transform.eulerAngles = new Vector3( transform.eulerAngles.x, yRotation, transform.eulerAngles.z );
    }

    void PushBody(Vector2 dir){
        // Turn dir to Vector3 along alignment rotation
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
        // moveDir.y = transform.up;

        // _playerRb.AddForce();
    }

    public void SwapDirection(){
        _movingBack = !_movingBack;
    }
}
