using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum StopAudioMode
{
    ClipPause,
    ClipStop,
    ClipMute,
    ClipClear,
}

public enum RemoveAudioMode
{
    RemoveAudioSource,
    RemoveGameObject,
}

public class AudioManager
{
    private static AudioManager instance = new AudioManager();
    public static AudioManager Instance => instance;
    
    private AudioManager() { }

    /// <summary>
    /// 设置(播放)音乐
    /// </summary>
    /// <param name="clipName">音乐切片名称(地址)</param>
    /// <param name="obj">要加音乐的物体</param>
    /// <param name="callback">回调函数</param>
    /// <param name="open3D">是否开启3D音效</param>
    /// <param name="rolloffMode">3D音效衰减模式</param>
    public void SetAudio(string path, string clipName, GameObject obj, UnityAction<AudioSource> callback = null, bool open3D = false, AudioRolloffMode rolloffMode = AudioRolloffMode.Linear)
    {
        if(clipName == null || clipName.Length == 0) return;
        
        if(obj.GetComponent<AudioSource>() == null) obj.AddComponent<AudioSource>();
        AudioSource audioSource = obj.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        ResourcesLoadManager.Instance.LoadSync<AudioClip>(path, clipName, (clip) =>
        {
            audioSource.clip = clip;
            audioSource.Play();
            audioSource.spatialBlend = open3D ? 1 : 0;
            audioSource.rolloffMode = rolloffMode;
            callback?.Invoke(audioSource);
        });
    }
    
    /// <summary>
    /// 设置音频
    /// </summary>
    /// <param name="clip">音频切片</param>
    /// <param name="obj">音源物体</param>
    /// <param name="callback">回调函数</param>
    /// <param name="open3D">是否开启3D音效</param>
    /// <param name="rolloffMode">3D音效衰减模式</param>
    public void SetAudio(AudioClip clip, GameObject obj, UnityAction<AudioSource> callback = null, bool open3D = false, AudioRolloffMode rolloffMode = AudioRolloffMode.Linear)
    {
        if(clip == null) return;

        if (obj != null)
        {
            AudioSource audioSource = obj.GetComponent<AudioSource>();

            if(audioSource == null) audioSource = obj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            audioSource.clip = clip;
            audioSource.Play();
            audioSource.spatialBlend = open3D ? 1 : 0;
            audioSource.rolloffMode = rolloffMode;
            callback?.Invoke(audioSource);
        }
        else
        {
            ObjectsPool.Instance.GetEmptyObjectFromPool("AudioClipPlayer", null, (poolObj) =>
            {
                AudioSource audioSource = poolObj.GetComponent<AudioSource>();

                if(audioSource == null) audioSource = poolObj.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;

                audioSource.clip = clip;
                audioSource.Play();
                audioSource.spatialBlend = open3D ? 1 : 0;
                audioSource.rolloffMode = rolloffMode;
                callback?.Invoke(audioSource);

                ObjectsPool.Instance.ReturnObjectToPool(poolObj, clip.length);
            });
        }
    }

    /// <summary>
    /// 移除(停止)音乐
    /// </summary>
    /// <param name="audioSource">音源组件</param>
    /// <param name="mode">移除(停止)模式</param>
    /// <param name="callback">回调函数</param>
    public void RemoveAudio(AudioSource audioSource, StopAudioMode mode = StopAudioMode.ClipClear, UnityAction<AudioSource> callback = null)
    {
        if (mode == StopAudioMode.ClipPause)
        {
            audioSource.Pause();
        }
        else if (mode == StopAudioMode.ClipStop)
        {
            audioSource.Stop();
        }
        else if (mode == StopAudioMode.ClipMute)
        {
            audioSource.mute = true;
        }
        else if (mode == StopAudioMode.ClipClear)
        {
            audioSource.clip = null;
        }
        
        callback?.Invoke(audioSource);
        
    }
    
    //无参回调函数重载
    public void RemoveAudio(AudioSource audioSource, RemoveAudioMode mode = RemoveAudioMode.RemoveAudioSource, UnityAction callback = null)
    {
        
        if (mode == RemoveAudioMode.RemoveAudioSource)
        {
            GameObject.Destroy(audioSource);
        }
        else
        {
            GameObject.Destroy(audioSource.gameObject);
        }
        
        callback?.Invoke();
        
    }
}
