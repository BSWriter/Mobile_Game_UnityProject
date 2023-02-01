using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorData : MonoBehaviour
{
    [SerializeField] Color _darkPurple;
    [SerializeField] Color _lightPurple;

    [SerializeField] Color _saturatedGold;
    [SerializeField] Color _lightGold;
    [SerializeField] Color _gold;

    Color[] colorList = new Color[5];

    void Start(){
        colorList[0] = _darkPurple;
        colorList[1] = _lightPurple;
        colorList[2] = _saturatedGold;
        colorList[3] = _lightGold;
        colorList[4] = _gold;
    }

    public Color[] getColors(){
        return colorList;
    }
}
