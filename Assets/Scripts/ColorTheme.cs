using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorTheme : MonoBehaviour
{
    [HideInInspector]
    public string themeName;

    [Header("Colors")]

    public Color playerDayColor = Color.white;
    public Color playerNightColor = Color.black;

    [Space(10)]

    public Color borderDayColor = Color.white;
    public Color borderNightColor = Color.black;

    [Space(10)]

    public Color backgroundDayColor = Color.white;
    public Color backgroundNightColor = Color.black;

    [Space(10)]

    public Color accentColor = Color.cyan;

    [Space(10)]

    public bool specialTheme = false;

    [Space(10)]

    [HideInInspector]
    public Sprite backgroundImage;
    public Gradient trailGrad;


    public void FlipColors() {
        Color tmp = playerDayColor;
        playerDayColor = playerNightColor;
        playerNightColor = tmp;

        tmp = borderDayColor;
        borderDayColor = borderNightColor;
        borderNightColor = tmp;

        tmp = backgroundDayColor;
        backgroundDayColor = backgroundNightColor;
        backgroundNightColor = tmp;
    }

    // Start is called before the first frame update
    void Start()
    {
        themeName = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
