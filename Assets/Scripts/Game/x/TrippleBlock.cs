using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrippleBlock : MonoBehaviour
{
    public VolatileBlock blockPrefab;

    public float blocHeight = 1;
    float blockWidth;

    // Start is called before the first frame update
    void Start()
    {
        blockWidth = (Game.borderDistance) * 2 / 3;

        Vector3 size = new Vector3(blockWidth, blocHeight, 1);

        int stateMain = Random.Range(0, 2); // 0, 1
        int stateSec = 1 - stateMain;

        VolatileBlock[] vb = new VolatileBlock[3];

        //Debug.Log("nani");

        for (int i = 0; i < vb.Length; i++)
        {
            vb[i] = Instantiate(blockPrefab, transform).GetComponent<VolatileBlock>();
            vb[i].transform.localScale = size;
            vb[i].transform.position = transform.position + Vector3.right * (i * blockWidth) + Vector3.right * blockWidth/2 - Vector3.right * Game.borderDistance;

            if (3 % (i + 1) == 0)
                vb[i].SetActiveState((State)stateMain);
            else
                vb[i].SetActiveState((State)stateSec);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
