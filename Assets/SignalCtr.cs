﻿using UnityEngine;
using System.Collections;

public class SignalCtr : MonoBehaviour
{
    [SerializeField]
    private Transform _pointer;
    [SerializeField]
    private GameObject _bossDoor;

    [SerializeField]
    private StatePatternEnemy[] _enemyGroup1, _enemyGroup2, _enemyGroup3;

    private void Awake()
    {
        _bossDoor.SetActive(false);
    }
	
	private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);

            if (!IsGroupAlive(_enemyGroup2))
            {
                _pointer.rotation = Quaternion.Euler(270, 0, 0);
            }

            if (!IsGroupAlive(_enemyGroup1) && !IsGroupAlive(_enemyGroup2) && !IsGroupAlive(_enemyGroup3))
                _bossDoor.SetActive(true);
        }
    }

    private bool IsGroupAlive(StatePatternEnemy[] group)
    {
        var alive = false;
        for (int i = 0; i < group.Length; i++)
            if (group[i].IsAlive)
            {
                alive = true;
            }
        return alive;
    }
}