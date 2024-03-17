using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip click;
    [SerializeField]
    private AudioClip hover;
    [SerializeField]
    private AudioClip exit;
    [SerializeField]
    private AudioClip accept;
    [SerializeField]
    private AudioClip mainMenuMusic;
    [SerializeField]
    private AudioClip gameMusic;
    [SerializeField]
    private AudioClip pause;
    [SerializeField]
    private AudioClip unpause;
    [SerializeField]
    private AudioClip denied;
    [SerializeField]
    private AudioClip levelUp;
    [SerializeField]
    private AudioClip win;
    [SerializeField]
    public AudioClip step;
    [SerializeField]
    public AudioClip hit;
    [SerializeField]
    public AudioClip death;
    [SerializeField]
    public AudioClip attack;
    [SerializeField]
    public AudioClip block;
    [SerializeField]
    public AudioClip teleport;
    [SerializeField]
    public AudioClip encounter;

    public static MenuMusicManager instance;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void PlayClick()
    {
        GetComponent<AudioSource>().PlayOneShot(click);
    }
    public void PlayHover()
    {
        GetComponent<AudioSource>().PlayOneShot(hover);
    }
    public void PlayExit()
    {
        GetComponent<AudioSource>().PlayOneShot(exit);
    }
    public void PlayAccept()
    {
        GetComponent<AudioSource>().PlayOneShot(accept);
    }
    public void StopAudio()
    {
        GetComponent<AudioSource>().Stop();
    }
    public void PlayMenuMusic()
    {
        StopAudio();
        GetComponent<AudioSource>().clip = mainMenuMusic;
        GetComponent<AudioSource>().Play();
    }
    public void PlayGameMusic()
    {
        StopAudio();
        GetComponent<AudioSource>().clip = gameMusic;
        GetComponent<AudioSource>().Play();
    }
    public void PlayPause()
    {
        GetComponent<AudioSource>().PlayOneShot(pause);
    }
    public void PlayUnpause()
    {
        GetComponent<AudioSource>().PlayOneShot(unpause);
    }
    public void PlayDenied()
    {
        GetComponent<AudioSource>().PlayOneShot(denied);
    }
    public void PlayLevelUp()
    {
        GetComponent<AudioSource>().PlayOneShot(levelUp);
    }
    public void PlayWin()
    {
        GetComponent<AudioSource>().PlayOneShot(win);
    }
    public static float GetDecibelValue(float value)
    {
        float normalizedValue = value / 100;
        float decibelValue = Mathf.Log10(normalizedValue) * 20;
        float mappedValue = Mathf.Clamp(decibelValue, -80f, 0f);
        //Mathf.Clamp maps the value to the range -80 to 0, as decibel values of input 0 is -infinity, so needs limiting.
        return mappedValue;
    }
}
