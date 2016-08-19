using System;
using UnityEngine;
using System.Collections.Generic;


public class HandsomegunProperty : MonoBehaviour
{
    public vp_FPWeaponShooter Shooter;

    //2的数组，0对应lefthand anim, 1对应righthand
    public AnimationClip[] FireClips;
    public AnimationClip IdelClip;
    public AnimationClip DefenseClip;
    public AnimationClip SpecailAttackClip;
    /// <summary>
    /// Use property instead!
    /// </summary>
    private Transform _posLeft, _posRight;
    private PositionTool[] _poss;

    private LineRenderer _bulletLine;

    private LineRenderer BulletLine
    {
        get
        {
            if (_bulletLine != null)
                return _bulletLine;
            _bulletLine = GetComponent<LineRenderer>();
            return _bulletLine;
        }
    }

    private PositionTool[] Poss
    {
        get
        {
            if (_poss != null)
                return _poss;
            _poss = GetComponentsInChildren<PositionTool>();
            return _poss;
        }
    }

    private Transform PosLeft
    {
        get
        {
            if (_posLeft != null)
                return _posLeft;
            if (Poss != null)
            {
                var left = Array.Find(Poss, tool => tool.Direction == Direction.LEFT);
                if(left != null)
                    _posLeft = left.transform;
            }
            return _posLeft;
        }
    }

    private Transform PosRight
    {
        get
        {
            if (_posRight != null)
                return _posRight;
            if (Poss != null)
            {
                var right = Array.Find(Poss, tool => tool.Direction == Direction.RIGHT);
                if (right != null)
                    _posRight = right.transform;
            }
            return _posRight;
        }
    }

    private Animation _animation;
    private Animation Animation
    {
        get
        {
            if (_animation != null)
                return _animation;
            
            _animation = GetComponentInChildren<Animation>();
            _animation[DefenseClip.name].speed *= 1.2f;
            //SA:means 2 seconds(in playerController._specialAttackEffectiveTime), normal 1.6667f
            _animation[SpecailAttackClip.name].speed *= 0.5f;  
            return _animation;
        }
    }

    public void PlayAnimation(string animName, bool isCrossFade = false)
    {
        if (Animation == null) return;

        if (!isCrossFade)
        {
            Animation[animName].time = 0.0f;
            Animation.Sample();
            Animation.Play(animName); 
        }
        else
        {
            Animation.CrossFade(animName);
        }
    }

    public void StopAnimation()
    {
        PlayAnimation(IdelClip.name);
    }

    public void TryDrawLine(Vector3? hitPoint = null)
    {
        Transform startPos = Shooter.IsFireLeft ? PosLeft : PosRight;
        if (PosLeft == null || PosRight == null)
        {
            Debug.Log("Bullet start pos not found!!");
            return;
        }

        Vector3 endPos;
        if (hitPoint != null)
        {
            endPos = (Vector3) hitPoint;
        }
        else
        {
            endPos = startPos.forward*1000f;
        }
        BulletLine.SetPosition(0, startPos.position);
        BulletLine.SetPosition(1, endPos);
    }

}
