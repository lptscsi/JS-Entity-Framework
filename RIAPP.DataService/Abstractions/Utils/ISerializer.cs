using System;
using System.IO;
using System.Threading.Tasks;

namespace RIAPP.DataService.Utils
{
    /// <summary>
    /// Интерфейс для сериализатора данных
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Сериализует объект в строку
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Serialize(object obj);

        /// <summary>
        /// Сериализует объект в строку асинхронно
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task SerializeAsync<T>(T obj, Stream stream);

        /// <summary>
        /// Десериализует строку в объект
        /// </summary>
        /// <param name="input"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        object DeSerialize(string input, Type targetType);
    }
}