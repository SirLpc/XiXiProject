using UnityEngine;
using UnityEngine.UI;

public class WeaponUIView : MonoBehaviour
{
	#region ===字段===

    [SerializeField] private Text bulleText;

    #endregion

    #region ===属性===

    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Update()
    {
        bulleText.text = string.Format("{0}/∞", PlayerController.Instance.EventHandler.CurrentWeaponAmmoCount.Get());
    }

    #endregion

    #region ===方法===

    #endregion
}
