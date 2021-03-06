﻿using UnityEngine;
using UnityEngine.UI;

public class WeaponUIView : MonoBehaviour
{
	#region ===字段===

    [SerializeField] private Text bulleText;
	[SerializeField]
	private Image saFiller, defFiller;

	private GameObject saGo, defGo;

    #endregion

    #region ===属性===

    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

	void Awake()
	{
		saGo = saFiller.transform.parent.gameObject;
		defGo = defFiller.transform.parent.gameObject;
	}

    private void Update()
    {
        bulleText.text = string.Format("{0}/∞", PlayerController.Instance.EventHandler.CurrentWeaponAmmoCount.Get());

		var saPer = PlayerController.Instance.GetSACdPercent ();
		var defPer = PlayerController.Instance.GetDefCdPercent ();
		if (saPer >= 1) 
		{
			if (saGo.gameObject.activeSelf)
				saGo.gameObject.SetActive (false);
		} 
		else
		{
			if (!saGo.gameObject.activeSelf)
				saGo.gameObject.SetActive (true);
			saFiller.fillAmount = saPer;
		}
		if (defPer >= 1) 
		{
			if (defGo.gameObject.activeSelf)
				defGo.gameObject.SetActive (false);
		} 
		else 
		{
			if (!defGo.gameObject.activeSelf)
				defGo.gameObject.SetActive (true);
			defFiller.fillAmount = defPer;
		}
    }

    #endregion

    #region ===方法===

    #endregion
}
