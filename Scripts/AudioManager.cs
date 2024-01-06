using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioClip cardPlay;
    static AudioSource src;
    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
        cardPlay = Resources.Load<AudioClip>("card");
    }

    public static void PlayAudio(string clip)
    {
        switch(clip)
        {
            case "play":
                src.PlayOneShot(cardPlay);
                break;
            default:
                break;
        }
    }
}
