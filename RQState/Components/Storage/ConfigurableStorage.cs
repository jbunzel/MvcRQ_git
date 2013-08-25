using System;
using System.Collections.Generic;
using RQState.Dal;

namespace RQState.Components.Storage
{
    public class ConfigurableStorage<T> : IStorage<T> where T : class
    {
        private readonly IStorage<T> _storage = ResolveStorage();

        private static IStorage<T> ResolveStorage()
        {
            switch (Config.StorageType)
            {
                case StorageType.AppicationStorage:
                    return new ApplicationStorage<T>();
                case StorageType.SessionStorage:
                    return new SessionStorage<T>();
                case StorageType.FileStorage:
                    return new FileStorage<T>();
                case StorageType.DaoStorage:
                    return new DaoStorage<T>();
                case StorageType.CookieStorage:
                    return new CookieStorage<T>();
                default:
                    throw new ApplicationException("StorageType");
            }
        }

        public void Save(object key, T obj)
        {
            _storage.Save(key, obj);
        }

        public T Get(object key)
        {
            return _storage.Get(key);
        }

        public void Delete(object key)
        {
            _storage.Delete(key);
        }

        public List<T> GetAll(Type type)
        {
            return _storage.GetAll(type);
        }

        public void Clear()
        {
            _storage.Clear();
        }
    }
}