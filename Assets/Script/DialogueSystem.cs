
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using System.IO;
public class DialogueSystem : MonoBehaviour
{
    public Canvas dialogueCanvas;
    public float textSpeed = 0.05f; // 文字の表示速度
    public AudioClip typingSound; // 文字を打つ音の効果音

    private string[] dialogues;
    private int currentDialogueIndex = 0;
    private bool isTyping = false;

    public MonologueSystem monologueSystem;
    private bool isNpcTalking = false;

    public TextAsset csvFile; //CSVファイル
    public static List<string[]> csvData = new List<string[]>(); //csvファイルの中身を入れるリスト

    public static int index = 1;  //現在地
    public Text LogText;     //地の文・セリフ表示

    private AudioSource audio;  //ボイス取得
    private AudioClip Sound; //ボイス出力
    private bool isPlaying = false; // 再生中の音声があるかどうかを示すフラグ


    void Start()
    {
        
        csvFile = Resources.Load("csv/Dialogue01_NPC1") as TextAsset; // テキストアセットをロードする
        StringReader reader = new StringReader(csvFile.text); // テキストリーダーを作成

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine(); // 1行読み込む
            csvData.Add(line.Split(',')); // 読み込んだ行をカンマで分割してリストに追加する
        }
        
        audio = GetComponent<AudioSource>();//音声を取得

        if (reader != null)
        {
            // index の初期化を確実にする
            index = 1;
            monologueSystem.ProcessFinished += OnNpcTalk_Flag;

        }
        
        dialogueCanvas.enabled = false;

    //     dialogues = new string[]
    //    {
    //         "ああ。",
    //         "そうだ。ああ。ようやく捕まえた。",
    //         "縛り付けてある。",
    //         "……そうだな。間違いない。",
    //         "ヤツが、……フィクサーだ。",
    //         "",
    //     };
    }



    void Update()
    {
        if(isNpcTalking&& !isTyping){
            if (Input.GetMouseButtonDown(0) && !isTyping)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    
                    if (hit.collider.CompareTag("NPC"))
                    {
                        if (index < csvData.Count)
                        {

                            Debug.Log("まだ再生するセリフがあります。");

                            //LogText.text = csvData[index][1]; // iの2番目の要素=セリフを表示する
                            StartCoroutine(Dialogue(csvData[index][1]));
                            
                            index++;
                            
                        }
                        else
                        {
                            Debug.Log("CSVのテキストをすべて表示しました。");

                            // セリフが最後まで行ったらCanvasを非表示にする
                            dialogueCanvas.enabled = false;
                            // ループしないようにisNpcTalkingをfalseに設定
                            isNpcTalking = false;
                        }
                    }
                }
            }

        }
    }



    private void OnNpcTalk_Flag(){
        isNpcTalking = true;
        Debug.Log("NPCが話し始めました。isNpcTalking: " + isNpcTalking);
    }



    IEnumerator Dialogue(string dialogue)
    {
        isTyping = true;
        dialogueCanvas.enabled = true;
        LogText.text = "";

        foreach (char letter in dialogue)
        {
            LogText.text += letter;
            if (typingSound != null)
                AudioSource.PlayClipAtPoint(typingSound, transform.position); // 文字の打つ音を再生
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }


    
    // IEnumerator PlaySound()
    // {
        
    //     isPlaying = true; // 再生中のフラグを立てる
    //     audio.PlayOneShot(Sound);
    //     yield return new WaitForSeconds(Sound.length); // 音声再生が終了するまで待つ
    //     isPlaying = false; // 再生が終了したらフラグを下げる
    // }

}
