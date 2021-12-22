using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Multimedia;
using UnityEngine;

public class FFMPEGPlayer : MonoBehaviour
{
    public string videoPath = "";

    private FFMPEGDecoder decoder;
    private Texture2D localTexture = null;
    
    void Awake()
    {
        FFMPEGDecoderWrapper.nativeCleanAll();
        decoder = new FFMPEGDecoder(videoPath);
    }
    
    void Update()
    {
        if (decoder.getDecoderState() == FFMPEGDecoder.DecoderState.INITIALIZED)
        {
            Material material = GetComponent<MeshRenderer>().sharedMaterial;
            Texture2D texture = decoder.GetTexture();
            if (texture != null && localTexture == null)
            {
                material.mainTexture = texture;
                localTexture = texture;
                decoder.Play();
            }
        }
        else if (decoder.getDecoderState() == FFMPEGDecoder.DecoderState.START)
        {
            decoder.UpdateVideoTexture();
        }
    }
}
