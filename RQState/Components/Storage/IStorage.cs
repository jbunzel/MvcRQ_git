using System;
using System.Collections.Generic;

namespace RQState.Components.Storage
{
    public interface IStorage<T> where T : class
    {
        void Save(object key, T obj);
        T Get(object key);
        void Delete(object key);
        List<T> GetAll(Type type);
        void Clear();
       
    }
}