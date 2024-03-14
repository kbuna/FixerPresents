using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneLoader : MonoBehaviour
{
    // ボタンがクリックされたときに呼ばれるメソッド
    public void LoadNextScene()
    {
        // 非同期でシーンを読み込む
        StartCoroutine(LoadSceneAsync("GameScene"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // シーンを非同期で読み込む
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 読み込みが終わるまで待機
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
