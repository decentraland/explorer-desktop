using System.Collections;
using System.Collections.Generic;
using bosqmode.libvlc;
using DCL.Components.Video.Plugin;
using UnityEngine;

public class VideoPluginWrapper_VLC : IVideoPluginWrapper
{
    struct VLCPlayerAdapter
    {
        public Texture2D texture;
        public VLCPlayer player;
        public bool neverPlayed;
    }
    Dictionary<string, VLCPlayerAdapter> videos = new Dictionary<string, VLCPlayerAdapter>();
    public void Create(string id, string url, bool useHls)
    {
        var adapter = new VLCPlayerAdapter();
        adapter.player = VLCEnvironment.i.CreatePlayer(id, url, useHls);
        adapter.neverPlayed = true;
        adapter.texture = null;
        videos[id] = adapter;
    }

    public void Remove(string id)
    {
        VLCEnvironment.i.RemovePlayer(id);
        videos.Remove(id);
    }

    public void TextureUpdate(string id)
    {
        var adapter = videos[id];
        if (adapter.player.CheckForImageUpdate(out var img))
        {
            adapter.texture.LoadRawTextureData(img);
            adapter.texture.Apply();
        }
    }

    public Texture2D PrepareTexture(string id)
    {
        return GetTexture(id);
    }
    public Texture2D GetTexture(string id)
    {
        Texture2D texture = null;
        if (videos.TryGetValue(id, out var video))
        {
            var player = video.player;
            if (player.ready)
            {
                if (video.texture == null)
                {
                    texture = new Texture2D(player.width, player.height, TextureFormat.BGRA32, false, false);
                    video.texture = texture;
                    videos[id] = video;
                }
                else
                {
                    texture = video.texture;
                }
            }
        }
        return texture;
    }

    public void Play(string id, float startTime)
    {
        if (videos.TryGetValue(id, out var video))
        {
            if (video.neverPlayed)
            {
                video.neverPlayed = false;
                videos[id] = video;
            }
            video.player.Play();
            if (startTime >= 0.0f)
                video.player.SetTime(startTime);    
        }
    }

    public void Pause(string id)
    {
        videos[id].player.Pause();
    }

    public void SetVolume(string id, float volume)
    {
        videos[id].player.SetVolume(volume);
    }

    public int GetHeight(string id)
    {
        return videos[id].player.width;
    }

    public int GetWidth(string id)
    {
        return videos[id].player.height;
    }

    public float GetTime(string id)
    {
        return videos[id].player.GetTime();
    }

    public float GetDuration(string id)
    {
        return videos[id].player.GetDuration();
    }

    public VideoState GetState(string id)
    {
        var vlcState = videos[id].player.GetState();
        VideoState state = VideoState.NONE;

        if (videos.TryGetValue(id, out var video))
        {
            if (!video.player.ready)
            {
                state = VideoState.LOADING;
            }
            else if (video.player.ready && video.neverPlayed)
            {
                return VideoState.READY;
            }
            else
            {
                switch (vlcState)
                {
                    case libvlc_state_t.libvlc_Buffering:
                    case libvlc_state_t.libvlc_Opening:
                        state = VideoState.LOADING;
                        break;
                    case libvlc_state_t.libvlc_Error:
                        state = VideoState.ERROR;
                        break;
                    case libvlc_state_t.libvlc_Ended:
                    case libvlc_state_t.libvlc_Paused:
                    case libvlc_state_t.libvlc_Stopped:
                        state = VideoState.PAUSED;
                        break;
                    case libvlc_state_t.libvlc_Playing:
                        state = VideoState.PLAYING;
                        break;
                    case libvlc_state_t.libvlc_NothingSpecial:
                        state = VideoState.NONE;
                        break;
                    default:
                        state = VideoState.NONE;
                        break;
                }
            }
        }

        return state;
    }

    public string GetError(string id)
    {
        return videos[id].player.errorDescription;
    }

    public void SetTime(string id, float second)
    {
        videos[id].player.SetTime(second);
    }

    public void SetPlaybackRate(string id, float playbackRate)
    {
        videos[id].player.SetPlaybackRate(playbackRate);
    }

    public void SetLoop(string id, bool loop)
    {
        videos[id].player.SetLoop(loop);
    }
}
