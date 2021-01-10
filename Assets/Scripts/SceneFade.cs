using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFade : MonoBehaviour
{

    public static bool showed = false;

    private void OnEnable()
    {
        if (showed)
            gameObject.SetActive(false);
    }

    public void OnAnimEnd() 
    {
        gameObject.SetActive(false);
        showed = true;
    }
}
