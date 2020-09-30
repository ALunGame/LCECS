using LCECS;
using LCECS.Layer.Info;
namespace Demo.Info
{
    /// <summary>
    /// 基础世界信息
    /// </summary>
    [WorldSensor(WorldInfoKey.WorldBaseInfo)]
    public class BaseWorldInfoSensor:IWorldSensor
    {
        public T GetInfo<T>(params object[] data) where T : InfoData
        {
            BaseWorldInfoData infoData=new BaseWorldInfoData();
            return infoData.As<T>();
        }
    }

    public class BaseWorldInfoData:InfoData
    {
        
    }
}