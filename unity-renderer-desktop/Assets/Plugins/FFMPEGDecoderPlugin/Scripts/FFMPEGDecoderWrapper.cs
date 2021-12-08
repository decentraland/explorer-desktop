using System;
using System.Runtime.InteropServices;

namespace HTC.UnityPlugin.Multimedia
{
    public class FFMPEGDecoderWrapper
    {
        private const string NATIVE_LIBRARY_NAME = "libffmpegdecoder";

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeCleanAll();
        //  Decoder
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern int nativeCreateDecoder(string filePath, ref int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern int nativeCreateDecoderAsync(string filePath, ref int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern int nativeGetDecoderState(int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeStartDecoding(int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeDestroyDecoder(int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeIsVideoBufferFull(int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeIsVideoBufferEmpty(int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeIsEOF(int id);
        
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeGrabVideoFrame(int id, ref IntPtr frameDataPtr, ref bool frameReady);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeReleaseVideoFrame(int id);
        
        //  Video
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeIsVideoEnabled(int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeSetVideoEnable(int id, bool isEnable);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeGetVideoFormat(int id, ref int width, ref int height, ref float totalTime);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeSetVideoTime(int id, float currentTime);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeIsContentReady(int id);

        //  Audio
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeIsAudioEnabled(int id);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeSetAudioEnable(int id, bool isEnable);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeSetAudioAllChDataEnable(int id, bool isEnable);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeGetAudioFormat(int id, ref int channel, ref int frequency, ref float totalTime);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern float nativeGetAudioData(int id, ref IntPtr output, ref int lengthPerChannel);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeFreeAudioData(int id);

        //  Seek
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void nativeSetSeekTime(int id, float sec);

        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern bool nativeIsSeekOver(int id);

        //  Utility
        [DllImport (NATIVE_LIBRARY_NAME)]
        public static extern int nativeGetMetaData(string filePath, out IntPtr key, out IntPtr value);
    }
}