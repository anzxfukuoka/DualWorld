using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsItem : MonoBehaviour
{
    public PointsItemSettings pointsItemSettings;

    public Text realCurrencyText;
    public Text starsText;
    public Text gemsText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPointsSetings(PointsItemSettings pointsItemSettings) 
    {
        this.pointsItemSettings = pointsItemSettings;

        float real = pointsItemSettings.realCurrency;
        int stars = pointsItemSettings.starsPrice;
        int gems = pointsItemSettings.gemsPrice;

        realCurrencyText.text = real + "$";
        starsText.text = stars + "";
        gemsText.text = gems + "";
    }
}
