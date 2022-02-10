using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class SoundManager
{
    static GameObject g;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void init()
    {
        if (g) return;
        g = new GameObject("AudioSources");
        for (int i = 0; i < 32; i++)
            sources.Enqueue(modSource(g.AddComponent<AudioSource>()));
    }
    static Queue<AudioSource> sources = new Queue<AudioSource>(32);

    static AudioSource modSource(AudioSource source)
    {
        void setVolume(float volume)
        {
            if (volume > 1f)
                volume = volume / 100f;
            source.volume = volume;
        }
        setVolume(OptionsMenu.Volumn);
        OptionsMenu.OnVolumnChange += volume => setVolume(volume);
        return source;
    }

    public static async void Play(AudioClip clip, Vector3 position = default)
    {
        if (!clip) return;
        if (!g) init();
        if(sources.Count == 0)
            sources.Enqueue(modSource(g.AddComponent<AudioSource>()));
        var source = sources.Dequeue();
        source.clip = clip;

        if (position == default)
            source.spatialBlend = 0;
        else
        {
            source.spatialBlend = 1;

            source.transform.position = position;
        }

        source.Play();
        await Task.Delay(TimeSpan.FromSeconds(clip.length));
        sources.Enqueue(source);
    }
}