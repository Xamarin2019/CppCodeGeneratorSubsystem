using System;
using System.Linq;

namespace CppCodeGeneratorSubsystem
{
    public class Element
    {
        public string Namespace { get; set; }
        public string[] Types { get; set; }
        public string Format { get; set; }
        public string Name { get; set; }
        public string Template { get; set; } = "";
        public string QualifiedName => Namespace != null ? Namespace + "::" + Name : Name;

        public Element(string Format, params string[] Types)
        {
            // Если есть простарнство имен, сохраняем его отдельно
            var firstType = Types[0].Split("::", 2);
            if (firstType.Length > 1)
            {
                Namespace = firstType[0];
                Types[0] = firstType[1];
            }

            // Имя типа без пространства имен
            Name = Types[0];

            // Если есть шаблон, сохраняем его отдельно
            if (Types[0].Contains("<"))
            {
                Types[0] = Types[0].Split("<", 2)[0];
                Template = Name.Substring(Types[0].Length);
            }

            this.Types = Types;
            this.Format = Format;
        }

        public override string ToString()
        { 
            string template = "";

            // Если есть шаблон, формируем
            if (!string.IsNullOrEmpty(Template))
            template = "tempalate <" + string.Join(", ", Template.Trim('<', '>').Split(",").Select(t => "typename " + t).ToArray()) + "> ";
            // лепим его спереди
            return template + String.Format(Format, Types);
        }

        public override bool Equals(object obj)
        {
            var item = obj as Element;

            if (item == null)
            {
                return false;
            }

            bool result = Namespace.Equals(item.Namespace)
                       && Format.Equals(item.Format)
                       && Name.Equals(item.Name)
                       && Template.Equals(item.Template)
                       && QualifiedName.Equals(item.QualifiedName);

            if (Types.Length == item.Types.Length)
            {
                for (int i = 0; i < Types.Length; i++) if (Types[i] != item.Types[i]) result = false;
             }
            else
            {
                result = false;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return (Format + string.Join(",",Types)).GetHashCode();
        }
    }

    // Формирование сигнатур типов
    public class Format
    {
        public static string Class = "class {0};";
        public static string Alias = "using {0} = {1}* (*)({2});";
        public static string Struct = "struct {0};";
    }

}
