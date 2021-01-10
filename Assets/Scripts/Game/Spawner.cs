using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEngine.Experimental.PlayerLoop;

public class Spawner : MonoBehaviour
{

    public bool spawnDistanceLabel = false;

    const int splitCount = 3;

    float blockWidth;
    public float blocHeight = 2;
    public GameObject volatileBlockPrefab;
    public GameObject starPrefab;
    public GameObject spinnerPrefab;
    public GameObject bonusPrefab;
    public GameObject DistanceLabel;


    Camera camera;
    Vector3 camPos;

    public int blockRate = 8;
    public int starRate = 1;

    [Range(0, 1)]
    public float spinnerRate = 0.92f;
    [Range(0, 1)]
    public float bonusRate = 0.9f;

    int y;
    int lastY = 0;
    int lastLvl = 1;
    int lastTextSpawned = 0;

    // сколько спавнов блоков пропустить
    // после спинера
    int skip = 0;

    void Start()
    {
        y = 0;
        lastY = 0;
        lastLvl = 1;
        lastTextSpawned = 0;

        skip = 0;

        blockWidth = (Game.borderDistance) * 2 / splitCount;
        camera = Camera.main;
    }

    void Update()
    {
        if (!Player.alive)
            return;

        camPos = camera.ViewportToWorldPoint(new Vector3(0, 1, 0)); // верхний левый угол камеры

        y = (int)camPos.y;

        SpawnBlocks();
        SpawnStars();

        if(spawnDistanceLabel)
            SpawnDistanceLabel();

        Player.nextLVL();
    }

    public void SpawnDistanceLabel() {
        if (y % 10 == 0 && y > lastTextSpawned)
        {
            GameObject label = Instantiate(DistanceLabel);
            label.transform.position = Vector3.up * camPos.y + Vector3.left * (Game.borderDistance + 0.2f);
            TextMesh text = label.GetComponent<TextMesh>();
            text.text = y + "m";
            lastTextSpawned = y;
        }
    }

    public void SpawnBlocks() {

        int lvl = Player.lvl;

        if (lastY != y) 
        {
            if (y % blockRate == 0) 
            {
                lastY = y;

                if (skip > 0)
                {
                    skip--;
                    return;
                }

                //if (Random.value < spinnerRate) 
                if(lastLvl < lvl)
                {
                    if (Player.currentBuff == BuffType.SpeedUp) {
                        Player.currentBuff = BuffType.None;
                    }
                    else
                    {
                        InitializateSpinner(new Vector3(0, y, 0));

                        skip = 2;
                        lastLvl = lvl;
                    }
                }
                else
                {
                    InitializateBlocks(new Vector3(-Game.borderDistance, y, 1));
                }
            }
        }
    }

    public void SpawnStars()
    {
        int lvl = Player.lvl;

        if (lastY != y)
        {
            if (y % starRate == 0)
            {
                int px = Random.Range(0, splitCount);

                Vector3 pos = new Vector3((px - 1) * (Game.borderDistance - Player.GetRadius()), y + 1, 1);

                if (Random.value < bonusRate && Player.currentBuff == BuffType.None && lastLvl == lvl)
                {
                    InitializateBonus(pos);
                }
                else 
                {
                    InitializateStar(pos);
                    
                }

                lastY = y;

            }
        }
    }

    public VolatileBlock[] InitializateBlocks(Vector3 pos)
    {

        VolatileBlock[] vb = new VolatileBlock[splitCount]; 

        Vector3 size = new Vector3(blockWidth, blocHeight, 1);

        int stateMain = Random.Range(0, 2); // 0, 1
        int stateSec = 1 - stateMain;

        for (int i = 0; i < splitCount; i++)
        {
            vb[i] = Instantiate(volatileBlockPrefab).GetComponent<VolatileBlock>();
            vb[i].SetSize(size);
            vb[i].SetPos(pos + Vector3.right * i * blockWidth);

            if (splitCount % (i + 1) == 0)
            {
                vb[i].SetActiveState((State)stateMain);
            }
            else {
                vb[i].SetActiveState((State)stateSec);
            }
        }

        return vb;
    }

    public void DeinitializateBlocks(VolatileBlock[] vb)
    {
        if (vb == null)
            return;

        for (int i = 0; i < vb.Length; i++)
        {
            if(vb[i] != null)
                Destroy(vb[i].gameObject);
        }
    }

    public GameObject InitializateStar(Vector3 pos) { 
        GameObject s = Instantiate(starPrefab);
        s.transform.position = pos;
        return s;
    }

    public void DeinitializateStar(GameObject s)
    {
        if (s != null)
            Destroy(s);
    }

    public GameObject InitializateBonus(Vector3 pos)
    {
        GameObject s = Instantiate(bonusPrefab);
        s.transform.position = pos;
        return s;
    }

    public void InitializateSpinner(Vector3 pos) {

        GameObject s = Instantiate(spinnerPrefab);
        s.transform.position = pos + s.GetComponent<BoxCollider2D>().size.y * 2 * Vector3.up;
    }
}
