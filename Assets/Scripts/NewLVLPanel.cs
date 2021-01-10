using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewLVLPanel : MonoBehaviour
{
    public Text lvlLabel;
         
    // Start is called before the first frame update
    void Start()
    {
        int lvl = PlayerPrefs.GetInt(Statics.HIGHTLVL, 1);
        lvlLabel.text = "lvl " + lvl;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
