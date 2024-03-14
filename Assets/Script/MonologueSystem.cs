using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI; // 追加(UIを使用するために必要)
using UnityEngine.EventSystems; // 追加（UI上の動作稼働）



public class MonologueSystem : MonoBehaviour
{
    public TextAsset csvFile; //CSVファイル
    public static List<string[]> csvData = new List<string[]>(); //csvファイルの中身を入れるリスト
    public static int index = 1;  //現在地
    public Text NameText;     //名前を表示
    public Text LogText;     //地の文・セリフ表示
    //public Image LeftImg; //左の立ち絵
    //public Image RightImg;  //右の立ち絵
    private AudioSource audio;  //ボイス取得
    private AudioClip Sound; //ボイス出力
    private bool isPlaying = false; // 再生中の音声があるかどうかを示すフラグ

    public GameObject TalkFrame;

    // イベント宣言
    public delegate void OnProcessFinished();
    public event OnProcessFinished ProcessFinished;
    
    private bool hasProcessed = false; // フラグを追加して処理の完了状態を追跡する



    void Start()
    {
        csvFile = Resources.Load("csv/Monologue01") as TextAsset; // テキストアセットをロードする
        StringReader reader = new StringReader(csvFile.text); // テキストリーダーを作成

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine(); // 1行読み込む
            csvData.Add(line.Split(',')); // 読み込んだ行をカンマで分割してリストに追加する
        }
        audio = GetComponent<AudioSource>();//音声を取得

        TalkFrame.SetActive (false);
    }

    void Update()
    {
        // ボタンを押したときは画面クリック無効
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!hasProcessed && Input.GetMouseButtonDown(0)) // マウス左ボタンがクリックされたら
        {
            TalkFrame.SetActive (true);

            if (isPlaying) // 再生中の音声がある場合
            {
                audio.Stop(); // 再生中の音声を停止
                isPlaying = false; // 再生中のフラグを下げる
            }

            if (index < csvData.Count)//セリフの残りがあり
            {
                NameText.text = csvData[index][0]; // iの0番目の要素＝名前を表示する
                LogText.text = csvData[index][1]; // iの2番目の要素=セリフを表示する
                //LeftImg.sprite = Resources.Load<Sprite>(csvData[i][3]);　　//太郎の立ち絵に相当
                //RightImg.sprite = Resources.Load<Sprite>(csvData[i][4]);　 //花子の立ち絵に相当

                //iの3番目＝音声データの所在を取得
                Sound = (AudioClip)Resources.Load(csvData[index][2]);
                if(Sound != null)
                {
                    StartCoroutine(PlaySound());
                }
                index++; // インデックスをインクリメントする


            }else
            {
                // CSVファイルの最後まで表示した後の処理をここに追加
                Debug.Log("CSVのテキストをすべて表示しました。");
                TalkFrame.SetActive (false);
                // 処理が終了したらイベントを発生させる
                if (ProcessFinished != null)
                {
                    ProcessFinished();
                    hasProcessed = true; // 処理が完了したのでフラグをセット
                }

            }
            
            
        }
    }
    IEnumerator PlaySound()
    {
        
        isPlaying = true; // 再生中のフラグを立てる
        audio.PlayOneShot(Sound);
        yield return new WaitForSeconds(Sound.length); // 音声再生が終了するまで待つ
        isPlaying = false; // 再生が終了したらフラグを下げる
    }

}