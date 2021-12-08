using System;
using System.Collections.Generic;
using DCL.Components.Video.Plugin;
using HTC.UnityPlugin.Multimedia;
using UnityEngine;

public class VideoPluginWrapper_FFMPEG : IVideoPluginWrapper
{
    private Dictionary<string, FFMPEGDecoder> decoderList = new Dictionary<string, FFMPEGDecoder>();

    public void Create(string id, string url, bool useHls)
    {
        decoderList.Add(id, new FFMPEGDecoder(url));
    }

    public void Remove(string id)
    {
        decoderList[id].Dispose();
        decoderList.Remove(id);
    }

    public void TextureUpdate(string id)
    {
        decoderList[id].UpdateVideoTexture();
    }

    public Texture2D PrepareTexture(string id)
    {
        return decoderList[id].GetTexture();
    }

    public void Play(string id, float startTime)
    {
        decoderList[id].Play();
        if (startTime > 0)
        {
            decoderList[id].setSeekTime(startTime);
        }
    }

    public void Pause(string id)
    {
        decoderList[id].setPause();
    }

    public void SetVolume(string id, float volume)
    {
        decoderList[id].setVolume(volume);
    }

    public int GetHeight(string id)
    {
        return decoderList[id].getVideoHeight();
    }

    public int GetWidth(string id)
    {
        return decoderList[id].getVideoWidth();
    }

    public float GetTime(string id)
    {
        return decoderList[id].getVideoCurrentTime();
    }

    public float GetDuration(string id)
    {
        return decoderList[id].videoTotalTime;
    }

    public VideoState GetState(string id)
    {
        switch (decoderList[id].getDecoderState())
        {
            case FFMPEGDecoder.DecoderState.INIT_FAIL:
                return VideoState.ERROR;
            case FFMPEGDecoder.DecoderState.STOP:
                return VideoState.NONE;
            case FFMPEGDecoder.DecoderState.NOT_INITIALIZED:
                return VideoState.NONE;
            case FFMPEGDecoder.DecoderState.INITIALIZING:
                return VideoState.LOADING;
            case FFMPEGDecoder.DecoderState.INITIALIZED:
                return VideoState.READY;
            case FFMPEGDecoder.DecoderState.START:
                return VideoState.PLAYING;
            case FFMPEGDecoder.DecoderState.PAUSE:
                return VideoState.PAUSED;
            case FFMPEGDecoder.DecoderState.SEEK_FRAME:
                return VideoState.SEEKING;
            case FFMPEGDecoder.DecoderState.BUFFERING:
                return VideoState.BUFFERING;
            case FFMPEGDecoder.DecoderState.EOF:
                return VideoState.NONE;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public string GetError(string id)
    {
        return "";
    }

    public void SetTime(string id, float second)
    {
        decoderList[id].setSeekTime(second);
    }

    public void SetPlaybackRate(string id, float playbackRate)
    {
        decoderList[id].playbackRate = playbackRate;
    }

    public void SetLoop(string id, bool loop)
    {
        decoderList[id].loop = loop;
    }
}