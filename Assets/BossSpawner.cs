using UnityEngine;
using System.Collections.Generic;


public class BossSpawner : MonoBehaviour
{
    #region ===字段===

    [SerializeField] private BossCtr _boss;

    private Transform _playerTr;
    private Transform _myTr;
    private const float SpawnRange = 3.5f;

    #endregion

    #region ===属性===
    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Start()
    {
        _playerTr = PlayerController.Instance.transform;
        _myTr = transform;
    }

    private void Update()
    {
        if (Vector3.Distance(_playerTr.position, _myTr.position) > SpawnRange)
            return;

        _boss.ActiveBoss();
        Destroy(gameObject);
    }

    #endregion

    #region ===方法===

    #endregion


}
