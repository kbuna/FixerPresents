using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // オーディオソースのリスト
    private AudioSource bgmSource;
    private AudioSource seSource;

    // デフォルトの音量設定
    public float defaultBGMVolume = 1.0f;
    public float defaultSEVolume = 1.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // BGM用のAudioSourceを作成
        GameObject bgmObject = new GameObject("BGM_Source");
        bgmObject.transform.parent = transform;
        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.volume = defaultBGMVolume; // デフォルトの音量を設定

        // SE用のAudioSourceを作成
        GameObject seObject = new GameObject("SE_Source");
        seObject.transform.parent = transform;
        seSource = seObject.AddComponent<AudioSource>();
        seSource.volume = defaultSEVolume; // デフォルトの音量を設定
    }

    // BGM再生メソッド
    public void PlayBGM(AudioClip bgmClip)
    {
        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    // SE再生メソッド
    public void PlaySE(AudioClip seClip)
    {
        seSource.PlayOneShot(seClip);
    }

    // 音量調整メソッド
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume); // 0から1の範囲にクランプ
    }

    public void SetSEVolume(float volume)
    {
        seSource.volume = Mathf.Clamp01(volume); // 0から1の範囲にクランプ
    }

    // 他の必要なメソッド（一時停止、停止、ループ設定など）を追加
}
