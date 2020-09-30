using System;

namespace LCECS.Server.Factory
{
    public interface IFactoryServer
    {
        T1 GetProduct<T1>(FactoryType factoryType,Action<object[]> func,params object[] data) where T1 : class;
    }
}
