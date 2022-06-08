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
        public string Template { get; set; }
        public string QualifiedName => Namespace != null ? Namespace + "::" + Name : Name;

        public Element(string Format, params string[] Types)
        {
            var firstType = Types[0].Split("::", 2);
            if (firstType.Length > 1)
            {
                Namespace = firstType[0];
                Types[0] = firstType[1];
            }

            Name = Types[0];

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
            if (!string.IsNullOrEmpty(Template))
            template = "tempalate <" + string.Join(", ", Template.Trim('<', '>').Split(",").Select(t => "typename " + t).ToArray()) + "> ";

            return template + String.Format(Format, Types);
        }
    }

    public class Format
    {
        public static string Class = "class {0};";
        public static string Alias = "using {0} = {1}* (*)({2});";
        public static string Struct = "struct {0};";
    }

}
