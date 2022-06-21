﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CppCodeGeneratorSubsystem
{
    public abstract class Element
    {
        protected string bareName;
        public string Name { get; protected set; }
        public string QualifiedName { get; protected set; }
 
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

            set
            {
                template = value;
            }
        }
        public Element Parent { get; set; }

        private List<Element> nested = new List<Element>();
        public IReadOnlyList<Element> Nested => nested;
 
        public Element(string name, params Element[] nestedElements)
        {
            // Удалим лишние пробелы, на всякий случай
            name = name.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(name)) throw new FormatException("Parameter'name' can't be empty!");

            Name = bareName = name;

            // Если есть шаблон, сохраняем его отдельно
            var findTemlate = Name.IndexOf("<");
            if (findTemlate > 0)
            {
                Template = Name.Substring(findTemlate);
                bareName = Name.Remove(findTemlate);
            }
            QualifiedName = Name + template;

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
            string nestedTostring = Nested.Count != 0 ? Environment.NewLine + "{" + Environment.NewLine + "    " + string.Join("    ", Nested.Select(n => n.ToString())) + "}" + Environment.NewLine : "";
            return $"{nestedTostring}";
        }

        Element CopyThis()
        {
            Element element = this, elementNew;
            switch (element)
            {
                case Namespace Namespace:
                    elementNew = new Namespace(Namespace.Name);
                    break;
 
                case Struct Struct when (Struct is Struct):
                    elementNew = new Struct(Struct.Name);
                    break;

                case Class Class when (Class is Class):
                    elementNew = new Class(Class.Name);
                    break;

                case Alias Alias:
                    //elementNew = new Alias(Alias.Name, Alias.Nested[0], Alias.Nested[1]);
                    elementNew = new Alias(Alias.Name);
                    break;

                case Enumeration Enum:
                    elementNew = new Enumeration(Enum.Name);
                    break;

                default:
                    elementNew = null;
                    break;
            }

            return elementNew;
        }

        Element CopyParent()
        {
            Element element = this, elementNew, elementTmp, elementParent;
 
             elementNew = elementTmp = CopyThis();
                    

            if (elementNew != null)
            {
                while (element.Parent != null)
                {
                    elementParent = element.Parent.CopyThis();
                    elementParent.AddNested(elementTmp);
                    elementTmp = elementParent;
                    element = element.Parent;
                }
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
            return $"namespace {bareName}" + base.ToString();
        }
    }

    public class Class : Element
    {
        public Class(string name) : base(name)
        {
            
        }
        public override string ToString()
        {
            return $"{Template}class {bareName}{base.ToString()};" + Environment.NewLine;
        }
    }

    public class Struct : Element
    {
        public Struct(string name) : base(name) { }

        public override string ToString() => $"{Template}struct {bareName};" + Environment.NewLine;
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
                return $"using {bareName} = {Nested[0]?.QualifiedName ?? "Unknown"}* (*)({Nested[1]?.QualifiedName ?? "Unknown"});" + Environment.NewLine;
            if (Nested.Count > 0)
                return $"using {bareName} = {Nested[0]?.QualifiedName ?? "Unknown"}* (*)(Unknown);" + Environment.NewLine;
            else
                return $"using {bareName} = Unknown* (*)(Unknown);" + Environment.NewLine;
        }
    }

    public class Enumeration : Element
    {
        public Enumeration(string name) : base(name)
        {

        }
        public override string ToString()
        {
            return $"enum {bareName}{base.ToString()};" + Environment.NewLine;
        }
    }
}
