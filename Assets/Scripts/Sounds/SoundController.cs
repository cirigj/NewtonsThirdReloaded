using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public enum MixGroup {
    Music,
    Player,
    Objects,
    Ambient
}

public static class SoundControllerExtensions {

    public static void AddToPlayerMixGroup (this AudioSource source) {
        SoundController.Instance.AddSourceToMixGroup(source, "Player");
    }

    public static void AddToObjectsMixGroup (this AudioSource source) {
        SoundController.Instance.AddSourceToMixGroup(source, "Objects");
    }

    public static void AddToMusicMixGroup (this AudioSource source) {
        SoundController.Instance.AddSourceToMixGroup(source, "Music");
    }

    public static void AddToAmbientMixGroup (this AudioSource source) {
        SoundController.Instance.AddSourceToMixGroup(source, "Ambient");
    }

}

public class SoundController : MonoBehaviour {

    public static SoundController Instance {
        get {
            return GameController.Instance.soundController;
        }
    }

    public AudioMixer mixer;

    public void AddSourceToMixGroup (AudioSource source, string groupName) {
        AudioMixerGroup group = mixer.FindMatchingGroups(groupName).First();
        source.outputAudioMixerGroup = group;
    }

}
