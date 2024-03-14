using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider seSlider;
    public Text bgmVolumeText;
    public Text seVolumeText;

    void Start()
    {
        // Sliderの値が変更されたときに対応するメソッドを呼び出すイベントハンドラーを設定
        bgmSlider.onValueChanged.AddListener(delegate { SetBGMVolume(); });
        seSlider.onValueChanged.AddListener(delegate { SetSEVolume(); });
    
    }

    // BGMの音量を調節するメソッド
    public void SetBGMVolume()
    {
        float volume = bgmSlider.value;
        AudioManager.instance.SetBGMVolume(volume);
        bgmVolumeText.text = "BGM Volume: " + Mathf.RoundToInt(volume * 100) + "%";
    }

    // SEの音量を調節するメソッド
    public void SetSEVolume()
    {
        float volume = seSlider.value;
        AudioManager.instance.SetSEVolume(volume);
        seVolumeText.text = "SE Volume: " + Mathf.RoundToInt(volume * 100) + "%";
    }
}
