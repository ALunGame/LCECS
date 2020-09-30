using System;
using System.Collections.Generic;

namespace LCECS.Data
{
    public class WeightJson
    {
        public int Key;
        public int Weight;
    }
    
    public class ReqWeightJson
    {
        public List<WeightJson> EntityReqWeight = new List<WeightJson>();
        public List<WeightJson> WorldReqWeight  = new List<WeightJson>();
    }
}