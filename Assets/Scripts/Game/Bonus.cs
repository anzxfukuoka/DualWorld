using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public bool random = false;

    //readonly
    public BuffType buffType;

    float buffTime = 10;

    public float[] buffTimes;

    public float lifeTime = 30f;
    float t = 0;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if (random) {
            int type = Random.Range(1, 5);
            buffType = (BuffType)type;
        }

        buffTime = buffTimes[(int)buffType];

        animator.SetInteger("type", (int)buffType);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if (t > lifeTime)
            Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            if (Player.currentBuff != BuffType.None)
                return;

            Debug.Log("~☺~");

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            for (var i = 0; i < players.Length; i++)
            {
                players[i].GetComponent<Player>().AddBuff(buffType, buffTime);
            }

            Destroy(gameObject);
        }
            
        if (collision.gameObject.tag == "Spinner")
        {
            Destroy(gameObject);
        }
    }
}

public enum BuffType
{
    None,
    SpeedUp, // 
    MirrorShadow, //
    DoubleShadow, //
    BigBall, // увелечение шарика
    SmallBall, // уменьшени шарика
}