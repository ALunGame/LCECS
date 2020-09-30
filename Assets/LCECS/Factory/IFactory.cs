using System;

namespace LCECS.Server.Factory
{
	public interface IFactory<T> where T : class
    {
        //生产产品
        T CreateProduct(Action<object[]> func, params object[] data);
    }
}
