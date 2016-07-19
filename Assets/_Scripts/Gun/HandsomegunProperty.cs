using UnityEngine;
using System.Collections.Generic;


public class HandsomegunProperty : MonoBehaviour
{

    //2的数组，0对应lefthand anim, 1对应righthand
    public AnimationClip[] FireClips;
    public AnimationClip IdelClip;
    public AnimationClip DefenseClip;
    public AnimationClip SpecailAttackClip;


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
            _animation[SpecailAttackClip.name].speed *= 1.2f;  
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

}
