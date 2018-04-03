using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundHandler : MonoBehaviour {

    public AudioSource source;
    public AudioClip clip;
    public MixGroup group = MixGroup.Objects;
    public bool loop;

    void Start () {
        AddSourceToMixGroup();
        source.clip = clip;
        source.loop = loop;
    }

    void AddSourceToMixGroup () {
        switch (group) {
            case MixGroup.Music:
                source.AddToMusicMixGroup();
                break;
            case MixGroup.Player:
                source.AddToPlayerMixGroup();
                break;
            case MixGroup.Objects:
                source.AddToObjectsMixGroup();
                break;
            case MixGroup.Ambient:
                source.AddToAmbientMixGroup();
                break;
        }
    }

    public void Play () {
        source.Play();
    }

    public void PlayContinuous () {
        if (!source.isPlaying) {
            source.Play();
        }
    }

    public void Stop () {
        source.Stop();
    }

}
