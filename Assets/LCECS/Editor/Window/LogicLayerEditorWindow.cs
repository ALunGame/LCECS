using System;
using System.Collections.Generic;
using System.Reflection;
using LCECS.Data;
using LCECS.Help;
using UnityEditor;
using UnityEngine;

namespace LCECS.Window
{
    public class LogicLayerEditorWindow : EditorWindow
    {
        private static ReqWeightJson MReqWeightJson=new ReqWeightJson();
        private const string JsonPath   =  "/"+ECSDefinitionPath.LogicReqWeightPath;
        
        private bool ShowEntityReq=true;
        private Vector2 reqPos;
        
        private ReqWeightJson ShowWeightJson=new ReqWeightJson();

        [MenuItem("LCECS/请求权重设置")]
        private static void ShowWindow()
        {
            var window = GetWindow<LogicLayerEditorWindow>();
            window.titleContent = new GUIContent("请求权重设置");
            window.Show();
            
            if (EDTool.CheckFileInPath(Application.dataPath+JsonPath))
            { 
                string dataJson = EDTool.ReadText(Application.dataPath+JsonPath);
                MReqWeightJson  = LitJson.JsonMapper.ToObject<ReqWeightJson>(dataJson);
                ClearJson();
            }
        }

        private void OnGUI()
        {
            EDLayout.CreateVertical("box",position.width, position.height, () =>
            {
                //编辑界面切换
                EDLayout.CreateHorizontal("box",position.width,20,(() =>
                {
                    EDButton.CreateBtn("实体请求",100,20,(() =>
                    {
                        SaveReqJson();
                        ShowEntityReq = true;
                    }));
                    
                    EDButton.CreateBtn("世界请求",100,20,(() =>
                    {
                        SaveReqJson();
                        ShowEntityReq = false;
                    }));
                }));

                //界面刷新
                EDLayout.CreateHorizontal("box", position.width, position.height-20, (DrawReqList));
            });
        }

        private void OnDestroy()
        {
            SaveReqJson();
        }

        private void DrawReqList()
        {
            EDLayout.CreateScrollView(ref reqPos,"box",position.width,position.height-20,(() =>
            {
                List<int> showList = GetWeightList();
                for (int i = 0; i < showList.Count; i++)
                {
                    int key = showList[i];
                    string reqName = GetReqIdName(key);

                    WeightJson json = GetReqWeightJson(key);
                    object value = json.Weight;
                    EDTypeField.CreateTypeField(reqName,ref value,typeof(int),position.width,20);
                    json.Weight = (int) value;
                    
                    EDButton.CreateBtn("强制置换请求权重",200,25,(() => { json.Weight = ECSDefinition.REForceSwithWeight; }));
                    
                    EDButton.CreateBtn("请求自身判断置换请求权重",200,25,(() => { json.Weight = ECSDefinition.RESwithRuleSelf; }));
                    
                    EDTypeField.CreateLableField("------------------------","",position.width,10);
                    EditorGUILayout.Space();
                }
            }));
        }

        private List<int> GetWeightList()
        {
            List<int> allList=new List<int>();
            if (ShowEntityReq)
            {
                foreach (int temp in Enum.GetValues(typeof(EntityReqId)))
                {
                    allList.Add(temp);
                }
            }
            else
            {
                foreach (int temp in Enum.GetValues(typeof(WorldReqId)))
                {
                    allList.Add(temp);
                }
            }
            return allList;
        }

        private string GetReqIdName(int reqId)
        {
            if (ShowEntityReq)
            {
                EntityReqId req = (EntityReqId) reqId;
                return req.ToString();
            }
            else
            {
                WorldReqId req = (WorldReqId) reqId;
                return req.ToString();
            }
        }

        private WeightJson GetReqWeightJson(int reqId)
        {
            List<WeightJson> jsons = ShowEntityReq ? MReqWeightJson.EntityReqWeight : MReqWeightJson.WorldReqWeight;
            for (int i = 0; i < jsons.Count; i++)
            {
                WeightJson json = jsons[i];
                if (json.Key==reqId)
                {
                    return json;
                }
            }
            
            WeightJson addJson=new WeightJson();
            addJson.Key = reqId;
            addJson.Weight = 0;
            jsons.Add(addJson);
            return addJson;
        }

        //清理Json
        private static void ClearJson()
        {
            //实体请求
            for (int i = 0; i < MReqWeightJson.EntityReqWeight.Count; i++)
            {
                WeightJson json = MReqWeightJson.EntityReqWeight[i];

                bool contain = true;
                foreach (int temp in Enum.GetValues(typeof(EntityReqId)))
                {
                    if (json.Key==temp)
                    {
                        contain = true;
                        break;
                    }

                    contain = false;
                }

                if (contain==false)
                {
                    MReqWeightJson.EntityReqWeight.RemoveAt(i);
                }
            }
            
            //世界请求
            for (int i = 0; i < MReqWeightJson.WorldReqWeight.Count; i++)
            {
                WeightJson json = MReqWeightJson.WorldReqWeight[i];

                bool contain = true;
                foreach (int temp in Enum.GetValues(typeof(WorldReqId)))
                {
                    if (json.Key==temp)
                    {
                        contain = true;
                        break;
                    }

                    contain = false;
                }

                if (contain==false)
                {
                    MReqWeightJson.WorldReqWeight.RemoveAt(i);
                }
            }
        }
        
        private void SaveReqJson()
        {
            string jsonData = LitJson.JsonMapper.ToJson(MReqWeightJson);
            EDTool.WriteText(jsonData,Application.dataPath+JsonPath);
            AssetDatabase.Refresh();
        }
    }
}