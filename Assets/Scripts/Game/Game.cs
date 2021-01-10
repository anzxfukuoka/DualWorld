using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Game : MonoBehaviour
{

    public static Game instance;

    public AdsManager adsManager;
    public GameObject errPanel;

    [Header("Settings")]

    public float scrollSpeed = 6f;
    public float speedModifier = 0.01f;

    public float restartDelay = 2f; //время в сек
    public float waitForContinue = 2f; //время в сек

    [Header("Colors")]
    public Store store;
    public ColorTheme currentColorTheme;

    [Header("UI")]

    public UIFuncs uiFuncs;

    public Text scoreLabel;
    public Text mLabel;

    public GameObject newLvlPanel;
    public GameObject restartPanel;
    public GameObject storePanel;

    [Header("Perfabs")]

    public GameObject view;
    public GameObject leftBorder;
    public GameObject rightBorder;
    public GameObject background; 
    public GameObject player;


    [Header("Materials")]
    public Material playerMaterial;
    public Material backgroundMaterial;
    public Material wallMaterial;
    public Material blockNightMaterial;
    public Material blockDayMaterial;

    public Material accentMaterial;

    public Material numsTextMaterial;
    public Material wordsTextMaterial;


    public static State gameState; //можно было бы замениь на bool, но так красивее

    public static float borderWidth;
    public static float borderDistance;

    public static float jumpWidth;

    public static bool paused = false;

    public static bool showNewLvlPanel = false;


    public static bool isContinue = false;
    public static bool stopContinueTimer = false;

    private void OnEnable()
    {
        borderDistance = rightBorder.transform.position.x - rightBorder.transform.localScale.x / 2f;
        borderWidth = rightBorder.transform.localScale.x;

        jumpWidth = borderWidth * 2;

        //SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        //playerSpriteRenderer.material = playerMaterial;

        instance = this;

        gameState = State.Night;
    }

    float m = 0;

    

    void Start()
    {
        //PlayerPrefs.DeleteAll();

        //load theme
        string cctName = PlayerPrefs.GetString(Statics.CURRENTTHEME, "Default");
        SetColorTheme(cctName);

        if ((State)PlayerPrefs.GetInt(Statics.LASTSTATE, 1) == State.Day)
        {
            currentColorTheme.FlipColors();
        }

        ResetColors();

        Player.score = PlayerPrefs.GetInt(Statics.HIGHTSCORE, 0);


        GamePause(true);

        if (showNewLvlPanel) {
            newLvlPanel.SetActive(true);
            showNewLvlPanel = false;
        }

        
    }

    void OnApplicationQuit()
    {
        ResetColors();
    }

    public void ResetColors()
    {
        //Materials
        wallMaterial.color = currentColorTheme.borderNightColor;
        backgroundMaterial.color = currentColorTheme.backgroundNightColor;

        blockDayMaterial.color = currentColorTheme.backgroundDayColor;
        blockNightMaterial.color = currentColorTheme.backgroundNightColor;

        playerMaterial.color = currentColorTheme.playerDayColor;

        accentMaterial.color = currentColorTheme.accentColor;
        //accentMaterial.SetColor("_EmisColor", currentColorTheme.accentColor);

        numsTextMaterial.color = currentColorTheme.backgroundDayColor;
        wordsTextMaterial.color = currentColorTheme.playerDayColor;

        //pl
        Player.UpdateBoomColor();
        Player.UpdateTrailColor();

        //UI
        //scoreLabel.color = currentColorTheme.backgroundDayColor;
        //touchTSLabel.color = currentColorTheme.playerDayColor;

        //uiFuncs.UpdateUIColors();
        
        //это уже не костыль, а инвалидная коляска
        Store.instance = store;
        Store.SetPreviewColorTheme(currentColorTheme);
    }

    void Update()
    {
        //view scroll
        if (Player.alive && !paused) {
            //view.transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed);
            
            Scroll(Time.deltaTime * scrollSpeed);
            
            Player.instance.ballSpeed += Player.instance.ballSpeed * speedModifier;

            scrollSpeed += scrollSpeed * speedModifier;

            //speedModifier = 0.01f * Player.lvl;
        }

        //m
        scoreLabel.text = Player.score + "";

        mLabel.text = (int)(Player.GetM() - Player.GetM() % 10) + "m";

    }

    public static void Scroll(float ds) {
        instance.view.transform.Translate(Vector3.up * ds);
    }

    public void GameStart() {
        Player.score = 0;
        Player.lvl = 0;

        GamePause(false);

        SoundMaster.CrossFade();
    }

    //Set pause
    public void GamePause(bool p) {
        if (p)
        {
            paused = true;
            Time.timeScale = 0;
        }
        else {
            paused = false;
            Time.timeScale = 1;
        }
    }
    

    public void GameOver() {

        SoundMaster.StopMusic();

        if (PlayerPrefs.GetInt(Statics.HIGHTSCORE, 0) < Player.score) 
        {
            //новый рекорд
            PlayerPrefs.SetInt(Statics.HIGHTSCORE, Player.score);
        }

        if (PlayerPrefs.GetInt(Statics.HIGHTLVL, 1) < Player.lvl)
        {
            //новый уровень
            PlayerPrefs.SetInt(Statics.HIGHTLVL, Player.lvl);
            showNewLvlPanel = true;
        }

        if (PlayerPrefs.GetInt(Statics.MAXDISTANCE, 0) < (int)Player.GetM())
        {
            //метры
            PlayerPrefs.SetInt(Statics.MAXDISTANCE, (int)Player.GetM());
        }

        //State
        PlayerPrefs.SetInt(Statics.LASTSTATE, (int)gameState);

        //магаз
        Store.AddStarsPoints(Player.score);

        StartCoroutine(Re());
    }

    public static float timeToRestart;
    
    public static float deathCounter = 0; // обнуляется на OnShowAdd

    //restart
    public IEnumerator Re() {

        if (!AdsManager.NoAds)
            deathCounter++;
        else
            deathCounter = 0;

        if (deathCounter > 5)
        {
            adsManager.ShowInterstitial(gameObject);

            while (deathCounter != 0)
            {
                yield return null;
            }

            isContinue = true;
        }

        // один раз уже continue жмякнули
        if (isContinue)
        {
            Debug.Log("?");
            yield return new WaitForSeconds(restartDelay);

            isContinue = false;
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        else {

            timeToRestart = 0;

            restartPanel.SetActive(true);

            while (timeToRestart <= waitForContinue)
            {

                if (isContinue)
                {
                    Player.Continue();
                    SoundMaster.StartMusic();
                    restartPanel.SetActive(false);
                    break;
                }

                //stopContinueTimer = true => пользователь смотрит рекламу (ну или хотя бы пытается)
                if (!stopContinueTimer)
                {
                    timeToRestart += Time.unscaledDeltaTime;
                }

                yield return null;
            }

            if(!isContinue)
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        
    }

    public void OnShowAdd(bool success)
    {
        deathCounter = 0;
    }

    public static bool IsUIActive() {
        return instance.restartPanel.activeSelf || instance.storePanel.activeSelf || instance.newLvlPanel.activeSelf;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.right * borderDistance + Vector3.down * 50, transform.position + Vector3.right * borderDistance + Vector3.up * 50);
        Gizmos.DrawLine(transform.position + Vector3.left * borderDistance + Vector3.down * 50, transform.position + Vector3.left * borderDistance + Vector3.up * 50);

        Camera camera = Camera.main;

        Gizmos.color = Color.cyan;
        Vector3 p1 = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        Vector3 p2 = camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane));
        Gizmos.DrawSphere(p1, 0.1F);
        Gizmos.DrawSphere(p2, 0.1F);
    }

    //анимация цвета
    public static void SetColorTransition(float t) {
        // x = 0 -> State.Day
        // x = 1 -> State.Night

        float x = 0;

        if (gameState == State.Day)
            x = t;
        else if (gameState == State.Night)
            x = 1 - t;

        Color playerColor = Color.Lerp(instance.currentColorTheme.playerDayColor, instance.currentColorTheme.playerNightColor, x);
        instance.playerMaterial.color = playerColor;

        Color borderColor = Color.Lerp(instance.currentColorTheme.borderNightColor, instance.currentColorTheme.borderDayColor, x);
        instance.wallMaterial.color = borderColor;

        Color backgroundColor = Color.Lerp(instance.currentColorTheme.backgroundNightColor, instance.currentColorTheme.backgroundDayColor, x);
        instance.backgroundMaterial.color = backgroundColor;

        Color textColor = Color.Lerp(instance.currentColorTheme.backgroundNightColor, instance.currentColorTheme.backgroundDayColor, 1 - x);
        //instance.scoreLabel.color = textColor;
        
        instance.numsTextMaterial.color = textColor;
        instance.wordsTextMaterial.color = playerColor;

        //instance.touchTSLabel.color = playerColor;
    }

    public static void FlipState() {
        if (gameState == State.Day) {
            gameState = State.Night;
        }
        else if (gameState == State.Night) {
            gameState = State.Day;
        }
    }

    public static ColorTheme FindThemeByName(string themeName)
    {
        // в корне сценны лежит пустой обьект с именем Themes
        // в нем пустые обьекты со скриптами ColorTheme
        // название темы - имя обьекта, на котором висит скрипт
        
        ColorTheme re = null;
        
        try
        {
            re = GameObject.Find("/Themes/" + themeName).GetComponent<ColorTheme>();
        }
        catch { }

        return re;
    }

    public static void SetColorTheme(string themeName)
    {
        ColorTheme cct = FindThemeByName(themeName);

        if (cct != null) 
        {
            PlayerPrefs.SetString(Statics.CURRENTTHEME, themeName);

            instance.currentColorTheme = cct;
            instance.ResetColors();
        }
    }

    public static ColorTheme GetColorTheme() {
        return instance.currentColorTheme;
    }

}

public enum State
{
    Day = 0,
    Night = 1
}