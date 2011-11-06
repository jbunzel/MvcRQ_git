using System;
using System.Collections;
using System.Collections.Generic;

namespace RQState.Components.Storage
{
    public abstract class HashStorage<T> : IStorage<T> where T : class
    {
        protected abstract Hashtable TypeStorage { set; get; }

        public void Save(object key, T obj)
        {
            if (TypeStorage == null)
                TypeStorage = new Hashtable();
            TypeStorage[key] = obj;
        }

        public T Get(object key)
        {
            if (TypeStorage == null)
                return null;
            return (T) TypeStorage[key];
        }

        public void Delete(object key)
        {
            if (TypeStorage == null)
                return;
            TypeStorage.Remove(key);
        }


        public List<T> GetAll(Type type)
        {
            List<T> list = new List<T>();
            if (TypeStorage == null)
                return list;
            foreach (T obj in TypeStorage.Values)
            {
                list.Add(obj);
            }
            return list;
        }


        public void Clear()
        {
            TypeStorage = null;
        }
    }
}