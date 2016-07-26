using UnityEngine;
using System.Collections;

public interface IEnemyState
{

    void UpdateState();

    void ToPatrolState();

    void ToAlertState();

    void ToChaseState();

    void ToAttackState();

    void ToSAHurtState();

}