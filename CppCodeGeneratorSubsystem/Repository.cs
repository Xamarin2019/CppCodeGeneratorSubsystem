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
        public Element GetElement(string typeName);
        public Element GetFilenameElement(string fileName, string typeName);
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
            Repository.AvailableTypes["<string>"].AddClass("std", "string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback1", "std::string", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback4", "std::string", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback1", "my_library1::callback1", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback2", "my_library2::callback1", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback2", "my_library1::callback2", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback3", "my_library2::callback2", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback3", "my_library1::callback1", "std::string");

            return Repository;
        }

        public string GetFilename(string typeName)
        {
            return AvailableTypes.GetFilename(typeName);
        }

        public Element GetElement(string typeName)
        {
            return AvailableTypes.FindElement(typeName);
        }

        public Element GetFilenameElement(string fileName, string typeName)
        {
            return AvailableTypes.FindElement(fileName, typeName);
        }

        public void Sort()
        {

        }

    }

    // Делаем словарь поудобнее
    public class FilesDictionary : Dictionary<string, ElementList>
    {
        virtual public new ElementList this[string key]
        {
            set { base[key] = value; }

            get
            {
                if (!base.Keys.Contains(key))
                {
                    base[key] = new ElementList(this);
                }
                return base[key];
            }
        }

        public Element FindElement(string typeName)
        {
            typeName = typeName.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(typeName)) throw new FormatException("Parameter'typeName' can't be empty!");

            Element element;
            var names = typeName.Split("::");
            element = this.SelectMany(d => d.Value).FirstOrDefault(e => e.Name == names[0]);
            IEnumerable<Element> elements = this.SelectMany(d => d.Value).Where(e => e.Name == names[0]);

            if (element == null) return element;

            foreach (var name in names.Skip(1))
            {
                elements = elements.SelectMany(e => e.Nested).Where(e => e.Name == name);
                if (elements == null) return element;
            }

            return elements.LastOrDefault();
        }

        public Element FindElement(string fileName, string typeName)
        {
            fileName = fileName.Replace(" ", string.Empty); typeName = typeName.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(fileName)) throw new FormatException("Parameter'fileName' can't be empty!");
            if (string.IsNullOrEmpty(typeName)) throw new FormatException("Parameter'typeName' can't be empty!");
            if (!this.ContainsKey(fileName)) throw new NullReferenceException($"'{fileName}' not found in AvailableTypes!");

            Element element = null;
            var names = typeName.Split("::");

            var elements = this[fileName].Where(e => e.Name == names[0]);
            if (elements.Count() == 0) return null;

            element = elements.LastOrDefault();

            foreach (var item in elements)
            {
                foreach (var name in names.Skip(1))
                {
                    element = item.Nested.LastOrDefault(e => e.Name == name);
                    if (element == null) continue;
                    return element;
                }
            }

            return element;
        }

        public string GetFilename(string typeName)
        {
            typeName = typeName.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(typeName)) throw new FormatException("Parameter'typeName' can't be empty!");
            var names = typeName.Split("::");
            string fileName = this.Where(d => d.Value.Exists(e => e.Name == names[0])).FirstOrDefault().Key;

            if (fileName == null) return null;

            Element element = FindElement(fileName, typeName);

            if (element == null) return null;
         
            return fileName;
        }
    }

    public class ElementList : List<Element>
    {
        public FilesDictionary FilesDictionary { get; set; }

        public ElementList(FilesDictionary filesDictionary)
        {
            FilesDictionary = filesDictionary;
        }

        public int FindNested(Element element)
        {
            List<Element> elements1 = this;
            List<Element> elements2 = element.Nested.ToList();
 
            Element element1; Element element2;
            int index = 0;

            for (int i = 0; i < elements1.Count(); i++)
            {
                element1 = elements1[i];

                for (int j = 0; j < elements2.Count(); j++)
                { 
                    element2 = elements2[j].Copy().FirstParent();

                    if (element1.Equals(element2)) index = index > i ? index : i;
 
                }
            }

            return index;
        }

        public new void Add(Element element)
        {
            IEnumerable<Element> elements = this;
            Element elementLast = null;
            Element elementCopy = element.Copy().FirstParent();

            // Пропускаем элементы списка в которых встречаются вложенные типы нового элемента
            elements = elements.Skip(FindNested(element)).ToList();

            if (Contains(elementCopy))
            {
                return;
            }

            if (elements.Count() == 0)
            {
                base.Add(elementCopy);

                return;
            }


            // Выбираем удобное место для парковки
            while (elements.Count() != 0)
            {
 
                elementLast = elements.FirstOrDefault();

                elements = elements.Where(e => e.EqualsTypeName(elementCopy)).SelectMany(e => e.Nested).ToList();
 
                if (elements.Count() != 0)
                {
                    elementCopy = elementCopy.Nested.LastOrDefault();
                }
                if (elementCopy == null) return;
            }

            // Если корневой элемент то добавляем
            if (elementLast == null || elementLast.Parent == null)
            {
                base.Add(elementCopy);

                return;
            }
            // Если вложенный, то добавляем ко вложенному
            elementLast.Parent.AddNested(elementCopy);
        }

        public Element AddNamespace(string _namespace)
        {
            Element element = this.LastOrDefault();

            var names = _namespace.Split("::");

            if (element == null || element.Name != names[0])
            {
                element = new Namespace(names[0]);
                base.Add(element);
            }

            foreach (var name in names.Skip(1))
            {
                if (element == null || element.Name != name)
                {
                    element.AddNested(new Namespace(name));
                }
                element = element.Nested.LastOrDefault();
            }

            return element;
        }

        public void AddClass(string _namespace, string _class)
        {
            Element element;
            Element elementNested;
            if (_namespace != null)
            {
                element = AddNamespace(_namespace);
                elementNested = new Class(_class);
                element.AddNested(elementNested);
            }
            else
            {
                base.Add(new Class(_class));
            }
    
        }
        public void AddClass(string _class)
        {
            AddClass(null, _class);
        }

        public void AddStruct(string _namespace, string _struct)
        {
            Element element;
            Element elementNested;
            if (_namespace != null)
            {
                element = AddNamespace(_namespace);
                elementNested = new Struct(_struct);
                element.AddNested(elementNested);
            }
            else
            {
                base.Add(new Struct(_struct));
            }

        }
        public void AddStruct(string _struct)
        {
            AddStruct(null, _struct);
        }

        public void AddAlias(string _namespace, string _name, string returnType, string parameterType)
        {
            Element element;
            Element elementNested;
            Element elementReturnType = Find(returnType);
            Element elementParameterType = Find(parameterType);
            if (_namespace != null)
            {
                element = AddNamespace(_namespace);
                elementNested = new Alias(_name, elementReturnType, elementParameterType);
                element.AddNested(elementNested);
            }
            else
            {
                base.Add(new Alias(_name, elementReturnType, elementParameterType));
            }


        }

        public void AddAlias(string _name, string returnType, string parameterType)
        {
            AddAlias(null, _name, returnType, parameterType);
        }

        public Element Find(string qialifiedName)
        {
            Element element = FilesDictionary.FindElement(qialifiedName);
 
            return element;
        }
    }

}
 