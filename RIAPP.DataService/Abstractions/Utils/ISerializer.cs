using System;
using System.IO;
using System.Threading.Tasks;

namespace RIAPP.DataService.Utils
{
    /// <summary>
    /// Интерфейс сериализатора
    /// </summary>
    public interface ISerializer
    {
        string Serialize(object obj);

        Task SerializeAsync<T>(T obj, Stream stream);

        object DeSerialize(string input, Type targetType);
    }
}