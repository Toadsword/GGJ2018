using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SoundManager : MonoBehaviour
{

    List<AudioSource> emitters = new List<AudioSource>();

    public enum SoundList
    {
        DIALOG,
        PLUG,
        UNPLUG,
        END_CALL_BAD,
        BROKEN_LINK,
        HALO,
        VALID_CALL,
        END_CALL_SUCCESS,
        LINK_NODE
    }

    [Header("Sounds")]
    [SerializeField] AudioClip dialog;
    [SerializeField] AudioClip plug;
    [SerializeField] AudioClip unplug;
    [SerializeField] AudioClip endCallBad;
    [SerializeField] AudioClip brokenLink;
    [SerializeField] AudioClip halo;
    [SerializeField] AudioClip validCall;
    [SerializeField] AudioClip endCallSuccess;
    [SerializeField] AudioClip linkNode;

    [Header("Emmiters")]
    [SerializeField] AudioSource emitter1;
    [SerializeField] AudioSource emitter2;
    [SerializeField] AudioSource emitter3;
    [SerializeField] AudioSource emitter4; 
    [SerializeField] AudioSource emitter5;
    [SerializeField] AudioSource emitter6;
    [SerializeField] AudioSource emitter7;
    [SerializeField] AudioSource emitter8;
    [SerializeField] AudioSource emitter9;
    [SerializeField] AudioSource emitter10;
   

    // Use this for initialization
    void Start ()
    {
        //audioSource = GetComponent<AudioSource>();
        emitters = new List<AudioSource> { emitter1,
                                    emitter2,
                                    emitter3,
                                    emitter4,
                                    emitter5,
                                    emitter6,
                                    emitter7,
                                    emitter8,
                                    emitter9,
                                    emitter10};
    }

    public void PlaySound(SoundList sound, bool doLoop=false)
    {
        Debug.Log("son :" + sound);

        AudioSource emitterAvailable = null;

        foreach(AudioSource emitter in emitters)
        {
            if(!emitter.isPlaying)
            {
                emitterAvailable = emitter;
            }
        }

        if (emitterAvailable != null)
        {
            switch (sound)
            {
                case SoundList.DIALOG:
                    emitterAvailable.clip = dialog;
                    break;
                case SoundList.PLUG:
                    emitterAvailable.clip = plug;
                    break;
                case SoundList.UNPLUG:
                    emitterAvailable.clip = unplug;
                    break;
                case SoundList.END_CALL_BAD:
                    emitterAvailable.clip = endCallBad;
                    break;
                case SoundList.BROKEN_LINK:
                    emitterAvailable.clip = brokenLink;
                    break;
                case SoundList.HALO:
                    emitterAvailable.clip = halo;
                    break;
                case SoundList.VALID_CALL:
                    emitterAvailable.clip = validCall;
                    break;
                case SoundList.END_CALL_SUCCESS:
                    emitterAvailable.clip = endCallSuccess;
                    break;
                case SoundList.LINK_NODE:
                    emitterAvailable.clip = linkNode;
                    break;
            }

            emitterAvailable.loop = false;
            emitterAvailable.Play();

        }
        else
        {
            Debug.Log("no emitter available");
        }

        
        
    }
}
