using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 本地化多语言配置窗口
    /// </summary>
    public class LocalizationConfig_Win : EditorWindow
    {
        [MenuItem("Tools/Localization/多语言配置窗口")]
        static void OpenLocalizationConfigWin()
        {
            LocalizationConfig_Win win = EditorWindow.GetWindow<LocalizationConfig_Win>("多语言本地化配置");
            win.minSize = new Vector2(400, 600);
            win.Show();
              win.InitialedLocalizationConfigWindow();

        }


        private void InitialedLocalizationConfigWindow()
        {
            mLocalizationConfigExcelPath = Application.dataPath;
        }

        #region Data
        private string mLocalizationConfigExcelPath;  //需要解析的本地化Excel配置路径
        #endregion


        private void OnGUI()
        {
            try
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical("box");

                #region Excel 配置信息
                GUILayout.Box("12345");
                GUILayout.BeginVertical();
                GUILayout.Label("231231");
                GUILayout.Label("231231");
                GUILayout.Label("231231");
                GUILayout.Label("231231");
                GUILayout.Label("231231");
                GUILayout.EndVertical();
                GUILayout.Label("231231");
                GUILayout.Label("231231");
                GUILayout.Label("231231");
                GUILayout.Label("231231");
                GUILayout.Label("231231");

                //   GUILayout.EndArea();
                #endregion


                GUILayout.EndVertical();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                Close();
                throw;
            }
         
        }



    }
}