using UnityEngine;
using System.Collections.Generic;


public class BossCtr : MonoBehaviour
{
	#region ===字段===

    private BossState _state;

	#endregion

	#region ===属性===
	#endregion

	#region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

	#endregion

	#region ===方法===

    public void ActiveBoss()
    {
        if (_state != BossState.INACTIVE)
            return;


    }

	#endregion
}
