using UnityEngine;
using System.Collections.Generic;


public class BossSpawner : MonoBehaviour
{
    #region ===字段===

    [SerializeField] private BossCtr _boss;

    #endregion

    #region ===属性===
    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Consts.PlayerTag))
            return;

        _boss.ActiveBoss();
        Destroy(gameObject);
    }

    #endregion

    #region ===方法===

    #endregion


}
