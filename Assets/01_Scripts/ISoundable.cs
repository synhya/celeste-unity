
using UnityEngine;

public interface ISoundable
{
    public void PlaySound(AudioClip clip, float pitch = 1f, float startRatio = 0f);
    public void StopSound();
}

