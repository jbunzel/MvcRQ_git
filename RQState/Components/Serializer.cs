using System.IO;
using System.Xml.Serialization;

namespace RQState.Components
{
    public static class Serializer<T> where T : class
    {
        public static string Serialize(T obj)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof (T));
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }

        public static T Deserialize(string xml)
        {
            if(string.IsNullOrEmpty(xml))
                return null;
            XmlSerializer serializer = new XmlSerializer(typeof (T));
            StringReader reader = new StringReader(xml);
            return (T) serializer.Deserialize(reader);
        }
    }
}