using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestQuaternion : MonoBehaviour
{
    public Transform mTarget;
    public Transform mLoolAtTarget;
    public Vector3 mtargetforward;
    public bool mIsLookAt = true;
    public Vector3 mtargetup;


    private void Update()
    {
        mtargetforward = mTarget.forward;
        mtargetup = mTarget.up;


        if (mtargetforward != null && mIsLookAt)
        {
            Quaternion result =Quaternion.LookRotation(mLoolAtTarget.position - mTarget.position, mTarget.up);
            mTarget.rotation = Quaternion.Lerp(   mTarget.rotation , result,0.2f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(mTarget.rotation);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Quaternion result = Quaternion.FromToRotation(mTarget.forward, mTarget.up);
            Debug.Log(result);
            mTarget.rotation = result;
            UnityEngine.Debug.Log("12");
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            Quaternion result = Quaternion.Euler(45, 0, 0);
            Debug.Log(result);
            mTarget.rotation = result;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Quaternion result = Quaternion.Inverse(Quaternion.Euler(45, 0, 0));
            Debug.Log(result);
            mTarget.rotation = result;
        }
    }
}
