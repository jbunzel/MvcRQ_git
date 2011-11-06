using RQState.Components.Storage;

namespace RQState.Components
{
    public class SingletonStateObject<T1, T2>
        where T1 : SingletonStateObject<T1, T2>, new()
        where T2 : IStorage<T1>, new()
    {
        private readonly IStorage<T1> _storageImp;

        public SingletonStateObject()
        {
            _storageImp = new T2();
        }

        protected IStorage<T1> StorageImp
        {
            get { return _storageImp; }
        }

        public static T1 Get()
        {
            T2 storageImp = new T2();
            return storageImp.Get(typeof (T1).FullName) ?? new T1();
        }

        public virtual void Save()
        {
            _storageImp.Save(typeof (T1).FullName, (T1) this);
        }
    }
}