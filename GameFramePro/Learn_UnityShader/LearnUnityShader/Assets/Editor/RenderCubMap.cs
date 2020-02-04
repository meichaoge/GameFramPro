using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class RenderCubMap : EditorWindow
{
    [MenuItem("Tools/渲染到立方体纹理")]
    private static void RenderCubMapWin()
    {
        RenderCubMap win = GetWindow<RenderCubMap>("渲染立方体纹理");
        win.minSize = new Vector2(300, 200);

        win.Show();
    }

    private Transform mCenterTrans;
    private Cubemap mCubemap;

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        mCenterTrans =(Transform) EditorGUILayout.ObjectField("中心点对象", mCenterTrans, typeof(Transform),false);
        mCubemap = (Cubemap)EditorGUILayout.ObjectField("渲染纹理", mCubemap, typeof(Transform), false);

        if (GUILayout.Button("渲染立方体纹理"))
        {
            GameObject go = new GameObject("camera");
            go.transform.position = mCenterTrans.transform.position;
            Camera camera = go.GetComponent<Camera>();
            camera.RenderToCubemap(mCubemap);
            GameObject.Destroy(go);
        }
        GUILayout.EndVertical();
    }

}
