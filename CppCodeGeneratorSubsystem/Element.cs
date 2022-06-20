using System;
using System.Collections.Generic;
using System.Linq;

namespace CppCodeGeneratorSubsystem
{
    public abstract class Element
    {
        public string Name { get; protected set; }
        public string QualifiedName { get; private set; }
        public Element Parent { get; set; }

        private List<Element> nested = new List<Element>();
        public IReadOnlyList<Element> Nested => nested;
 
        public Element(string name, params Element[] nestedElements)
        {
            // Удалим лишние пробелы, на всякий случай
            name = name.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(name)) throw new FormatException("Parameter'name' can't be empty!");
            QualifiedName = Name = name;

            foreach (var element in nestedElements)
            {
                element.Parent = this;

                nested.Add(element);
            }
        }

        public void AddNested(Element element)
        {
            if (!(this is Alias))
            {
                element.Parent = this;
                element.QualifiedName = QualifiedName + "::" + element.QualifiedName;
            }
            nested.Add(element);
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

            if (Nested.Count == item.Nested.Count)
            {
                for (int i = 0; i < Nested.Count; i++) if (!Nested[i].Equals(item.Nested[i])) return false;
            }
            else
            {
                return false;
            }

            //if (Parent != null) result = Parent.Equals(item.Parent);
            //if (result == false) return result;

            //if (Parent == null) result = item.Parent == null;
            //if (result == false) return result;
 
            return result;
        }

        public bool EqualsTypeName(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            var item = obj as Element;

            bool result = Name.Equals(item.Name);
            
            return result;
        }

        public override int GetHashCode()
        {
            return (QualifiedName + string.Join(",", Nested.Select(n => n.Name))).GetHashCode();
        }

        public override string ToString()
        {
            string nestedTostring = Nested.Count != 0 ? Environment.NewLine + "    " + string.Join("    ", Nested.Select(n => n.ToString())) : "...";
            return Environment.NewLine + $"{{{nestedTostring}}}" + Environment.NewLine + Environment.NewLine;
        }

        Element CopyThis()
        {
            Element element = this, elementNew;
            switch (element)
            {
                case Namespace Namespace:
                    elementNew = new Namespace(Namespace.Name);
                    break;

                case Class Class when (Class is Class):
                    elementNew = new Class(Class.Name);
                    break;

                case Struct Struct when (Struct is Struct):
                    elementNew = new Struct(Struct.Name);
                    break;

                case Alias Alias:
                    //elementNew = new Alias(Alias.Name, Alias.Nested[0], Alias.Nested[1]);
                    elementNew = new Alias(Alias.Name);
                    break;

                default:
                    elementNew = null;
                    break;
            }

            return elementNew;
        }

        Element CopyParent()
        {
            Element element = this, elementNew;
 
             elementNew = CopyThis();
                    

            if (elementNew != null)
            {
                if (element.Parent != null) element.Parent.CopyThis().AddNested(elementNew);
             }

            return elementNew;
        }

        Element CopyNested(Element elementNew)
        {
            Element element = this;
 
                foreach (var item in element.nested)
                {
                    elementNew.AddNested(item.Copy());
                }

            return elementNew;
        }

        public Element Copy()
        {
            return CopyNested(CopyParent());
        }

        public Element FirstParent()
        {
            Element element = this;
            while (element.Parent != null) element = element.Parent;
            return element;
        }
        //public static Element operator +(Element a, Element b) { a.nested.AddRange(b.Nested); return a; }

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

        public Class(string name) : base(name)
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
            return $"{Template}class {Name}{base.ToString()};" + Environment.NewLine;
        }
    }

    public class Struct : Class
    {
        public Struct(string name) : base(name)
        {

        }
        public override string ToString()
        {
            return $"{Template}struct {Name};" + Environment.NewLine;
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
        {   if (returnType != null && parameterType != null)
            {
                AddNested(returnType);
                AddNested(parameterType);
            }
            else
            {

            }
        }

        public Alias(string name) : base(name)
        {

        }

        public override string ToString()
        {   
            if (Nested.Count > 1) 
                return $"using {Name} = {Nested[0]?.QualifiedName ?? "Unknown"}* (*)({Nested[1]?.QualifiedName ?? "Unknown"});" + Environment.NewLine;
            if (Nested.Count > 0)
                return $"using {Name} = {Nested[0]?.QualifiedName ?? "Unknown"}* (*)(Unknown);" + Environment.NewLine;
            else
                return $"using {Name} = Unknown* (*)(Unknown);" + Environment.NewLine;
        }
    }
}
