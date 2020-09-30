using System.Collections.Generic;

namespace LCECS.Data
{
    public class SystemSortJson
    {
        public List<SortJson> UpdateList = new List<SortJson>();
        public List<SortJson> FixedUpdateList = new List<SortJson>();
    }

    public class SortJson
    {
        public string TypeName = "";
        public int Sort = 9999;
    }
}