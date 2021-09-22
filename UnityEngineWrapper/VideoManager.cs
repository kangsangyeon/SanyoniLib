using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;

namespace SanyoniLib.UnityEngineWrapper
{

    public class VideoManager : MonoBehaviour
    {

        //Raw Image to Show Video Images [Assign from the Editor]
        public RawImage image;

        private VideoPlayer videoPlayer;
        private AudioSource audioSource;

        public void Initialize()
        {
            if (this.videoPlayer == null) this.videoPlayer = gameObject.AddComponent<VideoPlayer>();
            if (this.audioSource == null) this.audioSource = gameObject.AddComponent<AudioSource>();

            videoPlayer.playOnAwake = false;
            audioSource.playOnAwake = false;

            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }

        public IEnumerator CPrepareVideoUrl(string _url)
        {
            Debug.Log("Try preparing video.");

            videoPlayer.source = VideoSource.Url;
            // "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
            videoPlayer.url = _url;

            videoPlayer.Prepare();

            yield return new WaitUntil(() => videoPlayer.isPrepared);

            image.texture = videoPlayer.texture;

            Debug.Log("Done Preparing Video");
        }

        public void PlayVideo(bool _withAudio)
        {
            videoPlayer.Play();
            if (_withAudio == true) audioSource.Play();

        }

    }

}