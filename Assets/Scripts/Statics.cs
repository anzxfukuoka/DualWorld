using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statics : MonoBehaviour
{
    /*
    // PlayerPrefs
    */

    //самостоятельные ключи
    public static string HIGHTSCORE = "HIGHTSCORE"; // int
    public static string LASTSTATE = "LASTSTATE"; // int

    public static string HIGHTLVL = "HIGHTLVL"; // int
    public static string MAXDISTANCE = "MAXDISTANCE"; // int
    
    public static string STARSPOINTS = "STARSPOINTS"; // int бессплатная валюта
    public static string GEMSPOINTS = "GEMSPOINTS"; // int платная (редкая) валюта

    public static string CURRENTTHEME = "CURRENTTHEME"; // str название темы (имя обьекта на сцене)

    public static string UNLOCKALL = "UNLOCKALL"; // открытие всех тем (не покупка всех тем) // bool

    public static string SOUND = "SOUND"; // bool off/on

    public static string NOADS = "NOADS"; //bool

    //преставки
    public static string THEME = "THEME_";
    public static string ITEM = "ITEM_";

}
