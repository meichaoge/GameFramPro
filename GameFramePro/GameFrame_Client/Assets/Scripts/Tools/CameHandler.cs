using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 辅助设置相机
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameHandler : MonoBehaviour
    {

        [SerializeField]
        private Transform mTarget;
        [SerializeField]
        private Vector2 mCanvasSize = new Vector2(640, 1136);
        [SerializeField]
        private float mDistance = 100;

        private Camera mTargetCamera;

        private void Awake()
        {
            mTargetCamera = GetComponent<Camera>();
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //设置相机垂直方向上FOv
                mTargetCamera.fieldOfView = 2 * Mathf.Atan(mCanvasSize.y * 0.5f / mDistance) * Mathf.Rad2Deg;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                StartCoroutine(ShareTool.GetCameraTextureForShare(mTargetCamera, (int)mCanvasSize.x, (int)mCanvasSize.y,null));
            }
        }

    }
}