using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMaster : MonoBehaviour
{
    public static SoundMaster instance;

    public static bool mute = false;

    public AudioMixer mixer;

    public static string MAIN = "Main"; // музика меню
    public static string GAME = "Game"; // музыка игры
    public static string M = "Master"; // все

    public AudioSource menuMusic;
    public AudioSource gameMusic;
    public AudioSource buttonSound;
    public AudioSource deathSound;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        mute = Convert.ToBoolean(PlayerPrefs.GetInt(Statics.SOUND, 0));
        SetMute(mute);

        mixer.SetFloat(GAME, -80);
        mixer.SetFloat(MAIN, 0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator A() {
        yield return null;
    }

    // плавный переход
    public static void CrossFade() {

        instance.gameMusic.Play();

        instance.StartCoroutine(StartFade(instance.mixer, GAME, 1, 1));
        instance.StartCoroutine(StartFade(instance.mixer, MAIN, 1, 0));
    }

    public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;

        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }

        yield break;
    }

    //на кнопке
    public void SoundToggle() {
        mute = !mute;
        SetMute(mute);
    }

    public void SetMute(bool m) {
        if (m)
        {
            instance.mixer.SetFloat(M, -80);
        }
        else
        {
            instance.mixer.SetFloat(M, 0);
        }

        PlayerPrefs.SetInt(Statics.SOUND, Convert.ToInt32(m));
    }

    //при смерти
    public static void StopMusic() {
        instance.menuMusic.Stop();
        instance.gameMusic.Stop();
    }

    public static void StartMusic()
    {
        instance.menuMusic.Play();
        instance.gameMusic.Play();
    }

    public void PlayBTNSound() {
        buttonSound.Play();
    }

    public void PlayDeathSound()
    {
        deathSound.Play();
    }
}
