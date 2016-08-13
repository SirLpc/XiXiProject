using UnityEngine;
using System.Collections.Generic;


public class BossAtkCheckHandler : MonoBehaviour
{
    #region ===字段===

    private BossCtr _boss;

    #endregion

    #region ===属性===
    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Awake()
    {
        _boss = transform.root.GetComponent<BossCtr>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag(Consts.PlayerTag))
            _boss.AttackSuccess();
    }

    #endregion

    #region ===方法===

    #endregion


}
