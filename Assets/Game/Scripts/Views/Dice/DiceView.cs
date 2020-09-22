using UnityEngine;
using System.Collections;
using System;

public class DiceView : MonoBehaviour {

    public Animator AnimatorController;
    Action m_endRollCallback;

    public void SetAction(Action endRollCallback)
    {
        m_endRollCallback = endRollCallback;

        AnimationClip clip = AnimatorController.runtimeAnimatorController.animationClips[0];

        AnimationEvent evnt = new AnimationEvent();
        evnt.time = clip.events[0].time;
        evnt.functionName = "RollAnimationEnded";
        clip.AddEvent(evnt);
    }

    public void RollAnimationEnded()
    {
        if(m_endRollCallback != null)
            m_endRollCallback();
    }
}
