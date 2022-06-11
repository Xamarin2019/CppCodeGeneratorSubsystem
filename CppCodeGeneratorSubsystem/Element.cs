using System;
using System.Collections.Generic;
using System.Linq;

namespace CppCodeGeneratorSubsystem
{
    public abstract class Element
    {
        public string Namespace { get; set; } = "";
        public string Name { get; set; }
        public string QualifiedName => string.IsNullOrEmpty(Namespace) ? Name + template : Namespace + "::" + Name + template;
        string template = "";
        public string Template
        {
            get
            {
                // Если есть шаблон, формируем
                if (!string.IsNullOrEmpty(template))
                    template = "tempalate <" + string.Join(", ", template.Trim('<', '>').Split(",").Select(t => "typename " + t).ToArray()) + "> ";
                return template;
            }

            set { template = value; }
        }
        public List<Element> NestedTypes { get; set; } = new List<Element>();

        public Element(string name, params string[] nestedNames)
        {
            // Удалим лишние пробелы, на всякий случай
            name = name.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(name)) throw new FormatException("Parameter'name' can't be empty!");

            // Если есть простарнство имен, сохраняем его отдельно
            var findNamespace = name.Split("::", 2);
            if (findNamespace.Length > 1)
            {
                Namespace = findNamespace[0];
                // Имя типа без пространства имен
                Name = findNamespace[1];
            }
            else
            {
                Name = name;
            }

            // Если есть шаблон, сохраняем его отдельно
            var findTemlate = Name.IndexOf("<");
            if (findTemlate > 0)
            {
                Template = Name.Substring(findTemlate);
                Name = Name.Remove(findTemlate);
            }

            foreach (var nestedName in nestedNames)
            {
                NestedTypes.Add(new Class(nestedName));
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            var item = obj as Element;

            bool result = Namespace.Equals(item.Namespace)
                       && Name.Equals(item.Name)
                       && Template.Equals(item.Template)
                       && QualifiedName.Equals(item.QualifiedName);

            if (NestedTypes.Count == item.NestedTypes.Count)
            {
                for (int i = 0; i < NestedTypes.Count; i++) if (NestedTypes[i] != item.NestedTypes[i]) result = false;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return (QualifiedName + string.Join(",", NestedTypes)).GetHashCode();
        }
    }

    public class Class : Element
    {
        public Class(string name) : base(name)
        {

        }

        public override string ToString()
        {
            return $"{Template}class {Name};";
        }
    }

    public class Struct : Element
    {
        public Struct(string name) : base(name)
        {

        }
        public override string ToString()
        {
            return $"{Template}struct {Name};";
        }

    }

    public class Alias : Element
    {
        /// <summary>
        /// This type forms an alias of the pointer to the function, which returns the returnType and takes the rameterType
        /// </summary>
        /// <param name="name">Alias name</param>
        /// <param name="returnType">function returns type</param>
        /// <param name="parameterType">function parameter</param>
        /// <example>
        /// <code>
        /// using callback = my_class* (*)(std::string);
        /// </code>
        /// </example>
        public Alias(string name, string returnType, string parameterType) : base(name, returnType, parameterType)
        {

        }

        public override string ToString()
        {
            return $"using {Name} = {NestedTypes[0].QualifiedName ?? "No data"}* (*)({NestedTypes[1].QualifiedName ?? "No data"});";
        }
    }
}
