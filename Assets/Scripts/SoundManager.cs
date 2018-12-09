using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SoundManager : MonoBehaviour
{

    List<AudioSource> emitters = new List<AudioSource>();

    public enum SoundList
    {
        DIALOG,
        DIALOG_FURIOUS,
        PLUG,
        UNPLUG,
        SHUSH,
        END_CALL_BAD,
        BROKEN_LINK,
        HALO,
        VALID_CALL,
        END_CALL_SUCCESS,
        LINK_NODE
    }

    [Header("Sounds")]
    List<AudioClip> dialogs = new List<AudioClip>();
    List<AudioClip> dialogsFurious = new List<AudioClip>();
    [SerializeField] AudioClip plug;
    [SerializeField] AudioClip unplug;
    [SerializeField] AudioClip endCallBad;
    [SerializeField] AudioClip brokenLink;
    [SerializeField] AudioClip halo;
    [SerializeField] AudioClip validCall;
    [SerializeField] AudioClip endCallSuccess;
    [SerializeField] AudioClip linkNode;
    [SerializeField] AudioClip shush;


    [Header("Dialogs")]
    [SerializeField] AudioClip dialog1;
    [SerializeField] AudioClip dialog2;
    [SerializeField] AudioClip dialog3;
    [SerializeField] AudioClip dialog4;
    [SerializeField] AudioClip dialog5;
    [SerializeField] AudioClip dialog6;
    [SerializeField] AudioClip dialog7;
    [SerializeField] AudioClip dialog8;
    [SerializeField] AudioClip dialog9;

    
    [Header("DialogsFurious")]
    [SerializeField] AudioClip dialogFurious1;
    [SerializeField] AudioClip dialogFurious2;
    [SerializeField] AudioClip dialogFurious3;
    [SerializeField] AudioClip dialogFurious4;
    [SerializeField] AudioClip dialogFurious5;

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
        dialogs = new List<AudioClip>{dialog1,
                                    dialog2,
                                    dialog3,
                                    dialog4,
                                    dialog5,
                                    dialog6,
                                    dialog7,
                                    dialog8,
                                    dialog9
        };
        dialogsFurious = new List<AudioClip>{dialogFurious1,
                                    dialogFurious2,
                                    dialogFurious3,
                                    dialogFurious4,
                                    dialogFurious5
        };
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
        //Debug.Log("son :" + sound);

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
                    int hasard = Random.Range(0,dialogs.Count);
                    emitterAvailable.clip = dialogs[hasard];
                    break;
                case SoundList.DIALOG_FURIOUS:
                    int hasardFurious = Random.Range(0,dialogsFurious.Count);
                    emitterAvailable.clip = dialogsFurious[hasardFurious];
                    break;
                case SoundList.PLUG:
                    emitterAvailable.clip = plug;
                    break;
                case SoundList.SHUSH:
                    emitterAvailable.clip = shush;
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
            //Debug.Log("no emitter available");
        }
        
    }

    public void stopAllDialogs() {
        foreach(AudioSource emitter in emitters)
        {
            if(emitter.isPlaying && dialogs.Contains(emitter.clip) )
            {
                emitter.Stop();
            }
        }
    }
}
