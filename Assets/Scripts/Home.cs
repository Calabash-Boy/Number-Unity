using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Home : MonoBehaviour
{
    private int count = 0;
    private static string key_count = "key_count";

    private void Start()
    {
        count = GameDataManager.GetInt(key_count);
    }

    public void GoMainGame()
    {
        //播放音效
        AudioSource source = GetComponent<AudioSource>();
        AudioClip newClip = Resources.Load<AudioClip>("Music/enter_game");
        source.clip = newClip;
        source.Play();
        count += 1;
        GameDataManager.SetInt(key_count, count);
        print(count);
        //SceneManager.LoadScene(1);
    }
}
