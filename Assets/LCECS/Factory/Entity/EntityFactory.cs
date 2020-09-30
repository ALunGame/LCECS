using LCECS.Core.ECS;
using LCECS.Data;
using LCECS.Help;

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LCECS.Server.Factory
{
    public class EntityFactory : IFactory<Entity>
    {
        public Entity CreateProduct(Action<object[]> func, params object[] data)
        {
			//解析参数
            int entityId         = int.Parse(data[0].ToString());
            string entityName    = data[1].ToString();
			GameObject entityGo  = data.Length>2?data[2] as GameObject:null;

			//配置数据
			EntityJson entityData = ECSLocate.Config.GetEntityData(entityName);
            if (entityData==null)
            {
                ECSLocate.ECSLog.LogError("实体配置数据不存在>>>>>>>", entityName);
                return null;
            }

            //创建实体节点
            if (entityGo == null)
            {
                entityGo = (GameObject)ECSLocate.Factory.GetProduct<Object>(FactoryType.Asset, null, entityData.PrefabPath);
                entityGo = Object.Instantiate(entityGo);
            }
            if (entityGo == null)
            {
                ECSLocate.ECSLog.LogR("有一个实体没有节点>>>>>>>>", entityId, entityName);
            }
            else
            {
#if UNITY_EDITOR
                    EntityEditorViewHelp sceneHelp = entityGo.AddComponent<EntityEditorViewHelp>();
                    sceneHelp.EntityId = entityId;
#endif
            }

			//创建实体
			Entity entity = new Entity();

			//添加组件
			for (int i = 0; i < entityData.Coms.Count; i++)
            {
                EntityComJson comJson = entityData.Coms[i];
                BaseCom com  = ReflectHelp.CreateInstanceByType<BaseCom>(comJson.ComName);

                //赋值
                for (int j = 0; j < comJson.Values.Count; j++)
                {
                    EntityComValueJson valueJson = comJson.Values[j];

                    object value = ReflectHelp.StrChangeToObject(valueJson.Value, valueJson.Type);
                    ReflectHelp.SetTypeFieldValue(com, valueJson.Name, value);
                }

                com.Init(entityId,entityGo);
                entity.AddCom(com);
            }

            func?.Invoke(new object[] { entityGo });

            ECSLocate.ECSLog.LogR("创建实体成功>>>>>>>>", entityName);
            return entity;
        }
    }
}
