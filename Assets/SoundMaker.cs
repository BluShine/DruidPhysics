using UnityEngine;
using System.Collections.Generic;

public class SoundMaker : MonoBehaviour {

    public static SoundMaker instance;

    List<AudioSource> sounds;

    public AudioSource bSource;
    public AudioSource cSource;
    public AudioSource kSource;
    public AudioSource fSource;
    public AudioSource tSource;

	// Use this for initialization
	void Start () {
        instance = this;
        sounds = new List<AudioSource>();
        foreach(AudioSource a in transform.GetComponentsInChildren<AudioSource>())
        {
            sounds.Add(a);
        }
    }

    public void Block()
    {
        bSource.Play();
        playRandomSounds(bSource.clip);
    }

    public void Clunk()
    {
        cSource.Play();
        playRandomSounds(cSource.clip);
    }

    public void Shake()
    {
        kSource.Play();
        playRandomSounds(kSource.clip);
    }

    public void Shift()
    {
        fSource.Play();
        playRandomSounds(fSource.clip);
    }

    public void Twinkle()
    {
        tSource.Play();
        playRandomSounds(tSource.clip);
    }
    
    void playRandomSounds(AudioClip clip)
    {
        foreach(AudioSource a in sounds)
        {
            if(Random.value > .5f)
            {
                a.clip = clip;
                a.pitch = Random.value + .5f;
                a.PlayDelayed(Random.value * .1f);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
