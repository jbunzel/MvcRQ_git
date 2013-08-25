using System;
using System.Collections.Generic;
using RQState.Components.Storage;

namespace RQState.Components
{
    public abstract class
        StateObject<T1, T2>
        where T1 : StateObject<T1, T2>
        where T2 : IStorage<T1>, new()
    {
        private object _id;

        private readonly IStorage<T1> _storageImp;

        public object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        protected StateObject()
        {
            _id = Guid.NewGuid();
            _storageImp = new T2();
        }
  
        public virtual void Save()
        {
            _storageImp.Save(_id, (T1) this);
        }

        public static T1 Get(object key)
        {
            T2 storageImp = new T2();
            return storageImp.Get(key);
        }

        public static List<T1> GetAll()
        {

            T2 storageImp = new T2();
            return storageImp.GetAll(typeof(T1));

        }

        public static void Delete(object ID)
        {
            T2 storageImp = new T2();
            storageImp.Delete(ID);
        }

        public static void Clear()
        {
            T2 storageImp = new T2();
            storageImp.Clear();

        }
    }
}