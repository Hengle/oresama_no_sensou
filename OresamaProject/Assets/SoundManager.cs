using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public static SoundManager SM;

    public float bgmVolume;

    private AudioSource bgmSource;
    public AudioClip storyMusic;
    public AudioClip gameMusic;
    public AudioClip bossMusic;

    private AudioSource seSource;

    public enum BGM {
        story,
        game,
        boss
    };


    void Awake()
    {
        if (!SM)
            SM = this;

        if (!bgmSource)
            bgmSource = gameObject.AddComponent<AudioSource>();

        if (!seSource)
            seSource = gameObject.AddComponent<AudioSource>();
    }

    public void playBGM(BGM bgm, bool loop = true)
    {
        AudioClip clip = null;

        switch (bgm) { 
            case BGM.story:
                clip = storyMusic;
                break;

            case BGM.game:
                clip = gameMusic;
                break;

            case BGM.boss:

                clip = bossMusic;
                break;

            default:
                Debug.Log("Error, no BGM set?");
                break;
        }

        if (clip != null) 
        {
            bgmSource.clip = clip;
            bgmSource.volume = bgmVolume;
            bgmSource.loop = loop;
            bgmSource.Play();
        } 
        else 
        {
            Debug.Log("Error, No BGM set?");
        }
    }


    public void playSE(AudioClip clip)
    {
        seSource.PlayOneShot(clip);
    }

}
