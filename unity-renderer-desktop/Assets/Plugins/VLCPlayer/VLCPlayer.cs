using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace bosqmode.libvlc
{
    public class VLCPlayer : IDisposable
    {
        public bool isError { get; private set; }
        public string errorDescription { get; private set; }

        private IntPtr mediaList;
        private IntPtr mediaListPlayer;
        private IntPtr media;
        private IntPtr mediaPlayer;
        private IntPtr imageIntPtr;
        private IntPtr libvlc;

        private LockCB lockHandle;
        private UnlockCB unlockHandle;
        private DisplayCB displayHandle;
        private GCHandle gcHandle;

        private bool locked = true;
        private bool imageUpdate = false;
        private byte[] currentImage;
        public int width { get; private set; } = 0;
        public int height { get; private set; } = 0;
        private int channels = 4;

        private libvlc_video_track_t? videoTrack = null;
        private volatile bool cancel = false;
        private readonly string mediaUrl;
        private readonly bool hls = false;
        public bool ready { get; private set; } = false;

        /// <summary>
        /// Checks if image has been updated since last check, and outs the image bytes
        /// </summary>
        /// <param name="currentImage">arr where to store the image bytes</param>
        /// <returns>whether update happened or not</returns>
        public bool CheckForImageUpdate(out byte[] currentImage)
        {
            currentImage = null;
            if (imageUpdate)
            {
                currentImage = this.currentImage;
                imageUpdate = false;
                return true;
            }
            return false;
        }

        public VLCPlayer(string mediaUrl, bool hls)
        {
            this.hls = hls;
            this.mediaUrl = mediaUrl;
            Debug.Log("Playing: " + mediaUrl);

            gcHandle = GCHandle.Alloc(this);

            libvlc = VLCEnvironment.i.GetVLCPtr();

            if (libvlc == IntPtr.Zero)
            {
                errorDescription = "Failed loading new libvlc instance...";
                Debug.LogError(errorDescription);
                isError = true;
                return;
            }

            media = VLCEnvironment.i.CreateMedia(mediaUrl);

            if (media == IntPtr.Zero)
            {
                errorDescription = "For some reason media is null, maybe typo in the URL?";
                Debug.LogError(errorDescription);
                isError = true;
                return;
            }
            
            mediaPlayer = LibVLCWrapper.libvlc_media_player_new(libvlc);
            LibVLCWrapper.libvlc_media_player_set_media(mediaPlayer, media);
            
            mediaList = LibVLCWrapper.libvlc_media_list_new(libvlc);
            LibVLCWrapper.libvlc_media_list_add_media(mediaList, media);

            mediaListPlayer = LibVLCWrapper.libvlc_media_list_player_new(libvlc);
            LibVLCWrapper.libvlc_media_list_player_set_media_list(mediaListPlayer, mediaList);
            LibVLCWrapper.libvlc_media_list_player_set_media_player(mediaListPlayer, mediaPlayer);


            lockHandle = vlc_lock;
            unlockHandle = vlc_unlock;
            displayHandle = vlc_picture;

            LibVLCWrapper.libvlc_video_set_callbacks(mediaPlayer, lockHandle, unlockHandle, displayHandle, GCHandle.ToIntPtr(gcHandle));
            LibVLCWrapper.libvlc_video_set_format(mediaPlayer, "RV32", (uint)this.width, (uint)this.height, (uint)this.width * (uint)channels);
            LibVLCWrapper.libvlc_media_list_player_play(mediaListPlayer);
            if (!hls) LibVLCWrapper.libvlc_media_list_player_set_pause(mediaListPlayer, 1);

            System.Threading.Thread t = new System.Threading.Thread(TrackReaderThread);
            t.Start();
            
            Debug.Log("Done");
        }

        private void TrackReaderThread()
        {
            const int maxAttempts = 240; // 2 minutes until fails
            int trackGetAttempts = 0;
            while (trackGetAttempts < maxAttempts && cancel == false)
            {
                libvlc_video_track_t? track = GetVideoTrack();

                if (track.HasValue && track.Value.i_width > 0 && track.Value.i_height > 0)
                {
                    videoTrack = track;
                    
                    if (width <= 0 || height <= 0)
                    {
                        width = (int)videoTrack.Value.i_width;
                        height = (int)videoTrack.Value.i_height;
                        LibVLCWrapper.libvlc_video_set_format(mediaPlayer, "RV32", videoTrack.Value.i_width, videoTrack.Value.i_height, (uint)width * (uint)channels);
                        ready = true;
                    }
                    break;
                }

                trackGetAttempts++;
                System.Threading.Thread.Sleep(500);
            }

            if (trackGetAttempts >= maxAttempts)
            {
                errorDescription = "Maximum attempts of getting video track reached, maybe opening failed?";
                Debug.LogError(errorDescription);
                isError = true;
            }
            else
            {
                Debug.Log("VLCPlayer should work!");
            }
        }

        private static void vlc_unlock(IntPtr opaque, IntPtr picture, ref IntPtr planes)
        {
            GCHandle handle = (GCHandle) opaque;
            VLCPlayer player = (VLCPlayer) handle.Target;
            player.locked = false;
        }

        private static IntPtr vlc_lock(IntPtr opaque, ref IntPtr planes)
        {
            GCHandle handle = (GCHandle) opaque;
            VLCPlayer player = (VLCPlayer) handle.Target;
            player.locked = true;

            if (player.imageIntPtr == IntPtr.Zero)
            {
                player.imageIntPtr = Marshal.AllocHGlobal(player.width * player.height * player.channels);
            }

            planes = player.imageIntPtr;

            return player.imageIntPtr;
        }

        private static void vlc_picture(IntPtr opaque, IntPtr picture)
        {
            GCHandle handle = (GCHandle) opaque;
            VLCPlayer player = (VLCPlayer) handle.Target;
            if (!player.imageUpdate)
            {
                player.currentImage = new byte[player.width * player.height * player.channels];
                Marshal.Copy(picture, player.currentImage, 0, player.width * player.height * player.channels);
                player.imageUpdate = true;
            }
        }

        private libvlc_video_track_t? GetVideoTrack()
        {
            libvlc_video_track_t? vtrack = null;
            IntPtr p;
            int tracks = LibVLCWrapper.libvlc_media_tracks_get(media, out p);

            for (int i = 0; i < tracks; i++)
            {
                IntPtr mtrackptr = Marshal.ReadIntPtr(p, i * IntPtr.Size);
                libvlc_media_track_t mtrack = Marshal.PtrToStructure<libvlc_media_track_t>(mtrackptr);
                if (mtrack.i_type == libvlc_track_type_t.libvlc_track_video)
                {
                    vtrack = Marshal.PtrToStructure<libvlc_video_track_t>(mtrack.media);
                }
            }
            
            if (p != IntPtr.Zero)
            {
                LibVLCWrapper.libvlc_media_tracks_release(p, tracks);
            }

            return vtrack;
        }

        public void Dispose()
        {
            Debug.Log("Dispose VLCPlayer");
            cancel = true;

            if (mediaListPlayer != IntPtr.Zero)
            {
                LibVLCWrapper.libvlc_media_list_player_release(mediaListPlayer);
            }
            
            if (mediaPlayer != IntPtr.Zero)
            {
                LibVLCWrapper.libvlc_media_player_release(mediaPlayer);
            }

            if (mediaList != IntPtr.Zero)
            {
                LibVLCWrapper.libvlc_media_list_release(mediaList);
            }

            VLCEnvironment.i.ReleaseMedia(mediaUrl);

            mediaPlayer = IntPtr.Zero;
        }

        public void Play()
        {
            Debug.Log("Play");
            LibVLCWrapper.libvlc_media_list_player_set_pause(mediaListPlayer, 0);
        }

        public void Pause()
        {
            Debug.Log("Pause");
            if (!hls) 
                LibVLCWrapper.libvlc_media_list_player_set_pause(mediaListPlayer, 1);
        }

        public bool IsPaused()
        {
            if (!hls)
                return !LibVLCWrapper.libvlc_media_list_player_is_playing(mediaListPlayer);
            else
                return false;
        }

        public void SetVolume(float volume)
        {
            volume *= 100;
            Debug.Log($"SetVolume: {volume}");
            LibVLCWrapper.libvlc_audio_set_volume(mediaPlayer, (int)(volume));
        }

        public void SetTime(float timeSecs)
        {
            long timeMs = (long) (timeSecs * 1000); // vlc time: ms
            Debug.Log("SetTime: " + timeMs);
            LibVLCWrapper.libvlc_media_player_set_time(mediaPlayer, timeMs, true);
        }

        public void SetLoop(bool loop)
        {
            LibVLCWrapper.libvlc_media_list_player_set_playback_mode(mediaListPlayer,
                loop ? libvlc_playback_mode_t.libvlc_playback_mode_repeat : libvlc_playback_mode_t.libvlc_playback_mode_default);
        }

        public void SetPlaybackRate(float playbackRate)
        {
            LibVLCWrapper.libvlc_media_player_set_rate(mediaPlayer, playbackRate);
        }

        public float GetTime()
        {
            float timeSecs = LibVLCWrapper.libvlc_media_player_get_time(mediaPlayer) / 1000.0f; // secs 
            return timeSecs;
        }

        public float GetDuration()
        {
            float timeSecs = LibVLCWrapper.libvlc_media_get_duration(media) / 1000.0f; // secs 
            return timeSecs;
        }

        public libvlc_state_t GetState()
        {
            return LibVLCWrapper.libvlc_media_list_player_get_state(mediaListPlayer);
        }
    }

}