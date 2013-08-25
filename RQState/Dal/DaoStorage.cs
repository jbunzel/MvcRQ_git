using System;
using System.Collections.Generic;
using System.Data.OleDb;
using RQState.Components;
using RQState.Components.Storage;

namespace RQState.Dal
{
    public class DaoStorage<T> : IStorage<T> where T : class
    {
        private static string Key(object key)
        {
            return TypeKey + "_" + key;
        }

        private static string TypeKey
        {
            get { return typeof (T).FullName; }
        }


        private void Update(OleDbConnection connection, object key, string value)
        {
            string insertSql = "update Heap set PropertyKey = ? , PropertyValue = ?";
            OleDbCommand command = new OleDbCommand(insertSql, connection);
            command.Parameters.Add(new OleDbParameter("PropertyKey", Key(key)));
            command.Parameters.Add(new OleDbParameter("PropertyValue", value));
            command.ExecuteNonQuery();
        }

        private static void Insert(OleDbConnection connection, object key, string value)
        {
            string insertSql = "INSERT INTO Heap(PropertyKey, PropertyValue) Values( ? , ?)";
            OleDbCommand command = new OleDbCommand(insertSql, connection);
            command.Parameters.Add(new OleDbParameter("PropertyKey", Key(key)));
            command.Parameters.Add(new OleDbParameter("PropertyValue", value));
            command.ExecuteNonQuery();
        }


        private static bool IsExist(OleDbConnection connection, object key)
        {
            string insertSql = "SELECT ID FROM Heap WHERE PropertyKey = ?";
            OleDbCommand command = new OleDbCommand(insertSql, connection);
            command.Parameters.Add(new OleDbParameter("PropertyKey", Key(key)));
            return command.ExecuteScalar() == null ? false : true;
        }

        private static void Delete(OleDbConnection connection, object key)
        {
            string deleteSql = "DELETE from Heap where PropertyKey= ? ";
            OleDbCommand command = new OleDbCommand(deleteSql, connection);
            command.Parameters.Add(new OleDbParameter("PropertyKey", Key(key)));
            command.ExecuteNonQuery();
        }


        private static string Get(OleDbConnection connection, object key)
        {
            string selectSql = "Select PropertyValue from Heap where PropertyKey = ? ";
            OleDbCommand command = new OleDbCommand(selectSql, connection);
            command.Parameters.Add(new OleDbParameter("PropertyKey", Key(key)));
            return (string) command.ExecuteScalar();
        }


        private static void Clear(OleDbConnection connection)
        {
            string deleteSql = "DELETE FROM HEAP WHERE PropertyKey LIKE '?*'";
            OleDbCommand command = new OleDbCommand(deleteSql, connection);
            command.Parameters.Add(new OleDbParameter("PropertyKey", TypeKey));
            command.ExecuteNonQuery();
        }


        private static List<T> GetAll(OleDbConnection connection)
        {
            List<T> list = new List<T>();
            string selectSql = " SELECT * FROM HEAP WHERE  PropertyKey LIKE ?+'%'";
            OleDbCommand command = new OleDbCommand(selectSql, connection);
            command.Parameters.Add(new OleDbParameter("PropertyKey", TypeKey));
            using (OleDbDataReader dataReader = command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    string obj = (string) dataReader["PropertyValue"];
                    list.Add(Serializer<T>.Deserialize(obj));
                }
            }
            return list;
        }


        public void Save(object key, T obj)
        {
            string value = Serializer<T>.Serialize(obj);

            using (OleDbConnection connection = new OleDbConnection(Config.ConnectionString))
            {
                connection.Open();
                if (IsExist(connection, key))
                    Update(connection, key, value);
                else
                    Insert(connection, key, value);
            }
        }


        public T Get(object key)
        {
            using (OleDbConnection connection = new OleDbConnection(Config.ConnectionString))
            {
                connection.Open();
                string obj = Get(connection, key);
                return Serializer<T>.Deserialize(obj);
            }
        }

        public void Delete(object key)
        {
            using (OleDbConnection connection = new OleDbConnection(Config.ConnectionString))
            {
                connection.Open();
                Delete(connection, key);
            }
        }


        public List<T> GetAll(Type type)
        {
            using (OleDbConnection connection = new OleDbConnection(Config.ConnectionString))
            {
                connection.Open();
                return GetAll(connection);
            }
        }


        public void Clear()
        {
            using (OleDbConnection connection = new OleDbConnection(Config.ConnectionString))
            {
                connection.Open();
                Clear(connection);
            }
        }
    }
}