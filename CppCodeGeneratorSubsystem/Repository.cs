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
 