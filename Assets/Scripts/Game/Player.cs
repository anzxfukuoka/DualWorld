using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public static Player instance;

    public static int score = 0;
    public static bool alive = true;

    public static int lvl = 1;
    public const int maxLVL = 10;

    public bool godMode = false;

    public bool isShadow = false;

    [Header("Shadow Properties")]

    public ShadowType shadowType;

    [Header("Player Properties")]

    public float ballSpeed = 1;

    public float jumpTime = 1f;

    public int scoreModifier = 1;

    [Range(0, 1)]
    public float radius = 1f;

    public Player[] shadows;

    ParticleSystem boom;
    TrailRenderer trail;
    [HideInInspector]
    public Animator animator;
    SpriteRenderer spriteRenderer;
    CircleCollider2D col;

    public ParticleSystem speedUpEffect;
    public SpriteRenderer speedUpPanel;

    public UnityEvent onKill;

    bool grounded = true;

    Spinner spinner = null;

    float ypos = 0; //локальная позиция относительно view

    public static BuffType currentBuff = BuffType.None;
    public static BuffType lastBuff = BuffType.None;

    public static bool magnet = false;

    // для анимции мячика
    Vector3 startPos;
    Vector3 endPos;

    bool enableJump = true;


    //вызывается на блоке
    public void Kill() {

        if (currentBuff == BuffType.DoubleShadow) {

            //бум
            if (!boom.isPlaying)
                boom.Play();

            if (isShadow)
            {
                animator.SetTrigger("kill");
                animator.ResetTrigger("showOn");
                animator.ResetTrigger("showOff");
            } 

            return;
        }

        if (alive) {
            UpdateBoomColor();

            animator.SetTrigger("kill");

            //бум
            if (!boom.isPlaying)
                boom.Play();

            if (!isShadow)
                onKill.Invoke();

        }

    }

    public void OnKillAnimEnded() {
        if (isShadow) {
            animator.ResetTrigger("showOn");
            animator.ResetTrigger("showOff");
            animator.Play("alive", -1, 0f);
            gameObject.SetActive(false);
        }
    }

    //вызывается на звездочке
    public void AddScore(int c) {
        score += c * scoreModifier;
    }

    //вызывается на бонусе
    public void AddBuff(BuffType buff, float time) {

        lastBuff = currentBuff;
        currentBuff = buff;
        StartCoroutine(Buff(buff, time));
    }

    public static float metersForLVL(int l) {
        if (l <= 0)
            return 0;

        return Mathf.Pow(2, l) * 100;
    }

    public static float metersForNextLVL() {
        return metersForLVL(lvl);
    }

    public static float metersToNextLVL() {
        return metersForNextLVL() - (GetM() - metersForLVL(lvl - 1));
    }

    public static void nextLVL() {

        //Debug.Log(metersToNextLVL() + " " + GetM() + " " + metersForNextLVL());

        if (Game.paused)
            return;

        if (GetM() - metersForLVL(lvl - 1) >= metersForNextLVL() + 10) {
            lvl++;
            Debug.Log("m:" + (GetM()) + " l:" + lvl);
        }
    }

    public static float GetM() {
        if(Game.paused)
            return PlayerPrefs.GetInt(Statics.MAXDISTANCE, 0);

        return instance.transform.position.y - instance.ypos;
    }

    IEnumerator Buff(BuffType buff, float time) {
        float timer = 0;

        animator.ResetTrigger("exitBuff");

        switch (buff)
        {
            case BuffType.SmallBall:
                animator.SetTrigger("small");
                break;

            case BuffType.BigBall:
                animator.SetTrigger("big");
                break;

            case BuffType.MirrorShadow:
                if (!isShadow)
                {
                    shadows[0].gameObject.SetActive(true);
                    shadows[0].animator.SetTrigger("showOn");
                    scoreModifier = 2;
                }
                break;

            case BuffType.DoubleShadow:
                if (!isShadow)
                {
                    shadows[1].gameObject.SetActive(true);
                    shadows[1].animator.SetTrigger("showOn");
                    scoreModifier = 2;
                }
                break;

            case BuffType.SpeedUp:
                godMode = true;
                Debug.Log("(☆ Д ☆)!");

                scoreModifier = 4;

                magnet = true;

                speedUpEffect.Play();

                float startScrollSpeed = Game.instance.scrollSpeed;
                float startBallSpeed = ballSpeed;

                float endScrollSpeed = startScrollSpeed * 2f;
                float endBallSpeed = startBallSpeed * 2f;


                for (float t = 0; t < 1f; t += Time.deltaTime) {

                    Game.instance.scrollSpeed = Mathf.Lerp(startScrollSpeed, endScrollSpeed, Easing.Exponential.InOut(t));
                    //ballSpeed = Mathf.Lerp(endScrollSpeed, endBallSpeed, Easing.Exponential.InOut(t));
                    //Debug.Log(t);
                    speedUpPanel.color = Color.Lerp(Color.clear, new Color(1, 1, 1, 0.6f), Easing.Exponential.InOut(t));

                    yield return null;
                }

                break;
        }

        yield return null;

        while (timer < time && currentBuff == buff)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        switch (buff)
        {
            case BuffType.SmallBall:
                animator.ResetTrigger("small");
                break;

            case BuffType.BigBall:
                animator.ResetTrigger("big");
                break;

            case BuffType.MirrorShadow:
                if (!isShadow) {
                    shadows[0].animator.SetTrigger("showOff");
                    scoreModifier = 1;
                }
                break;

            case BuffType.DoubleShadow:
                if (!isShadow)
                {
                    shadows[1].animator.SetTrigger("showOff");
                    scoreModifier = 1;
                }
                break;

            case BuffType.SpeedUp:

                speedUpEffect.Stop();

                float startScrollSpeed = Game.instance.scrollSpeed;
                float startBallSpeed = ballSpeed;

                float endScrollSpeed = startScrollSpeed / 2f;
                float endBallSpeed = startBallSpeed / 1.5f;

                for (float t = 0; t < 1; t += Time.deltaTime)
                {
                    Game.instance.scrollSpeed = Mathf.Lerp(startScrollSpeed, endScrollSpeed, Easing.Exponential.InOut(t));
                    //ballSpeed = Mathf.Lerp(endScrollSpeed, endBallSpeed, Easing.Exponential.InOut(t));
                    speedUpPanel.color = Color.Lerp(new Color(1, 1, 1, 0.6f), Color.clear, Easing.Exponential.InOut(t));

                    yield return null;
                }

                magnet = false;

                godMode = false;
                Debug.Log("( - _ -)...");

                scoreModifier = 1;

                break;
        }

        lastBuff = currentBuff;
        currentBuff = BuffType.None;

        animator.SetTrigger("exitBuff");

        yield return null;
    }

    public void ShowAnimation(int aTime) {

        int animLayer = 0;

        //animator.GetCurrentAnimatorStateInfo(animLayer).IsTag("show");

        float time = 130f / 60f; //animator.GetCurrentAnimatorStateInfo(animLayer).clip length
        float speed = animator.GetCurrentAnimatorStateInfo(animLayer).speed;

        //Debug.Log(aTime + " " + speed);

        if (aTime == 0)
        {
            if (speed < 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(TrailAnim(time, aTime));
            }
        }

        if (aTime == 1)
        {
            if (speed < 0)
            {
                col.enabled = false;
                StartCoroutine(TrailAnim(time, aTime));
            }
            else
            {
                col.enabled = true;
            }
        }
    }

    public IEnumerator TrailAnim(float time, int aTime) {

        Gradient gradient;
        GradientAlphaKey[] keys;

        float a = spriteRenderer.color.a;

        for (float t = 0; t < time; t += Time.deltaTime) {

            a = spriteRenderer.color.a;

            gradient = GetGrad();

            keys = gradient.alphaKeys;
            keys[0].alpha = a;
            gradient.SetKeys(gradient.colorKeys, keys);

            trail.colorGradient = gradient;

            yield return null;
        }

        gradient = GetGrad();

        keys = gradient.alphaKeys;
        keys[0].alpha = 1f;
        gradient.SetKeys(gradient.colorKeys, keys);

        trail.colorGradient = gradient;

    }

    public void EnterSpinner(Spinner s) {
        spinner = s;
    }

    public void ExitSpinner()
    {
        spinner = null;
    }

    void Start()
    {
        if (!isShadow)
            instance = this;

        alive = true;

        currentBuff = BuffType.None;

        boom = GetComponentInChildren<ParticleSystem>();
        trail = GetComponentInChildren<TrailRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();

        if (isShadow)
            animator.SetBool("isShadow", true);

        if (!isShadow)
            InitPos();

        //Debug.Log(ypos);

        UpdateTrailColor();

        if (isShadow)
            gameObject.SetActive(false);
    }

    public static void Continue()
    {
        alive = true;
        currentBuff = BuffType.None;
        instance.enableJump = true;

        for (int i = 0; i < instance.shadows.Length; i++) 
        {
            instance.shadows[i].enableJump = true;
        }

        instance.animator.Play("alive", -1, 0f);
        instance.animator.SetTrigger("showOn");
    }

    private void OnEnable()
    {
        InitPos();
        UpdateTrailColor();

        animator = GetComponent<Animator>();
        if (isShadow)
            animator.SetBool("isShadow", true);
    }

    public void InitPos() {
        ypos = transform.localPosition.y;

        transform.localPosition = new Vector3(-Game.borderDistance + radius, ypos, 0);

        //Debug.Log(ypos);

        if (isShadow)
        {
            if (shadowType == ShadowType.Mirror)
            {
                if (Game.gameState == State.Day)
                {
                    transform.localPosition = new Vector3(-Game.borderDistance + radius, ypos, 0);
                }
                else
                {
                    transform.localPosition = new Vector3(Game.borderDistance - radius, ypos, 0);
                }
            }
            else
            {
                if (Game.gameState == State.Day)
                {
                    transform.localPosition = new Vector3(Game.borderDistance - radius, ypos, 0);
                }
                else
                {
                    transform.localPosition = new Vector3(-Game.borderDistance + radius, ypos, 0);
                }
            }
        }
    }

    void Update()
    {
        //обновление позиции, во время изменения радиуса
        UpdatePos();

        if (isShadow)
            return;

        //управление
        if (!Game.IsUIActive() //not UI
            && !Game.paused)  
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (spinner != null) {

                    if (!spinner.passed && !godMode) {
                        if (grounded)
                        {
                            StartCoroutine(Spin(Game.gameState));
                        }
                        else
                        {
                            if (!isShadow)
                            {
                                spinner.spinSpeed += 1;
                                spinner.cl = 1;
                            }
                        }
                        return;
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (grounded)
                {
                    StartCoroutine(Jump(Game.gameState));
                }
            }

        }
    }

    void UpdatePos() {

        if (!grounded)
            return;

        int animLayer = 0;
        string tag = "radiusChanging";

        //проигрывается ли анимация, меняющая радиус мячика (c тегом radiusChanging)
        if (animator.GetCurrentAnimatorStateInfo(animLayer).IsTag(tag) 
            && animator.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f) 
        {
            State state = Game.gameState;

            if (isShadow && shadowType == ShadowType.Mirror)
                state = state - 1;

            switch (state) {
                case State.Day:
                    transform.localPosition = new Vector3(+Game.borderDistance - radius, ypos, 0);
                    break;
                case State.Night:
                    transform.localPosition = new Vector3(-Game.borderDistance + radius, ypos, 0);
                    break;
            }
        }

    }

    

    public void AnimateJump(State state, float t) // t c [0, 1]
    {
        //Debug.Log(enableJump);

        if (isShadow) {
            if (shadowType == ShadowType.Mirror) {
                state = 1 - state; //обратное состояние
            }
        }

        if (t == 0)
        {
            grounded = false;

            if (!enableJump)
                return;

            startPos = Vector3.up * ypos;
            endPos = Vector3.up * ypos;

            if (state == State.Day)
            {
                startPos += (Game.borderDistance - radius) * Vector3.right;
                endPos += (-Game.borderDistance + radius) * Vector3.right;
            }
            else if (state == State.Night)
            {
                startPos += (-Game.borderDistance + radius) * Vector3.right;
                endPos += (Game.borderDistance - radius) * Vector3.right;
            }

            //Debug.Log(name + " " + startPos.x + " " + endPos.x + " " + state);
        }
        else {

            if (enableJump)
                transform.localPosition = Vector3.Lerp(startPos, endPos, Easing.Exponential.InOut(t));

            if (t == 1) {

                grounded = true;
                
                if (!alive) {
                    enableJump = false;
                }
            }
                
        }
        
    }

    IEnumerator ShadowJump(State state) {

        if(shadowType == ShadowType.Snake)
            yield return new WaitForSeconds(0.1f);

        AnimateJump(state, 0);

        for (float t = 0; t < 0.9f; t += Time.deltaTime * Player.instance.ballSpeed)
        {
            AnimateJump(state, t);

            yield return new WaitForEndOfFrame();
        }

        AnimateJump(state, 1);

    }

    IEnumerator Jump(State state)
    {

        Game.FlipState();

        Game.SetColorTransition(0);

        //тени
        for (int i = 0; i < shadows.Length; i++) 
        {
            if (shadows[i].gameObject.activeSelf) {
                shadows[i].StartCoroutine(shadows[i].ShadowJump(state));
            }
        }

        AnimateJump(state, 0); // в начальную позицию

        // здесь верхняя граница t = 0.9f, а не 1 потому,
        // что экспоненциальная функция, использованная для описания
        // движения мячика, при значенияx t >= 0.9 выдает числа, очень близкие к 1, 
        // из-за чего визуально мячик уже вроде бы уже приземлился, 
        // но нажатия еще не реагирует, ведь фактически анимация еще не закончилась

        for (float t = 0; t < 0.9f; t += Time.deltaTime * Player.instance.ballSpeed) {

            AnimateJump(state, t);

            Game.SetColorTransition(t);

            yield return null;
        }

        Game.SetColorTransition(1);

        AnimateJump(state, 1); // в конечную позицию

        yield return null;
    }

    public void AnimateToSpinner(State state, Spinner spinner, float t) {

        if (isShadow)
        {
            if (shadowType == ShadowType.Mirror)
            {
                state = 1 - state;
            }
        }

        if (t == 0)
        {
            grounded = false;

            startPos = transform.localPosition;
            endPos = spinner.GetStartPoint(this, state);

        }
        else {
            transform.localPosition = Vector3.Lerp(startPos, endPos, Easing.Exponential.In(t));

            //transform.localPosition = endPos; 
        }

    }

    public void AnimateFromSpinner(State state, Spinner spinner, float t) {

        if (isShadow)
        {
            if (shadowType == ShadowType.Mirror)
            {
                state = 1 - state;
            }
        }

        if (t == 0)
        {
            startPos = transform.localPosition;
            endPos = Vector3.up * ypos;

            if (state == State.Day)
            {
                endPos += (Game.borderDistance - radius) * Vector3.right;
            }
            else if (state == State.Night)
            {
                endPos += -(Game.borderDistance - radius) * Vector3.right;
            }
        }
        else 
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, Easing.Exponential.Out(t));

            if (t == 1)
                grounded = true;

            //transform.localPosition = endPos;
        }

    }

    IEnumerator Spin(State state) {

        float scrollSpeed = Game.instance.scrollSpeed;

        Game.instance.scrollSpeed = 0;

        //к спиннеру

        AnimateToSpinner(state, spinner, 0);

        //тени
        for (int i = 0; i < shadows.Length; i++)
        {
            shadows[i].AnimateToSpinner(state, spinner, 0);
        }

        Vector3 viewStartPos = Game.instance.view.transform.position;
        Vector3 viewEndPos = spinner.transform.position;

        for (float t = 0; t < 0.98f; t += Time.deltaTime * Player.instance.ballSpeed)
        {
            AnimateToSpinner(state, spinner, t);

            //тени
            for (int i = 0; i < shadows.Length; i++)
            {
                shadows[i].AnimateToSpinner(state, spinner, t);
            }

            Game.instance.view.transform.position = Vector3.Lerp(viewStartPos, viewEndPos, Easing.Exponential.In(t));

            //Game.SetColorTransition(t);

            yield return null;
        }

        AnimateToSpinner(state, spinner, 1);

        //тени
        for (int i = 0; i < shadows.Length; i++)
        {
            shadows[i].AnimateToSpinner(state, spinner, 1);
        }

        Game.instance.view.transform.position = viewEndPos;

        //спиннер

        spinner.Spin2(this, state);

        ////тени
        //for (int i = 0; i < shadows.Length; i++)
        //{
        //    spinner.Spin2(shadows[i], state);
        //}

        while (spinner.isSpinning) {
            //Debug.Log("?");
            spinner.Spin2(this, state);

            spinner.AnimateSpin(this, state);

            //тени
            for (int i = 0; i < shadows.Length; i++)
            {
                spinner.AnimateSpin(shadows[i], state);
            }

            yield return null;
        }

        //от спиннера

        //Game.FlipState();

        if (!spinner.failed)
        {
            Game.instance.scrollSpeed = scrollSpeed;

            viewStartPos = Game.instance.view.transform.position;
            viewEndPos = viewStartPos + Vector3.up * scrollSpeed;
        }
        else 
        {
            viewStartPos = Game.instance.view.transform.position;
            viewEndPos = viewStartPos + Vector3.up * -ypos;
        }

        //viewStartPos = Game.instance.view.transform.position;
        //viewEndPos = viewStartPos + Vector3.up * -ypos;

        AnimateFromSpinner(state, spinner, 0);

        for (float t = 0; t < 0.98f; t += Time.deltaTime * Player.instance.ballSpeed)
        {
            AnimateFromSpinner(state, spinner, t);

            //тени
            for (int i = 0; i < shadows.Length; i++)
            {
                shadows[i].AnimateFromSpinner(state, spinner, t);
            }

            Game.instance.view.transform.position = Vector3.Lerp(viewStartPos, viewEndPos, Easing.Exponential.Out(t));
            Game.instance.view.transform.position += Game.instance.scrollSpeed * Vector3.up * Time.deltaTime;

            //if (!spinner.failed)
            //{
            //    Vector3 tmp = Vector3.Lerp(Vector3.up * scrollSpeed * 0.2f, Vector3.zero, Easing.Exponential.Out(t));
            //    Game.instance.view.transform.position += tmp;
            //}
            //else {
            //    Game.instance.view.transform.position = Vector3.Lerp(viewStartPos, viewEndPos, Easing.Exponential.Out(t));
            //}

            //Game.SetColorTransition(t);
            yield return null;
        }

        AnimateFromSpinner(state, spinner, 1);

        //тени
        for (int i = 0; i < shadows.Length; i++)
        {
            shadows[i].AnimateFromSpinner(state, spinner, 1);
        }

        //Game.instance.view.transform.position = viewEndPos;

        Game.instance.scrollSpeed = scrollSpeed;

        yield return null;

        //speed up
        Game.instance.speedModifier += Game.instance.speedModifier * Mathf.Pow(2, lvl) * 0.001f;

        yield return null;
    }

    public static void UpdateBoomColor()
    {
        if (instance == null)
            return;

        ParticleSystem.MainModule main = instance.boom.main;

        Color startColor = Color.white;

        if (Game.gameState == State.Day)
        {
            startColor = Game.GetColorTheme().playerNightColor;
        }
        else if (Game.gameState == State.Night)
        {
            startColor = Game.GetColorTheme().playerDayColor;
        }

        main.startColor = startColor;

        for (int i = 0; i < instance.shadows.Length; i++) {
            ParticleSystem.MainModule shadowMain = instance.shadows[i].boom.main;
            shadowMain.startColor = startColor;
        }
    }

    public static void UpdateTrailColor()
    {
        if (instance == null)
            return;

        instance.trail.colorGradient = GetGrad();

        for (int i = 0; i < instance.shadows.Length; i++)
        {
            instance.shadows[i].trail.colorGradient = GetGrad();
        }
    }

    public static Gradient GetGrad() {
        Gradient gradient;

        if (!Game.GetColorTheme().specialTheme)
        {
            Color accent = Game.GetColorTheme().accentColor;

            gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(accent, 0.42f), new GradientColorKey(Color.black, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
        }
        else
        {
            gradient = Game.GetColorTheme().trailGrad;

        }

        return gradient;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0, 1, 0.6f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public static float GetRadius() {
        return instance.radius;
    }
}

public enum ShadowType { 
    Mirror,
    Snake
}