using System;
using System.Collections.Generic;
using LitJson;

namespace LCECS.Data
{
    public class EntityJsonList
    {
        public List<EntityJson> List=new List<EntityJson>();
    }
    
    public class EntityJson
    {
        public string EntityName="";
        public string TipStr="";
        public EntityDecGroup Group = EntityDecGroup.Player;
        public string PrefabPath="";
        public List<EntityComJson> Coms = new List<EntityComJson>();
    }

    public class EntityComJson
    {
        public string ComName="";
        public List<EntityComValueJson> Values = new List<EntityComValueJson>();
    }

    public class EntityComValueJson
    {
        public string Name = "";
        public string Type="";
        public string Value="";
    }
}