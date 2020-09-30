using LCECS.Core.Tree.Base;
using LCECS.Core.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace LCECS.Data
{
    /// <summary>
    /// 实体数据流
    /// </summary>
    public class EntityWorkData : NodeData
    {
        public Entity MEntity { get; }

        //请求参数
        private Dictionary<EntityReqId, ParamData> ReqParamData = new Dictionary<EntityReqId, ParamData>();
        //需要清除Id
        public int ClearReqId;
        //当前请求Id
        public int CurrReqId;
        //下一个请求Id
        public int NextReqId;
        
        //获取请求参数
        public ParamData GetReqParam(EntityReqId reqId)
        {
            if(!ReqParamData.ContainsKey(reqId))
            {
                ReqParamData.Add(reqId, new ParamData());
            }
            return ReqParamData[reqId];
        }

        public EntityWorkData(int id,Entity entity):base(id)
        {
            MEntity = entity;
        }
    }

    /// <summary>
    /// 世界数据流
    /// </summary>
    public class WorldWorkData : NodeData
    {
        //请求参数
        private Dictionary<EntityReqId, ParamData> ReqParamData = new Dictionary<EntityReqId, ParamData>();

        //需要清除Id
        public int ClearReqId;
        //当前请求Id
        public int CurrReqId;
        //下一个请求Id
        public int NextReqId;

        //获取请求参数
        public ParamData GetReqParam(EntityReqId reqId)
        {
            if (!ReqParamData.ContainsKey(reqId))
            {
                ReqParamData.Add(reqId, new ParamData());
            }
            return ReqParamData[reqId];
        }

        public WorldWorkData(int id) : base(id)
        {
        }
    }

    /// <summary>
    /// 参数数据
    /// </summary>
    public class ParamData
    {
        private bool boolData = false;
        private int intData = 0;
        private float floatData = 0;
        private double doubleData = 0;
        private string stringData = "";

        private Vector2 vect2Data = Vector2.zero;
        private Vector2Int vect2IntData = Vector2Int.zero;
        //现在不需要（应该也不需要）
        //private GameObject goData;

        #region Set

        public void SetBool(bool value)
        {
            boolData = value;
        }

        public void SetInt(int value)
        {
            intData = value;
        }

        public void SetFloat(float value)
        {
            floatData = value;
        }

        public void SetDouble(double value)
        {
            doubleData = value;
        }

        public void SetString(string value)
        {
            stringData = value;
        }

        public void SetVect2(Vector2 value)
        {
            vect2Data = value;
        }

        public void SetVect2Int(Vector2Int value)
        {
            vect2IntData = value;
        }

        #endregion

        #region Get   取一次值，就清空

        public bool GetBool()
        {
            bool tmp = boolData;
            boolData = false;
            return tmp;
        }

        public int GetInt()
        {
            int tmp = intData;
            intData = 0;
            return tmp;
        }

        public float GetFloat()
        {
            float tmp = floatData;
            floatData = 0;
            return tmp;
        }

        public double GetDouble()
        {
            double tmp = doubleData;
            doubleData = 0;
            return tmp;
        }

        public string GetString()
        {
            string tmp = stringData;
            stringData = "";
            return tmp;
        }

        public Vector2 GetVect2()
        {
            Vector2 tmp = vect2Data;
            vect2Data = Vector2.zero;
            return tmp;
        }

        public Vector2Int GetVect2Int()
        {
            Vector2Int tmp = vect2IntData;
            vect2IntData = Vector2Int.zero;
            return tmp;
        }

        #endregion
    }
}
