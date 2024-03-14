using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudioClips : MonoBehaviour
{
    
    public AudioClip bgmClip1;
    public AudioClip bgmClip2;
    public AudioClip seClip1;
    public AudioClip seClip2;

    // Start is called before the first frame update
    void Start()
    {        // BGMの音量を半分に設定
        AudioManager.instance.SetBGMVolume(0.1f);
        // ゲーム開始時にBGMを再生する例
        AudioManager.instance.PlayBGM(bgmClip1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
