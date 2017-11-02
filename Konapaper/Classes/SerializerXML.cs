using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public static class SerializerXML
{
    /// <summary>
    /// Determines whether the generated XML should be indented or not 
    /// </summary>
    public static bool Indent = true;

    /// <summary>
    /// Determines whether the generated XML should omit the XML declaration or not 
    /// </summary>
    public static bool OmitXmlDeclaration;

    /// <summary>
    /// Serializes a given object into the desired file
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <param name="file">The file where the object will be serialized</param>
    public static void Serialize<T>(T obj, string file) =>
        File.WriteAllText(file, SerializeToString(obj), Encoding.UTF8);

    /// <summary>
    /// Serializes a given object into a string
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The serialized object</returns>
    public static string SerializeToString<T>(T obj)
    {
        var settings = new XmlWriterSettings { Indent = Indent, OmitXmlDeclaration = OmitXmlDeclaration };

        using (var ms = new MemoryStream())
        using (var sr = new StreamReader(ms))
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = XmlWriter.Create(ms, settings))
                serializer.Serialize(writer, obj);

            ms.Position = 0;
            return sr.ReadToEnd();
        }
    }

    /// <summary>
    /// Deserializes an object from the given file
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize</typeparam>
    /// <param name="file">The file from where the object will be deserialized</param>
    /// <returns>The deserialized object</returns>
    public static T Deserialize<T>(string file) =>
        DeserializeFromString<T>(File.ReadAllText(file, Encoding.UTF8));

    /// <summary>
    /// Deserializes an object from a given XML string
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize</typeparam>
    /// <param name="xml">The serialized object</param>
    /// <returns>The deserialized object</returns>
    public static T DeserializeFromString<T>(string xml)
    {
        using (var ms = new MemoryStream())
        {
            byte[] data = Encoding.UTF8.GetBytes(xml);
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            var deserializer = new XmlSerializer(typeof(T));
            return (T)deserializer.Deserialize(ms);
        }
    }
}