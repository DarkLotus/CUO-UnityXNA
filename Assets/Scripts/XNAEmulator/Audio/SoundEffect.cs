using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Microsoft.Xna.Framework.Audio
{
    
    
public class DynamicSoundEffectInstance : IDisposable
{
    public DynamicSoundEffectInstance(int frequency)
    {
    }

    public UnityEngine.AudioClip Clip { get; set; }
    public float Volume { get; set; }
    public float Pitch { get; set; }

    public void Play()
    {
        GameObject gameObject = new GameObject("SoundEffectAudioClip");
        gameObject.AddComponent<AudioSource>();
        gameObject.GetComponent<AudioSource>().clip = Clip;
        gameObject.GetComponent<AudioSource>().Play();
        gameObject.AddComponent<AudioSourceController>();
        // TODO
    }

    public void Dispose()
    { }
}
    public class SoundEffect : IDisposable
    {
        public UnityEngine.AudioClip Clip { get; set; }

        public void Play()
        {
            GameObject gameObject = new GameObject("SoundEffectAudioClip");
            gameObject.AddComponent<AudioSource>();
            gameObject.GetComponent<AudioSource>().clip = Clip;
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.AddComponent<AudioSourceController>();
            // TODO
        }

        public void Dispose()
        { }
    }
}
