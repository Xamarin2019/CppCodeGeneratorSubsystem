using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CppCodeGeneratorSubsystem
{
    static class Repository
    {
        // Словарь доступных для генерации типов упорядоченных по именам файлов являющихся ключами
        public static FilesDictionary AvailableTypes = new FilesDictionary();
        
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
 