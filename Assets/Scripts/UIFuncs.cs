using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFuncs : MonoBehaviour
{
    //public Text modeLabel;
    //public Text modeLabel2;

    //public Text starsPointsLabel;
    //public Text gemsPointsLabel;

    [Space(10)]

    public Button soundButton;

    public Sprite soundOn;
    public Sprite soundOff;

    // Start is called before the first frame update
    void Start()
    {
        SoundButtonImageSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void UpdateUIColors() {

    //    modeLabel.color = Game.GetColorTheme().playerDayColor; //.backgroundDayColor;
    //    modeLabel2.color = Game.GetColorTheme().playerDayColor;

    //    starsPointsLabel.color = Game.GetColorTheme().backgroundDayColor;
    //    gemsPointsLabel.color = Game.GetColorTheme().backgroundDayColor;

    //    //storeButtonImage.color = c;
    //    //soundButtonImage.color = c;

    //    //showAdButtonImage.color = c;
    //    //buyMyShitPLZButtonImage.color = c;

    //}

    //на кнопке
    public void SoundButtonImageSwitch() {
        Debug.Log("mute sound: " + SoundMaster.mute);

        if (!SoundMaster.mute)
        {
            soundButton.image.sprite = soundOn;
        }
        else 
        {
            soundButton.image.sprite = soundOff;
        }
    }

    public void WatchAd() {
        Debug.Log(":з");
    }
}
