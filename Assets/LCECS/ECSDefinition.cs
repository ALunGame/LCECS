namespace LCECS
{
    /// <summary>
    /// ECS默认路径定义
    /// </summary>
    public static class ECSDefinitionPath
    {
        //实体配置路径
        public const string EntityJsonPath   =  "Resources/EntityJson.txt";

        //逻辑层请求权重路径
        public const string LogicReqWeightPath =  "Resources/ReqWeight.txt";

        //系统排序
        public const string SystemSortPath = "Resources/SystemSort.txt";

        //决策树路径
        public const string DecTreePath   =  "Resources/DecTree.txt";
        //决行为树路径
        public const string BevTreePath   =  "Resources/BevTree.txt";

        //技能配置路径
        public const string SkillJsonPath = "Resources/SkillJson.txt";
    }
    
    /// <summary>
    /// ECS各种定义
    /// </summary>
    public static class ECSDefinition
    {
        //只调用请求实例置换规则
        public const int RESwithRuleSelf = -9;
        //强制置换请求权重
        public const int REForceSwithWeight = -99;
    }

    /// <summary>
    /// 工厂类别
    /// </summary>
    public enum FactoryType
    {
        Entity,                 //实体
        Asset,                  //资源
    }

    /// <summary>
    /// 世界信息键
    /// </summary>
    public enum WorldInfoKey
    {
        PlayerInfo,                  //玩家信息
        WorldBaseInfo,                    //基础信息
        MapInfo,                     //地图信息
    }

    /// <summary>
    /// 实体信息键
    /// </summary>
    public enum EntityInfoKey
    {
        NearEntity,                 //最近的实体
        NearEntityList,             //距离一定值的实体列表

        NearMonster,                //最近的怪物
        NearMonsterList,            //距离一定值的怪物列表
    }

    /// <summary>
    /// 实体决策组
    /// </summary>
    public enum EntityDecGroup
    {
        Player,
        Enemy,
        Boss,
        NPC,
    }

    /// <summary>
    /// 世界决策组
    /// </summary>
    public enum WorldDecGroup
    {
        Out,                        //表世界
        In,                         //里世界
    }

    /// <summary>
    /// 实体请求Id
    /// </summary>
    public enum EntityReqId
    {
        None,
        PlayerMove,                                     //玩家移动
        PlayerNormalAttack,                             //玩家普通攻击

        EnemySeekPlayer,                    //敌人寻路玩家
        EnemyWander,                        //敌人徘徊
        EnemyEnemy,                        //敌人攻击
    }

    /// <summary>
    /// 世界请求Id
    /// </summary>
    public enum WorldReqId
    {
        Change,                             //改变世界
    }
}
