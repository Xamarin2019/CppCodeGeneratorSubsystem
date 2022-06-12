using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CppCodeGeneratorSubsystem
{
    public interface IRepository
    {
        public string[] IncludeOnlyTypes { get; set; }
        public FilesDictionary AvailableTypes { get; set; }
        public string GetFilename(string declaration);
        public Element GetType(string typeName);
        public Element GetFilenameType(string fileName, string typeName);
        public void Sort();
    }

    public class Repository : IRepository
    {
        // Типы для включения
        public string[] IncludeOnlyTypes { get; set; } = new string[] { "std::string" };

        // Список пространств имен
        public string[] Namespaces = new string[] { "std::string" };

        // Словарь доступных для генерации типов упорядоченных по именам файлов являющихся ключами
        public virtual FilesDictionary AvailableTypes { get; set; } = new FilesDictionary();

        public static Repository GetRepository()
        {
            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].Add(new Class("std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library1::callback1", "std::string", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library2::callback1", "my_library1::callback1", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library1::callback2", "my_library2::callback1", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library2::callback2", "my_library1::callback2", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library1::callback3", "my_library2::callback2", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library2::callback3", "my_library1::callback3", "std::string"));

            return Repository;
        }

        public string GetFilename(string typeName)
        {
            typeName = typeName.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(typeName)) throw new FormatException("Parameter'typeName' can't be empty!");
            var fileName = AvailableTypes.Where(d => d.Value.Exists(e => e.QualifiedName == typeName)).FirstOrDefault().Key;
            return fileName;
        }

        public Element GetType(string typeName)
        {
            typeName = typeName.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(typeName)) throw new FormatException("Parameter'typeName' can't be empty!");
            Element element = AvailableTypes.SelectMany(d => d.Value).FirstOrDefault(e => e.QualifiedName == typeName);
            return element;
        }

        public Element GetFilenameType(string fileName, string typeName)
        {
            fileName = fileName.Replace(" ", string.Empty); typeName = typeName.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(fileName)) throw new FormatException("Parameter'fileName' can't be empty!");
            if (string.IsNullOrEmpty(typeName)) throw new FormatException("Parameter'typeName' can't be empty!");
            if (!AvailableTypes.ContainsKey(fileName)) throw new NullReferenceException($"'{fileName}' not found in AvailableTypes!");
            Element element = AvailableTypes[fileName].FirstOrDefault(e => e.QualifiedName == typeName);
            return element;
        }

        public void Sort()
        {

        }

    }

    // Делаем словарь поудобнее
    public class FilesDictionary : Dictionary<string, List<Element>>
    {
        virtual public new List<Element> this[string key]
        {
            set { base[key] = value; }

            get
            {
                if (!base.Keys.Contains(key))
                {
                    base[key] = new List<Element>();
                }
                return base[key];
            }
        }
    }
}
 