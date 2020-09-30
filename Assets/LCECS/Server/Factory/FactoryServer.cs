
using System;

namespace LCECS.Server.Factory
{
    public class FactoryServer : IFactoryServer
    {
        private EntityFactory entityFactory = new EntityFactory();
        private AssetFactory  assetFactory  = new AssetFactory();

		public T1 GetProduct<T1>(FactoryType factoryType, Action<object[]> func, params object[] data) where T1 : class
		{
			switch (factoryType)
			{
				case FactoryType.Entity:
					return entityFactory.CreateProduct(func,data) as T1;
				case FactoryType.Asset:
					return assetFactory.CreateProduct(func,data) as T1;
			}

			return null;
		}
	}
}
