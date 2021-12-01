using System;
using System.Collections.Generic;
using bosqmode.libvlc;
using UnityEngine;

public class VLCEnvironment : IDisposable
{
    public static readonly VLCEnvironment i = new VLCEnvironment();

    private const string defaultArgs = "--ignore-config;--no-xlib;--no-video-title-show;--no-osd;";
    
    private IntPtr libvlc = IntPtr.Zero;

    class MediaCache
    {
        public int usage = 0;
        public IntPtr media = IntPtr.Zero;
    }
    private Dictionary<string, MediaCache> mediaList = new Dictionary<string, MediaCache>();

    private Dictionary<string, VLCPlayer> vlcPlayers = new Dictionary<string, VLCPlayer>();

    public VLCPlayer CreatePlayer(string id, string mediaUrl, bool hls)
    {
        var player = new VLCPlayer(mediaUrl, hls);
        vlcPlayers[id] = player;
        return player;
    }
    
    public void RemovePlayer(string id)
    {
        vlcPlayers[id].Dispose();
        vlcPlayers.Remove(id);
    }

    public IntPtr GetVLCPtr()
    {
        if (libvlc == IntPtr.Zero)
        {
            string argstrings = defaultArgs;

            string[] args = argstrings.Split(';');

            Debug.Log("libvlc_new");
            libvlc = LibVLCWrapper.libvlc_new(args.Length, args);
            
            if (libvlc == IntPtr.Zero)
            {
                Debug.LogError("Failed loading new libvlc instance...");
                return IntPtr.Zero;
            }
            Debug.Log("New VLC");
        }
        return libvlc;
    }

    public IntPtr CreateMedia(string url)
    {
        if (mediaList.TryGetValue(url, out var value))
        {
            value.usage += 1;
            //mediaList[url].usage += 1;
            Debug.Log("Already existed: " + value.usage);
            return value.media;
        }
        else
        {
            Debug.Log("New Media");
            IntPtr media = LibVLCWrapper.libvlc_media_new_location(libvlc, url);
            if (media == IntPtr.Zero)
            {
                Debug.LogError("For some reason media is null, maybe typo in the URL?");
                return IntPtr.Zero;
            }
            
            /*LibVLCWrapper.libvlc_media_add_option(media, "-vvv");
            LibVLCWrapper.libvlc_media_add_option(media, "--avcodec-dr");
            LibVLCWrapper.libvlc_media_add_option(media, "--clock-jitter=1500");
            LibVLCWrapper.libvlc_media_add_option(media, "--live-caching=1500");
            LibVLCWrapper.libvlc_media_add_option(media, "--network-caching=1500");
            LibVLCWrapper.libvlc_media_add_option(media, "--file-caching=3000");
            LibVLCWrapper.libvlc_media_add_option(media, "--no-drop-late-frames");
            LibVLCWrapper.libvlc_media_add_option(media, "--no-skip-frames");
            LibVLCWrapper.libvlc_media_add_option(media, "--no-sout-smem-time-sync");
            LibVLCWrapper.libvlc_media_add_option(media, "--sout-mp4-faststart");
            LibVLCWrapper.libvlc_media_add_option(media, "--sout-x264-partitions=fast");
            LibVLCWrapper.libvlc_media_add_option(media, "--adaptive-logic=nearoptimal");
            LibVLCWrapper.libvlc_media_add_option(media, "--adaptive-use-access");
            LibVLCWrapper.libvlc_media_add_option(media, "--avcodec-threads=10");
            LibVLCWrapper.libvlc_media_add_option(media, "--sout-x264-psy");
            LibVLCWrapper.libvlc_media_add_option(media, "--aout=opensles");
            LibVLCWrapper.libvlc_media_add_option(media, "--demuxdump-append");
            LibVLCWrapper.libvlc_media_add_option(media, "--avcodec-hw=d3d11va");*/

            var mediaCache = new MediaCache
            {
                media = media,
                usage = 1
            };

            mediaList[url] = mediaCache;
            return media;
        }
    }
    public void ReleaseMedia(string url)
    {
        if (mediaList.TryGetValue(url, out var value))
        {
            value.usage -= 1;

            if (value.usage == 0)
            {
                LibVLCWrapper.libvlc_media_release(value.media);
                mediaList.Remove(url);
            }
        }
    }

    public void Dispose()
    {
        foreach(var player in vlcPlayers.Values)
        {
            if (player != null)
            {
                player.Dispose();
            }
        }
        vlcPlayers.Clear();

        foreach(var mediaCache in mediaList.Values)
        {
            if (mediaCache.media != IntPtr.Zero)
            {
                LibVLCWrapper.libvlc_media_release(mediaCache.media);
            }
        }
        mediaList.Clear();

        if (libvlc != IntPtr.Zero)
        {
            LibVLCWrapper.libvlc_release(libvlc);
        }
        
        libvlc = IntPtr.Zero;
        
        Debug.Log("Dispose VLCEnvironment");
    }
}
