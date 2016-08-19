using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region ===字段===

    [SerializeField]
    private int _levelIndex;

    private Transform _playerTr;
    private Transform _myTr;
    private const float SpawnRange = 3.5f;

    private bool _isLoading;

    #endregion

    #region ===属性===
    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Start()
    {
        _playerTr = PlayerController.Instance.transform;
        _myTr = transform;
        _isLoading = false;
    }

    private void Update()
    {
        if (Vector3.Distance(_playerTr.position, _myTr.position) > SpawnRange)
            return;

        LoadScene();
    }

    #endregion

    #region ===方法===

    private void LoadScene()
    {
        if (_isLoading)
            return;
        _isLoading = true;

        UIViewCtr.Instance.FadeOut(() => SceneManager.LoadScene(_levelIndex));
    }

    #endregion
}

