using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace LCECS.Help
{
    public class EDScene
    {
        public static List<T> GetSceneObjects<T>() where T : Component
        {
            Object[] gos = Resources.FindObjectsOfTypeAll(typeof(T));



            //.Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go)) && go.hideFlags == HideFlags.None);

            List<T> resComs=new List<T>();
            for (int i = 0; i < gos.Length; i++)
            {
                if (gos[i].hideFlags==HideFlags.None)
                {
                    //Debug.LogWarning(gos[i].GetType().FullName)
                    resComs.Add(((T)gos[i]).GetComponent<T>());
                }
            }

            return resComs;
        }
    }
}
