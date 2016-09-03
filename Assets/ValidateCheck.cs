using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class ValidateCheck : MonoBehaviour
{
    private const string path = "http://115.28.87.227/UnityFiles/zfhdvrlock.txt";

    private void Start()
    {
        StartCoroutine(CoLoadValidate());
    }

    private IEnumerator CoLoadValidate()
    {
        WWW w = new WWW(path);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            if(w.text.Equals("1"))
                SceneManager.LoadScene(1);
            else 
                Application.Quit();
        }
        else 
            Application.Quit();
    } 

}
