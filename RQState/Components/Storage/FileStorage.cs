using System;
using System.Collections.Generic;
using System.IO;

namespace RQState.Components.Storage
{
    public class FileStorage<T> : IStorage<T> where T : class
    {
        private static object sync = new object();

        private const string EXTENSION = ".instance";

        private static string TypeStoragePath
        {
            get
            {
                string typeKey = typeof (T).FullName;
                return Config.DirectoryStoragePath + @"/" + typeKey;
            }
        }

        private static string FilePath(object key)
        {
            return TypeStoragePath + @"/" + key + EXTENSION;
        }


        public void Save(object key, T obj)
        {
            lock (sync)
            {
                string xml = Serializer<T>.Serialize(obj);
                if (!Directory.Exists(TypeStoragePath))
                    Directory.CreateDirectory(TypeStoragePath);
                using (StreamWriter writer = new StreamWriter(FilePath(key), false))
                {
                    writer.Write(xml);
                }
            }
        }

        private T Deserialize(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            using (StreamReader reader = new StreamReader(filePath))
            {
                return Serializer<T>.Deserialize(reader.ReadToEnd());
            }
        }

        public T Get(object key)
        {
            return Deserialize(FilePath(key));
        }

        public void Delete(object key)
        {
            File.Delete(FilePath(key));
        }


        public List<T> GetAll(Type type)
        {
            List<T> list = new List<T>();

            if (!Directory.Exists(TypeStoragePath))
                return list;

            string[] files = Directory.GetFiles(TypeStoragePath, "*" + EXTENSION);

            foreach (string file in files)
            {
                list.Add(Deserialize(file));
            }
            return list;
        }


        public void Clear()
        {
            Directory.Delete(TypeStoragePath);
        }
    }
}