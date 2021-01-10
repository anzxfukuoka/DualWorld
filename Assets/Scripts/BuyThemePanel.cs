using IAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyThemePanel : MonoBehaviour
{
    public GameObject value;
    public GameObject specialValue;

    public Text stars;
    public Text gems;
    public Text realCurrency;

    private void OnEnable()
    {
        StoreItemSettings selectedItem = Store.selectedItem;

        if (selectedItem.special)
        {
            value.SetActive(false);
            specialValue.SetActive(true);

            //realCurrency.text = selectedItem.starsPrice + "";
        }
        else
        {
            value.SetActive(true);
            specialValue.SetActive(false);

            stars.text = selectedItem.starsPrice + "";
            gems.text = selectedItem.gemsPrice + "";
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnOKClick() {
        Store.BuySelectedTheme();
    }
}
