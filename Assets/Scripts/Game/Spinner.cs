using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Spinner : MonoBehaviour
{

    public float size = 2f;
    public float radius = 2f;

    [Space(10)]
    public float spinSpeed = 0;

    [Space(10)]
    public int targetRotatesCount = 10;
    public int cost = 10;

    public float timeoutTime = 0; //sec
    public float timerSpeed = 0.002f;

    int lastRotatesCount;
    float rotatesCount = 0;

    [Space(10)]
    [Range(0, 1)]
    public float timer = 1;
    [Range(0, 1)]
    public float cl = 1;

    [Space(10)]
    public bool isSpinning = false;
    public bool passed = false;
    public bool failed = false;

    [Space(10)]
    public TextMesh label;

    [Header("Anim")]

    public float startScale;
    public SpriteRenderer star;
    public SpriteRenderer circle;

    public GameObject singPassed;
    public GameObject signFailed;

    [Header("Sound")]
    public AudioSource sound;

    BoxCollider2D collider;

    float _t = 0;

    private void OnEnable()
    {
        size = Game.borderDistance;

        collider = GetComponent<BoxCollider2D>();

        collider.size = new Vector2(1, 2) * size;
        collider.offset = new Vector2(0, -size / 2);

        label.text = "lvl " + Player.lvl;

        //startScale = star.gameObject.transform.localScale.x;
    }

    void Update()
    {
        Color sc = star.color;

        if (isSpinning) {
            timer -= timerSpeed * Mathf.Pow(2, (lastRotatesCount));
            cl -= timerSpeed * 10;

            star.color = new Color(sc.r, sc.g, sc.b, cl);

            //star.transform.localScale = startScale * Vector3.one * timer;

            //circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, cl);
        }
    }

    public Vector3 GetStartPoint(Player player, State state) 
    {
        if (state == State.Day)
        {
            return new Vector3(1, 0, 0) * (radius + player.radius);
        }
        else 
        {
            return new Vector3(1, 0, 0) * -(radius + player.radius);
        }
    }

    public void AnimateSpin(Player player, State state) {

        if (player.isShadow) {
            if (player.shadowType == ShadowType.Mirror) {
                state = 1 - state;
            }
        }

        if (state == State.Day)
        {
            player.transform.localPosition = new Vector3(Mathf.Cos(_t), Mathf.Sin(_t), 0) * (radius + player.radius);
        }
        else if (state == State.Night)
        {
            player.transform.localPosition = new Vector3(Mathf.Cos(_t + Mathf.PI), Mathf.Sin(_t), 0) * (radius + player.radius);
        }
    }

    public void Spin2(Player player, State state)
    {
        if (rotatesCount >= targetRotatesCount && !player.isShadow && !failed)
        {
            //if (Mathf.Sin(_t) >= Mathf.Sin(Mathf.PI/4)) {

            //}

            //passed

            passed = true;
            //star.color = Color.green;

            star.gameObject.SetActive(false);

            singPassed.SetActive(true);

            if (rotatesCount >= targetRotatesCount + 1) {
                sound.Stop();
                isSpinning = false;

                return;
            }
        }

        if ((timer <= 0 || cl <= 0) && !player.isShadow && !passed)
        {

            //fail

            sound.Stop();

            isSpinning = false;
            passed = true;
            failed = true;

            //star.color = Color.red;

            star.gameObject.SetActive(false);

            signFailed.SetActive(true);



            return;
        }

        isSpinning = true;

        if (!sound.isPlaying)
            sound.Play();

        _t += Time.deltaTime * spinSpeed * cl;

        if (!player.isShadow) 
        { 
            rotatesCount = _t / (Mathf.PI * 2);
            
            if(lastRotatesCount != (int)rotatesCount)
            {
                lastRotatesCount = (int)rotatesCount;
                
                //label.text = (targetRotatesCount - lastRotatesCount) + "";
                
                player.AddScore(lastRotatesCount * cost);
                
                timer = 1;
            }
        }
        
    }

    //public void Spin(Player player) {

    //    if (rotatesCount >= targetRotatesCount)
    //    {
    //        player.transform.rotation = Quaternion.Euler(0, 0, 0);

    //        isSpinning = false;
    //        cleared = true;

    //        return;
    //    }

    //    isSpinning = true;

    //    //_t += Time.deltaTime;

    //    transform.Rotate(Vector3.forward * spinSpeed * spinSpeed * Time.deltaTime);

    //    Vector3 dv = (transform.position - player.transform.position).normalized; //вектор смотрящий в центр спиннера

    //    // притягивание мячика к спиннеру
    //    float d = Vector3.Distance(player.transform.position, transform.position);

    //    if (d > radius + player.radius)
    //    {
    //        player.transform.position += dv * Time.deltaTime * spinSpeed;
    //    }


    //    //...
    //    player.transform.RotateAround(transform.position, Vector3.forward, spinSpeed * spinSpeed * Time.deltaTime);

    //    Game.instance.view.transform.Translate((player.transform.localPosition.y - player.ypos) * Vector3.up);
    //    player.transform.localPosition = new Vector3(player.transform.localPosition.x, player.ypos, player.transform.position.x);

    //    //Game.instance.view.transform.position += new Vector3(player.transform.localPosition.x, (player.transform.localPosition.y - player.ypos));

    //    //player.transform.LookAt(transform.position);
    //    //player.transform.Rotate(Vector3.up * -player.transform.rotation.eulerAngles.y);

    //    rotatesCount += (spinSpeed * spinSpeed * Time.deltaTime) / 360;
        
    //}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SendMessage("EnterSpinner", this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SendMessage("ExitSpinner");
        }
    }


}
