using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{

    [SerializeField] private Material starColor;
    [SerializeField] private GameObject collaspingStar;

    List<GameObject> _currentCollectibles;
    List<GameObject> _nextCollectibles; 

    List<GameObject> _collectibles;

    void Awake(){
        _currentCollectibles = new List<GameObject>();
        _nextCollectibles = new List<GameObject>();
        _collectibles = new List<GameObject>();
        starColor.color = new Color(1, 1, 0.3f, 1);
    }

    void Start()
    {
        // Have the color of the material change with the pulse
        LeanTween.color(gameObject, new Color(1, 0.7f, 0.3f, 1), 1f).setLoopPingPong();

        // Remove later
        // CreateCollectibles("Normal");
        // Vector3[] path = CreateVectorPath(6, 135, 3, 6);
        // foreach (Vector3 v in path){
        //     print(v);
        // }
    }

    

    public List<GameObject> CreateCollectibles(string lv){
        List<GameObject> collectibles = new List<GameObject>();
        if(lv.Equals("Normal")){
            for(int i = 1; i<=10; i++){
                GameObject star = Instantiate(collaspingStar, new Vector3(0, -9, 15 * i), Quaternion.Euler(0, 0, 0)) as GameObject;
                star.transform.parent = this.transform;
                
                // Have each individual star pulsate
                LeanTween.scale(star,new Vector3(0.65f, 0.65f, 0.65f), 1).setEaseInOutBounce().setLoopPingPong();

                // Add the newly created star to the collectible list
                collectibles.Add(star);
            }
        }
        else if(lv.Equals("RightPit")){
            Vector3[] path = CreateVectorPath(6, 135, 3, 6);
            foreach(Vector3 v in path){
                GameObject star = Instantiate(collaspingStar, v, Quaternion.identity, this.transform) as GameObject;

                // Have each individual star pulsate
                LeanTween.scale(star,new Vector3(0.65f, 0.65f, 0.65f), 1).setEaseInOutBounce().setLoopPingPong();

                // Add the newly created star to the collectible list
                collectibles.Add(star);
            }
        }

        return collectibles;
    }


    // print(6 * Mathf.Sin(45 * Mathf.PI/180));
    Vector3[] CreateVectorPath(int num, int angle, int section, int distance){
        Vector3[] spawnPath = new Vector3[num];
        
        float x = distance * Mathf.Sin(angle * Mathf.PI/180);
        float y = distance * Mathf.Cos(angle * Mathf.PI/180);
        float z = -30 + (60*(section-1)) + 5;
        float Xincre = (x * 2) / (num-1);
        // float Yincre 

        for(int i = 0; i<num; i++){
            // 300 is for next level offset
            Vector3 pos = new Vector3(x - Xincre*i, -0.3469f*(Mathf.Pow(x - Xincre*i, 2)) + 2, (z + 10*i)+300);
            spawnPath[i] = pos;
        }

        return spawnPath;
    }

    void Update()
    {
        // print(starColor.color.g);
        // while(_climax){
        //     if(starColor.color.g <= 0.7f){
        //         _climax = false;
        //     }
        //     else{
        //         starColor.color = new Color(1, starColor.color.g, 0.3f, 1);
        //     }
        // }
        // while(!_climax){
        //     if(starColor.color.g >= 0.99f){
        //         _climax = true;
        //     }
        //     else{
        //         starColor.color = new Color(1, starColor.color.g, 0.3f, 1);
        //     }
        // }
    }

    //     IEnumerator colorShift(){
    //     while(_climax){
    //         if(starColor.color.g <= 176){
    //             _climax = false;
    //         }
    //         else{
    //             starColor.color = new Color(255, starColor.color.g-1, 76, starColor.color.a);
    //         }
    //         yield return new WaitForEndOfFrame();
    //     }
    //     while(!_climax){
    //         if(starColor.color.g >= 255){
    //             _climax = true;
    //         }
    //         else{
    //             starColor.color = new Color(255, starColor.color.g+1, 76, starColor.color.a);
    //         }
    //         yield return new WaitForEndOfFrame();
    //     }
    //     yield return null;
    // }
}
