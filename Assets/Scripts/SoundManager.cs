using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public enum SoundList
    {
        DIALOG,
        END_CALL_BAD,
        BROKEN_LINK,
        HALO,
        VALID_CALL,
        END_CALL_SUCCESS,
        LINK_NODE
    }

    [Header("Sons")]
    /*[SerializeField] AudioClip dialog;
    [SerializeField] AudioClip endCallBad;
    [SerializeField] AudioClip brokenLink;*/
    [SerializeField] AudioClip halo; /*
    [SerializeField] AudioClip validCall;
    [SerializeField] AudioClip endCallSuccess;*/
    [SerializeField] AudioClip linkNode;

    private AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundList sound, bool doLoop)
    {
        switch(sound)
        {
         /*   case SoundList.DIALOG:
                audioSource.clip = dialog;
                break;
            case SoundList.END_CALL_BAD:
                audioSource.clip = endCallBad;
                break;
            case SoundList.BROKEN_LINK:
                audioSource.clip = brokenLink;
                break;*/
            case SoundList.HALO:
                audioSource.clip = halo;
                break;
            /*case SoundList.VALID_CALL:
                audioSource.clip = validCall;
                break;
            case SoundList.END_CALL_SUCCESS:
                audioSource.clip = endCallSuccess;
                break;*/
            case SoundList.LINK_NODE:
                audioSource.clip = linkNode;
                break;
        }
        audioSource.loop = doLoop;
        audioSource.Play();
    }
}
