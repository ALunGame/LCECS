using System.Collections.Generic;

namespace LCECS.Core.ECS
{
    public sealed class Entity
    {
        //唯一Id
        private int Id;

        //实体名
        private string EntityName;

        public bool IsEnable { get; set; } = true;

        //实体决策分组
        private EntityDecGroup DecGroup = EntityDecGroup.Player;

        private Dictionary<string, BaseCom> IdComDict = new Dictionary<string, BaseCom>();

#if UNITY_EDITOR
        public List<string> Systems = new List<string>();
        //获得所有组件名
        public HashSet<string> GetAllComStr()
        {
            return new HashSet<string>(IdComDict.Keys);
        }
#endif

        //重写哈希值
        public override int GetHashCode()
        {
            return Id;
        }

        //实体名
        public string GetEntityName()
        {
            return EntityName;
        }
        
        public EntityDecGroup GetEntityDecGroup()
        {
            return DecGroup;
        }

        //获得组件
        public BaseCom GetCom(string comName)
        {
            if (!IdComDict.ContainsKey(comName))
                return null;
            return IdComDict[comName];
        }

        //获得组件
        public T GetCom<T>() where T:BaseCom
        {
            string typeName = typeof(T).FullName;
            return IdComDict[typeName] as T;
        }

        //初始化
        public void Init(int id,string entityName,EntityDecGroup group)
        {
            Id = id;
            EntityName = entityName;
            DecGroup = group;
        }

        //禁用
        public void Enable()
        {
            IsEnable = true;
            foreach (BaseCom com in IdComDict.Values)
            {
                com.EntityEnable();
            }
            ECSLocate.ECS.CheckEntityInSystem(Id);
        }

        //禁用
        public void Disable()
        {
            IsEnable = false;
            foreach (BaseCom com in IdComDict.Values)
            {
                com.EntityDisable();
            }
            ECSLocate.ECS.CheckEntityInSystem(Id);
        }

        //添加组件
        public void AddCom<T>(T com) where T:BaseCom
        {
            string typeName = typeof(T).FullName;

            if (IdComDict.ContainsKey(typeName))
                return;

            //调用函数
            if (!com.IsActive)
                com.Enable();

            //保存数据
            IdComDict.Add(typeName, com);
        }

        //添加组件
        public void AddCom(BaseCom com)
        {
            string fullName = com.GetType().FullName;
            if (IdComDict.ContainsKey(fullName))
                return;

            //调用函数
            if (!com.IsActive)
                com.Enable();

            //保存数据
            IdComDict.Add(fullName, com);
        }

        //删除组件
        public void RemoveCom<T>() where T : BaseCom
        {
            string typeName = typeof(T).Name;
            if (!IdComDict.ContainsKey(typeName))
                return;

            //调用函数
            BaseCom com = IdComDict[typeName];
            if (com.IsActive)
                com.Disable();

            //清除数据
            IdComDict.Remove(typeName);
        }

        //删除组件
        public void RemoveCom(string typeName)
        {
            if (!IdComDict.ContainsKey(typeName))
                return;

            //调用函数
            BaseCom com = IdComDict[typeName];
            if (com.IsActive)
                com.Disable();

            //清除数据
            IdComDict.Remove(typeName);
        }
    }
}
