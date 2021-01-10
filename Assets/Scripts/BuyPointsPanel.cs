using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyPointsPanel : MonoBehaviour
{
    public Transform content;
    public GameObject pointsItemPrefab;

    void Start()
    {
        PointsItemSettings[] items = Store.GetPointsItens();

        for (int i = 0; i < items.Length; i++) {
            GameObject p = Instantiate(pointsItemPrefab, content);
            //p.transform.SetParent(transform);
            PointsItem pointsItem = p.GetComponent<PointsItem>();
            pointsItem.SetPointsSetings(items[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
