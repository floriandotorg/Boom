using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boom
{
	class Store
	{
//		private static Store _instance;
//
//		private static Store Instance
//		{
//			get 
//			{
//				if (_instance == null)
//				{
//					_instance = new Store();
//				}
//
//				return _instance;
//			}
//		}
//
//		private StoreBase _store;
//
//		protected Store()
//		{
//			try
//			{
//				if (Environment.OSVersion.Version.Major >= 8)
//				{
//					_store = StoreLauncher.StoreLauncher.GetStoreInterface("StoreWrapper.Store, StoreWrapper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
//				}
//			}
//			catch
//			{
//				_store = null;
//				#if DEBUG
//				throw;
//				#endif
//			}
//		}

		public static bool Available
		{
			get
			{
				return false;
				//return Instance._store != null;
			}
		}

		public static bool HasPurchased(string productID)
		{
			try
			{
				if (Available)
				{
//					if (Instance._store.LicenseInformation.ProductLicenses.Keys.Contains(productID) && Instance._store.LicenseInformation.ProductLicenses[productID].IsActive)
//					{
//						return true;
//					}
				}
			}
			catch
			{
#if DEBUG
				throw;
#else
				return false;
#endif
			}

			return false;
		}

		public static void Purchase(string productID, Action completed)
		{
			try
			{
				if (Available)
				{
//					Instance._store.RequestProductPurchaseAsync(productID, false).Completed = (IAsyncOperationBase<string> operation, StoreAsyncStatus status) =>
//					{
//						if (status == StoreAsyncStatus.Completed)
//						{
//							completed();
//						}
//					};
				}
			}
			catch
			{
				#if DEBUG
				throw;
				#endif
			}
		}
	}
}
