using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.UI;
using GameFramePro;

public class TestAudioClip : MonoBehaviour
{
    public string mAudioClipPath;
    public bool mIsReleaseClicp = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AudioManager.S_Instance.PlayBackgroundAudioSync(mAudioClipPath,0.5f,true,true);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            AudioManager.S_Instance.StopBackgroundAudio(mIsReleaseClicp);
        }
    }

}
