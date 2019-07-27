using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 自动导入Tag和Layer 设置  (随包一起打包可以确保导入自己设置的Layer和Tag)  貌似必须打包成package 才会执行
    /// </summary>
    public class AutoImportTagOrLayer : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string s in importedAssets)
            {
                if (s.Equals("Assets/NewBehaviourScript.cs"))
                {
                    //          LayerAndTagManager. AddTag("UICacheItemMark");
                    //增加一个叫ruoruo的layer
                    LayerAndTagManager.AddLayer("UnVisualLayer");  //Layer 目前不行需要处理 TODO
                    return;
                }
            }
        }



    }
}