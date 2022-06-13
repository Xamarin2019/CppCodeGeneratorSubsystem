using System;
using System.Collections.Generic;
using System.Linq;

namespace CppCodeGeneratorSubsystem
{
    public abstract class Element
    {
        public string Name { get; set; }
        public string QualifiedName => Parent == null ? Name : Parent.Name + "::" + Name;
        public Element Parent { get; set; }
        public List<Element> Nested { get; set; } = new List<Element>();

        public Element(string name, params Element[] nested)
        {
            // Удалим лишние пробелы, на всякий случай
            name = name.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(name)) throw new FormatException("Parameter'name' can't be empty!");
            Name = name;

            foreach (var element in nested)
            {
                element.Parent = this;

                Nested.Add(element);
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            var item = obj as Element;

            bool result = Name.Equals(item.Name)
                          && QualifiedName.Equals(item.QualifiedName);
            if (result == false) return result;

            if (Parent != null) result = Parent.Equals(item.Parent);
            if (result == false) return result;

            if (Parent == null) result = item.Parent == null;
            if (result == false) return result;

            if (Nested.Count == item.Nested.Count)
            {
                for (int i = 0; i < Nested.Count; i++) if (!Nested[i].Equals(item.Nested[i])) return false;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return (QualifiedName + string.Join(",", Nested.Select(n => n.Name))).GetHashCode();
        }

        public override string ToString()
        {
            string nestedTostring = Nested.Count != 0 ? Environment.NewLine + "    " + string.Join("    ", Nested.Select(n => n.ToString())) + Environment.NewLine : "...";
            return $"{{{nestedTostring}}}" + Environment.NewLine;
        }

        public static Element operator +(Element a, Element b) { a.Nested.AddRange(b.Nested); return a; }
    }

    public class Namespace : Element
    {
        public Namespace(string name, params Element[] nested) : base(name, nested)
        {
            
        }
        public override string ToString()
        {
            return $"namespace {Name}" + base.ToString();
        }
    }

    public class Class : Element
    {
        public new string QualifiedName => base.QualifiedName + template;
        string template = "";
        public string Template
        {
            get
            {
                // Если есть шаблон, формируем
                if (!string.IsNullOrEmpty(template))
                    return "tempalate <" + string.Join(", ", template.Trim('<', '>').Split(",").Select(t => "typename " + t).ToArray()) + "> ";
                return template;
            }

            set { template = value; }
        }

        public Class(string name, params Element[] nested) : base(name, nested)
        {
            // Если есть шаблон, сохраняем его отдельно
            var findTemlate = Name.IndexOf("<");
            if (findTemlate > 0)
            {
                Template = Name.Substring(findTemlate);
                Name = Name.Remove(findTemlate);
            }
        }

        public override string ToString()
        {
            return $"{Template}class {Name}{base.ToString()};";
        }
    }

    public class Struct : Class
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
        public Alias(string name, Element returnType, Element parameterType) : base(name)
        {
            Nested.Add(returnType);
            Nested.Add(parameterType);
        }

        public override string ToString()
        {
            return $"using {Name} = {Nested[0]?.QualifiedName ?? "No data"}* (*)({Nested[1]?.QualifiedName ?? "No data"});";
        }
    }
}
