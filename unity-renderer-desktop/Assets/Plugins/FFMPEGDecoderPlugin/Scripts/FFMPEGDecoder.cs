//========= Copyright 2015-2019, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace HTC.UnityPlugin.Multimedia
{
	public class CoroutineStarter : MonoBehaviour
	{
	}
	public class FFMPEGDecoder : IDisposable
	{
		private const string LOG_TAG = "[FFMPEGDecoder]";
		private const bool VERBOSE = true;

		bool newFrame = false;
		
		public double playbackRate = 1.0f;
		private GameObject localObject;
		private MonoBehaviour coroutineStarter;
		private const string VERSION = "1.1.7.190215";
		public string mediaPath = null;	            //	Assigned outside.
		public UnityEvent onInitComplete = new UnityEvent();    //  Initialization is asynchronized. Invoked after initialization.
		public UnityEvent onVideoEnd = new UnityEvent();        //  Invoked on video end.

		public bool loop = false;

		public enum DecoderState {
			INIT_FAIL = -2,
			STOP,
			NOT_INITIALIZED,
			INITIALIZING,
			INITIALIZED,
			START,
			PAUSE,
			SEEK_FRAME,
            BUFFERING,
            EOF
		};
		private DecoderState lastState = DecoderState.NOT_INITIALIZED;
		private DecoderState decoderState = DecoderState.NOT_INITIALIZED;
		private int decoderID = -1;

		public bool isVideoEnabled { get; private set; }
		public bool isAudioEnabled { get; private set; }
        private bool isAllAudioChEnabled = false;
        
		private bool seekPreview = false;           //  To preview first frame of seeking when seek under paused state.
		private Texture2D videoTexture = null;
		private int videoWidth = -1;
		private int videoHeight = -1;

		private const int AUDIO_FRAME_SIZE = 2048;  //  Audio clip data size. Packed from audioDataBuff.
        private const int SWAP_BUFFER_NUM = 4;		//	How many audio source to swap.
		private AudioSource[] audioSource = new AudioSource[SWAP_BUFFER_NUM];
		private List<float> audioDataBuff = null;   //  Buffer to keep audio data decoded from native.
		public int audioFrequency { get; private set; }
        public int audioChannels { get; private set; }
        private const double OVERLAP_TIME = 0.02;   //  Our audio clip is defined as: [overlay][audio data][overlap].
        private int audioOverlapLength = 0;         //  OVERLAP_TIME * audioFrequency.
        private int audioDataLength = 0;            //  (AUDIO_FRAME_SIZE + 2 * audioOverlapLength) * audioChannel.
        private float volume = 1.0f;

        //	Time control
        private double globalStartTime  = 0;        //  Video and audio progress are based on this start time.
		private bool isVideoReadyToReplay = false;
		private bool isAudioReadyToReplay = false;
		private double audioProgressTime = -1.0;
		private double hangTime = -1.0f;            //  Used to set progress time after seek/resume.
        private double firstAudioFrameTime = -1.0;
        public float videoTotalTime { get; private set; }   //  Video duration.
        public float audioTotalTime { get; private set; }   //  Audio duration.

        private BackgroundWorker backgroundWorker;
        private object _lock = new object();

        private Coroutine decoderAsyncCoroutine;
        private Coroutine audioPlayCoroutine;
        private Coroutine videoPlayCoroutine;

        private double GetOverlapTime()
        {
	        return OVERLAP_TIME * playbackRate;
        }
        public FFMPEGDecoder(string mediaPath)
        {
	        localObject = new GameObject("_VideoPlayer");
	        coroutineStarter = localObject.AddComponent<CoroutineStarter>();
	        this.mediaPath = mediaPath;
			if (VERBOSE) Debug.Log(LOG_TAG + " ver." + VERSION + " url: " + mediaPath);
			initDecoder(mediaPath);
        }

        public void UpdateVideoTexture()
        {
	        if (newFrame && videoTexture != null)
	        {
		        videoTexture.Apply();
		        newFrame = false;
	        }
        }

        //  Video progress is triggered using Update. Progress time would be set by nativeSetVideoTime.
        private void UpdateDecoder()
        {
	        switch (decoderState) {
				case DecoderState.START:
					if (isVideoEnabled) {
						
						IntPtr frameDataPtr = new IntPtr();
						bool frameReady = false;
						FFMPEGDecoderWrapper.nativeGrabVideoFrame(decoderID, ref frameDataPtr, ref frameReady);

						if (frameReady)
						{
							int size = videoWidth * videoHeight * 3;
							if (videoTexture != null)
								videoTexture.LoadRawTextureData(frameDataPtr, size);
							FFMPEGDecoderWrapper.nativeReleaseVideoFrame(decoderID);
							newFrame = true;
						}

						//	Update video frame by dspTime.
						double setTime = (AudioSettings.dspTime - globalStartTime) * playbackRate;

						//	Normal update frame.
						if (setTime < videoTotalTime || videoTotalTime == -1.0f) {
							if (seekPreview && FFMPEGDecoderWrapper.nativeIsContentReady(decoderID)) {
								setPause();
								seekPreview = false;
								unmute();
							} else {
								FFMPEGDecoderWrapper.nativeSetVideoTime(decoderID, (float) setTime);
							}
						} else {
                            if (!FFMPEGDecoderWrapper.nativeIsVideoBufferEmpty(decoderID)) {
	                            FFMPEGDecoderWrapper.nativeSetVideoTime(decoderID, (float)setTime);
                            } else {
                                isVideoReadyToReplay = true;
                            }
						}
					}

                    if (FFMPEGDecoderWrapper.nativeIsVideoBufferEmpty(decoderID) && !FFMPEGDecoderWrapper.nativeIsEOF(decoderID)) {
                        decoderState = DecoderState.BUFFERING;
                        hangTime = AudioSettings.dspTime - globalStartTime;
                    }

                    break;

				case DecoderState.SEEK_FRAME:
					if (FFMPEGDecoderWrapper.nativeIsSeekOver(decoderID)) {
						globalStartTime = AudioSettings.dspTime - hangTime;
						decoderState = DecoderState.START;
						if (lastState == DecoderState.PAUSE) {
							seekPreview = true;
							mute();
						}
					}
					break;

                case DecoderState.BUFFERING:
                    if (FFMPEGDecoderWrapper.nativeIsVideoBufferFull(decoderID) || FFMPEGDecoderWrapper.nativeIsEOF(decoderID)) {
                        decoderState = DecoderState.START;
                        globalStartTime = AudioSettings.dspTime - hangTime;
                    }
                    break;

                case DecoderState.PAUSE:
                case DecoderState.EOF:
                default:
					break;
			}

			if (isVideoEnabled || isAudioEnabled) {
				if ((!isVideoEnabled || isVideoReadyToReplay) && (!isAudioEnabled || isAllAudioChEnabled || isAudioReadyToReplay)) {
                    decoderState = DecoderState.EOF;
                    isVideoReadyToReplay = isAudioReadyToReplay = false;

                    if (onVideoEnd != null) {
						onVideoEnd.Invoke();
					}

                    if (loop)
                    {
	                    replay();
                    }
                }
			}
		}
        
        private void initDecoder(string path, bool enableAllAudioCh = false)
        {
	        isAllAudioChEnabled = enableAllAudioCh;
	        decoderAsyncCoroutine = coroutineStarter.StartCoroutine(initDecoderAsync(path));
        }

        public void Play()
        {
	        if (decoderState == DecoderState.INITIALIZED)
	        {
		        startDecoding();
	        }
	        else if (decoderState == DecoderState.INITIALIZING)
	        {
		        onInitComplete.RemoveAllListeners();
		        onInitComplete.AddListener(startDecoding);
	        }
	        else
	        {
		        setResume();
	        }
        }
        
		IEnumerator initDecoderAsync(string path) {
			if (VERBOSE) Debug.Log(LOG_TAG + " init Decoder.");
			decoderState = DecoderState.INITIALIZING;

			mediaPath = path;
			decoderID = -1;
			FFMPEGDecoderWrapper.nativeCreateDecoderAsync(mediaPath, ref decoderID);
			
			if (VERBOSE) Debug.Log($"Decoder ID {decoderID}");

			int result = 0;
			do {
				yield return null;
				result = FFMPEGDecoderWrapper.nativeGetDecoderState(decoderID);
			} while (!(result == 1 || result == -1));

			//  Init success.
			if (result == 1) {
				if (VERBOSE) Debug.Log(LOG_TAG + " Init success.");
				isVideoEnabled = FFMPEGDecoderWrapper.nativeIsVideoEnabled(decoderID);
				if (isVideoEnabled) {
					float duration = 0.0f;
					FFMPEGDecoderWrapper.nativeGetVideoFormat(decoderID, ref videoWidth, ref videoHeight, ref duration);
					videoTotalTime = duration > 0 ? duration : -1.0f ;
					if (VERBOSE) Debug.Log(LOG_TAG + " Video format: (" + videoWidth + ", " + videoHeight + ")");
					if (VERBOSE) Debug.Log(LOG_TAG + " Total time: " + videoTotalTime);
				}

				//	Initialize audio.
				isAudioEnabled = FFMPEGDecoderWrapper.nativeIsAudioEnabled(decoderID);
				if (VERBOSE) Debug.Log(LOG_TAG + " isAudioEnabled = " + isAudioEnabled);
				if (isAudioEnabled) {
                    if (isAllAudioChEnabled) {
	                    FFMPEGDecoderWrapper.nativeSetAudioAllChDataEnable(decoderID, isAllAudioChEnabled);
                        getAudioFormat();
                    } else {
                        getAudioFormat();
                        initAudioSource();
                    }
                }

				PrepareTexture();

				decoderState = DecoderState.INITIALIZED;
				if (VERBOSE) Debug.Log(LOG_TAG + "Initialized!");

				if (onInitComplete != null) {
					onInitComplete.Invoke();
				}
			} else {
				if (VERBOSE) Debug.Log(LOG_TAG + " Init fail.");
				decoderState = DecoderState.INIT_FAIL;
			}
		}

        private void getAudioFormat() {
            int channels = 0;
            int freqency = 0;
            float duration = 0.0f;
            FFMPEGDecoderWrapper.nativeGetAudioFormat(decoderID, ref channels, ref freqency, ref duration);
            audioChannels = channels;
            audioFrequency = freqency;
            audioTotalTime = duration > 0 ? duration : -1.0f;
            if (VERBOSE) Debug.Log(LOG_TAG + " audioChannel " + audioChannels);
            if (VERBOSE) Debug.Log(LOG_TAG + " audioFrequency " + audioFrequency);
            if (VERBOSE) Debug.Log(LOG_TAG + " audioTotalTime " + audioTotalTime);
        }

        private void initAudioSource() {
            getAudioFormat();
            audioOverlapLength = (int) (GetOverlapTime() * audioFrequency + 0.5f);

            audioDataLength = (AUDIO_FRAME_SIZE + 2 * audioOverlapLength) * audioChannels;
            for (int i = 0; i < SWAP_BUFFER_NUM; i++) {
                if (audioSource[i] == null) {
                    audioSource[i] = localObject.AddComponent<AudioSource>();
                }
                audioSource[i].clip = AudioClip.Create("testSound" + i, audioDataLength, audioChannels, audioFrequency, false);
                audioSource[i].playOnAwake = false;
                audioSource[i].volume = volume;
                audioSource[i].minDistance = audioSource[i].maxDistance;
                audioSource[i].spatialize = false;
                audioSource[i].SetSpatializerFloat(0, 0.0f);
            }
        }
		
		public void startDecoding() {
			if(decoderState == DecoderState.INITIALIZED) {
                if (!FFMPEGDecoderWrapper.nativeStartDecoding(decoderID)) {
	                if (VERBOSE) Debug.Log(LOG_TAG + " Decoding not start.");
					return;
				}

                decoderState = DecoderState.BUFFERING;
                globalStartTime = AudioSettings.dspTime;
                hangTime = AudioSettings.dspTime - globalStartTime;
                
                videoPlayCoroutine = coroutineStarter.StartCoroutine(videoPlay());

                isVideoReadyToReplay = isAudioReadyToReplay = false;
				if(isAudioEnabled && !isAllAudioChEnabled)
				{
					audioPlayCoroutine = coroutineStarter.StartCoroutine(audioPlay());
                    backgroundWorker = new BackgroundWorker();
                    backgroundWorker.WorkerSupportsCancellation = true;
                    backgroundWorker.DoWork += new DoWorkEventHandler(pullAudioData);
                    backgroundWorker.RunWorkerAsync();
                }
			}
		}

        private void pullAudioData(object sender, DoWorkEventArgs e) {
            IntPtr dataPtr = IntPtr.Zero;      //	Pointer to get audio data from native.
            float[] tempBuff = new float[0];    //	Buffer to copy audio data from dataPtr to audioDataBuff.
            int audioFrameLength = 0;
            double lastTime = -1.0f;            //	Avoid to schedule the same audio data set.

            audioDataBuff = new List<float>();
            while (decoderState >= DecoderState.START) {
                if (decoderState != DecoderState.SEEK_FRAME) {
                    double audioNativeTime = FFMPEGDecoderWrapper.nativeGetAudioData(decoderID, ref dataPtr, ref audioFrameLength);
                    if (0 < audioNativeTime && lastTime != audioNativeTime && decoderState != DecoderState.SEEK_FRAME && audioFrameLength != 0) {
                        if (firstAudioFrameTime == -1.0) {
                            firstAudioFrameTime = audioNativeTime;
                        }

                        lastTime = audioNativeTime;
                        audioFrameLength *= audioChannels;
                        if (tempBuff.Length != audioFrameLength) {
                            //  For dynamic audio data length, reallocate the memory if needed.
                            tempBuff = new float[audioFrameLength];
                        }
                        Marshal.Copy(dataPtr, tempBuff, 0, audioFrameLength);
                        lock (_lock) {
                            audioDataBuff.AddRange(tempBuff);
                        }
                    }

                    if (audioNativeTime != -1.0) {
	                    FFMPEGDecoderWrapper.nativeFreeAudioData(decoderID);
                    }

                    System.Threading.Thread.Sleep(2);
                }
            }
            
            lock (_lock) {
                audioDataBuff.Clear();
                audioDataBuff = null;
            }
        }

        private void ReleaseTexture() {
	        videoTexture = null;
        }

		private void PrepareTexture()
		{
			videoTexture = new Texture2D(videoWidth, videoHeight, TextureFormat.RGB24, false, false);
		}

		public Texture2D GetTexture()
		{
			return videoTexture;
		}

		public void replay() {
            if (setSeekTime(0.0f)) {
                globalStartTime = AudioSettings.dspTime;
                isVideoReadyToReplay = isAudioReadyToReplay = false;
            }
		}

        public void getAllAudioChannelData(out float[] data, out double time, out int samplesPerChannel) {
            if (!isAllAudioChEnabled) {
	            if (VERBOSE) Debug.Log(LOG_TAG + " this function only works for isAllAudioEnabled == true.");
                data = null;
                time = 0;
                samplesPerChannel = 0;
                return;
            }

            IntPtr dataPtr = new IntPtr();
            int lengthPerChannel = 0;
            double audioNativeTime = FFMPEGDecoderWrapper.nativeGetAudioData(decoderID, ref dataPtr, ref lengthPerChannel);
            float[] buff = null;
            if (lengthPerChannel > 0) {
                buff = new float[lengthPerChannel * audioChannels];
                Marshal.Copy(dataPtr, buff, 0, buff.Length);
                FFMPEGDecoderWrapper.nativeFreeAudioData(decoderID);
            }

            data = buff;
            time = audioNativeTime;
            samplesPerChannel = lengthPerChannel;
        }

        IEnumerator videoPlay()
        {
	        while (true)
	        {
		        UpdateDecoder();
		        yield return null;
	        }
        }

		IEnumerator audioPlay() {
			if (VERBOSE) Debug.Log(LOG_TAG + " start audio play coroutine.");
			int swapIndex = 0; //	Swap between audio sources.
			double audioDataTime = ((double) AUDIO_FRAME_SIZE / audioFrequency) / playbackRate;
            int playedAudioDataLength = AUDIO_FRAME_SIZE * audioChannels; //  Data length exclude the overlap length.

            if (VERBOSE) Debug.Log(LOG_TAG + " audioDataTime " + audioDataTime);

            audioProgressTime = -1.0;           //  Used to schedule each audio clip to be played.
			while(decoderState >= DecoderState.START) {
				if(decoderState == DecoderState.START) {
					double currentTime = AudioSettings.dspTime - globalStartTime;
					if(currentTime < audioTotalTime || audioTotalTime == -1.0f) {
                        if (audioDataBuff != null && audioDataBuff.Count >= audioDataLength) {
                            if (audioProgressTime == -1.0) {
                                //  To simplify, the first overlap data would not be played.
                                //  Correct the audio progress time by adding OVERLAP_TIME.
                                audioProgressTime = firstAudioFrameTime + GetOverlapTime();
                                globalStartTime = AudioSettings.dspTime - audioProgressTime;
                            }

                            while (audioSource[swapIndex].isPlaying || decoderState == DecoderState.SEEK_FRAME) { yield return null; }

                            //  Re-check data length if audioDataBuff is cleared by seek.
                            if (audioDataBuff.Count >= audioDataLength) {
                                double playTime = audioProgressTime + globalStartTime;
                                double endTime = playTime + audioDataTime;

                                //  If audio is late, adjust start time and re-calculate audio clip time.
                                if (playTime <= AudioSettings.dspTime) {
                                    globalStartTime = AudioSettings.dspTime - audioProgressTime;
                                    playTime = audioProgressTime + globalStartTime;
                                    endTime = playTime + audioDataTime;
                                }

                                audioSource[swapIndex].clip.SetData(audioDataBuff.GetRange(0, audioDataLength).ToArray(), 0);
                                audioSource[swapIndex].PlayScheduled(playTime);
                                audioSource[swapIndex].SetScheduledEndTime(endTime);
                                audioSource[swapIndex].time = (float) GetOverlapTime();
                                audioProgressTime += audioDataTime;
                                swapIndex = (swapIndex + 1) % SWAP_BUFFER_NUM;

                                lock (_lock) {
                                    audioDataBuff.RemoveRange(0, playedAudioDataLength);
                                }
                            }
                        }
					} else {
						//print(LOG_TAG + " Audio reach EOF. Prepare replay.");
						isAudioReadyToReplay = true;
						audioProgressTime = firstAudioFrameTime = - 1.0;
                        if (audioDataBuff != null) {
                            lock (_lock) {
                                audioDataBuff.Clear();
                            }
                        }
                    }
				}
				yield return new WaitForFixedUpdate();
			}
        }

		public void stopDecoding() {
			if(decoderState >= DecoderState.INITIALIZING) {
				if (VERBOSE) Debug.Log(LOG_TAG + " stop decoding.");
				decoderState = DecoderState.STOP;
				ReleaseTexture();
				
				if (videoPlayCoroutine != null)
					coroutineStarter.StopCoroutine(videoPlayCoroutine);
				
				if (isAudioEnabled && !isAllAudioChEnabled) {
					if (audioPlayCoroutine != null)
						coroutineStarter.StopCoroutine(audioPlayCoroutine);
                    backgroundWorker.CancelAsync();

                    if (audioSource != null) {
						for (int i = 0; i < SWAP_BUFFER_NUM; i++) {
							if (audioSource[i] != null) {
								AudioClip.Destroy(audioSource[i].clip);
								AudioSource.Destroy(audioSource[i]);
								audioSource[i] = null;
							}
						}
					}
				}

				FFMPEGDecoderWrapper.nativeDestroyDecoder (decoderID);
				decoderID = -1;
				decoderState = DecoderState.NOT_INITIALIZED;

				isVideoEnabled = isAudioEnabled = isAllAudioChEnabled = false;
				isVideoReadyToReplay = isAudioReadyToReplay = false;
                isAllAudioChEnabled = false;
            }
		}

		public bool setSeekTime(float seekTime) {
            if (decoderState != DecoderState.SEEK_FRAME && decoderState >= DecoderState.START) {
                lastState = decoderState;
                decoderState = DecoderState.SEEK_FRAME;

                float setTime = 0.0f;
                if ((isVideoEnabled && seekTime > videoTotalTime) ||
                    (isAudioEnabled && !isAllAudioChEnabled && seekTime > audioTotalTime) ||
                    isVideoReadyToReplay || isAudioReadyToReplay ||
                    seekTime < 0.0f) {
	                if (VERBOSE) Debug.Log(LOG_TAG + " Seek over end. ");
                    setTime = 0.0f;
                } else {
                    setTime = seekTime;
                }
                
                if (VERBOSE) Debug.Log(LOG_TAG + " set seek time: " + setTime);
                hangTime = setTime;
                FFMPEGDecoderWrapper.nativeSetSeekTime(decoderID, setTime);
                FFMPEGDecoderWrapper.nativeSetVideoTime(decoderID, (float) setTime);

                if (isAudioEnabled && !isAllAudioChEnabled) {
                    lock (_lock) {
	                    if (audioDataBuff != null)
							audioDataBuff.Clear();
                    }
                    audioProgressTime = firstAudioFrameTime = -1.0;
                    foreach (AudioSource src in audioSource) {
                        src.Stop();
                    }
                }

                return true;
            } else {
                return false;
            }
		}
		
		public bool isSeeking() {
			return decoderState >= DecoderState.INITIALIZED && (decoderState == DecoderState.SEEK_FRAME || !FFMPEGDecoderWrapper.nativeIsContentReady(decoderID));
		}

        public bool isVideoEOF() {
            return decoderState == DecoderState.EOF;
        }

		public void setStepForward(float sec) {
			double targetTime = AudioSettings.dspTime - globalStartTime + sec;
            if (setSeekTime((float) targetTime)) {
	            if (VERBOSE) Debug.Log(LOG_TAG + " set forward : " + sec);
            }
		}

		public void setStepBackward(float sec) {
			double targetTime = AudioSettings.dspTime - globalStartTime - sec;
            if (setSeekTime((float) targetTime)) {
	            if (VERBOSE) Debug.Log(LOG_TAG + " set backward : " + sec);
            }
		}

		public void getVideoResolution(ref int width, ref int height) {
			width = videoWidth;
			height = videoHeight;
		}

		public int getVideoWidth()
		{
			return videoWidth;
		}

		public int getVideoHeight()
		{
			return videoHeight;
		}

		public float getVideoCurrentTime() {
			if (decoderState == DecoderState.INITIALIZED || decoderState == DecoderState.INITIALIZING ||
			    decoderState == DecoderState.NOT_INITIALIZED)
			{
				return 0.0f;
			}
			if (decoderState == DecoderState.PAUSE || decoderState == DecoderState.SEEK_FRAME) {
				return (float) hangTime;
			} else {
				return (float) (AudioSettings.dspTime - globalStartTime);
			}
		}

		public DecoderState getDecoderState() {
			return decoderState;
		}

		public void setPause() {
			if (decoderState == DecoderState.START) {
				hangTime = AudioSettings.dspTime - globalStartTime;
				decoderState = DecoderState.PAUSE;
				if (isAudioEnabled && !isAllAudioChEnabled) {
					foreach (AudioSource src in audioSource) {
						src.Pause();
					}
				}
			}
		}

		public void setResume() {
			if (decoderState == DecoderState.PAUSE) {
				globalStartTime = AudioSettings.dspTime - hangTime;
				decoderState = DecoderState.START;
				if (isAudioEnabled && !isAllAudioChEnabled) {
					foreach (AudioSource src in audioSource) {
						src.UnPause();
					}
				}
			}
		}

		public void setVolume(float vol) {
			volume = Mathf.Clamp(vol, 0.0f, 1.0f);
			foreach (AudioSource src in audioSource) {
				if(src != null) {
					src.volume = volume;
				}
			}
		}

		public float getVolume() {
			return volume;
		}

		public void mute() {
			float temp = volume;
			setVolume(0.0f);
			volume = temp;
		}

		public void unmute() {
			setVolume(volume);
		}

		public static void getMetaData(string filePath, out string[] key, out string[] value) {
			IntPtr keyptr = IntPtr.Zero;
			IntPtr valptr = IntPtr.Zero;

			int metaCount = FFMPEGDecoderWrapper.nativeGetMetaData(filePath, out keyptr, out valptr);

			IntPtr[] keys = new IntPtr[metaCount];
			IntPtr[] vals = new IntPtr[metaCount];
			Marshal.Copy(keyptr, keys, 0, metaCount);
			Marshal.Copy(valptr, vals, 0, metaCount);

			string[] keyArray = new string[metaCount];
			string[] valArray = new string[metaCount];
			for (int i = 0; i < metaCount; i++) {
				keyArray[i] = Marshal.PtrToStringAnsi(keys[i]);
				valArray[i] = Marshal.PtrToStringAnsi(vals[i]);
				Marshal.FreeCoTaskMem(keys[i]);
				Marshal.FreeCoTaskMem(vals[i]);
			}
			Marshal.FreeCoTaskMem(keyptr);
			Marshal.FreeCoTaskMem(valptr);

			key = keyArray;
			value = valArray;
		}

		public void setAudioEnable(bool isEnable) {
			FFMPEGDecoderWrapper.nativeSetAudioEnable(decoderID, isEnable);
            if (isEnable) {
                setSeekTime(getVideoCurrentTime());
            }
        }

        public void setVideoEnable(bool isEnable) {
	        FFMPEGDecoderWrapper.nativeSetVideoEnable(decoderID, isEnable);
            if (isEnable) {
                setSeekTime(getVideoCurrentTime());
            }
        }

        public void Dispose()
		{
			if (VERBOSE) Debug.Log(LOG_TAG + " Dispose");
			stopDecoding();
			coroutineStarter.StopCoroutine(decoderAsyncCoroutine);
			GameObject.Destroy(localObject);
		}
	}
}