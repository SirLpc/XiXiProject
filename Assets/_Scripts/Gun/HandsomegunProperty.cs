using UnityEngine;
using System.Collections.Generic;


public class HandsomegunProperty : MonoBehaviour
{

    //2的数组，0对应lefthand anim, 1对应righthand
    public AnimationClip[] FireClips;
    public AnimationClip IdelClip;
    public AnimationClip DefenseClip;


    private Animation _fireAnimation;
    private Animation FireAnimation
    {
        get
        {
            if (_fireAnimation != null)
                return _fireAnimation;
            
            _fireAnimation = GetComponentInChildren<Animation>();
            _fireAnimation[DefenseClip.name].speed *= 1.2f;
            return _fireAnimation;
        }
    }

    public void PlayAnimation(string animName, bool isCrossFade = false)
    {
        if (FireAnimation == null) return;

        if (!isCrossFade)
        {
            FireAnimation[animName].time = 0.0f;
            FireAnimation.Sample();
            FireAnimation.Play(animName); 
        }
        else
        {
            FireAnimation.CrossFade(animName);
        }
    }

}
