using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CppCodeGeneratorSubsystem
{
    public class Repository
    {
        // Типы для включения
        public string[] IncludeOnlyTypes = new string[] { "std::string" };

        // Словарь доступных для генерации типов упорядоченных по именам файлов являющихся ключами
        public FilesDictionary AvailableTypes = new FilesDictionary();

        public static Repository GetRepository()
        {
            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].Add(new Element(Format.Class, "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library1::callback1", "std::string", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library2::callback1", "my_library1::callback1", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library1::callback2", "my_library2::callback1", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library2::callback2", "my_library1::callback2", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library1::callback3", "my_library2::callback2", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library2::callback3", "my_library1::callback3", "std::string"));

            return Repository;
        }

    }

    // Делаем словарь поудобнее
    public class FilesDictionary : Dictionary<string, List<Element>>
    {
        public List<Element> Value { set; get; } = new List<Element>();

        public new List<Element> this[string key]
        {
            set { base[key] = Value; }

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
 