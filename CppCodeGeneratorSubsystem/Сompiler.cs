using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CppCodeGeneratorSubsystem
{
    public class Сompiler
    {
        //include only types
        string[] includeOnly = new string[] { "std::string" };

        //includes input
        public List<string> Includes { get; set; } = new List<string>();

        //forward declarations input
        public List<string> Declarations { get; set; } = new List<string>();

        List<string> IncludeOutput { get; set; } = new List<string>();

        IEnumerable<IGrouping<string, Element>> DeclarationsOutput { get; set; }
 
        public void Compile()
        {
            foreach (var item in Includes)
            {
                var tmp = Repository.AvailableTypes.Where(d => d.Value.Exists(e => e.QualifiedName == item)).FirstOrDefault().Key;
                IncludeOutput.Add(tmp);
            }

            DeclarationsOutput = GetDeclarations(Declarations);
        }

        IEnumerable<IGrouping<string, Element>> GetDeclarations (IEnumerable<String> Declarations)
        {
            List<Element> elements = new List<Element>();

            foreach (var item in Declarations)
            {

                var tmp = Repository.AvailableTypes.Where(d => d.Value.Exists(e => e.QualifiedName == item)).FirstOrDefault().Key;

                if (IncludeOutput.Contains(tmp))
                {
                    continue;
                }

                if (includeOnly.Contains(item))
                {
                    IncludeOutput.Add(tmp);
                    continue;
                }

                var element = Repository.AvailableTypes.Select(d => d.Value)
                                                       .Where(l => l.Exists(e => e.QualifiedName == item))
                                                       .FirstOrDefault()?.FirstOrDefault(e => e.QualifiedName == item);

                elements.Add(element);
            }

            var elementGroups = elements.GroupBy(e => e.Namespace).OrderBy(g => g.Key) as IEnumerable<IGrouping<string, Element>>;
            foreach (var group in elementGroups)
            {
                foreach (var element in group)
                {
                    elementGroups = GetDeclarations(element.Types.Skip(1)).Concat(elementGroups);
                }
            }

            return elementGroups;
        }

        public string BildOutput() 
        {
            Compile();

            string output = "";
            
            foreach (var item in IncludeOutput.Distinct().OrderBy(k => k))
            {
                output += $"#include {item}" + Environment.NewLine;
            }

            output += Environment.NewLine;

            foreach (var name in DeclarationsOutput)
            {
                if (name.Key != null)
                {
                    output += "namespace " + name.Key + Environment.NewLine + "{" + Environment.NewLine;
                    foreach (var item in name)
                    {
                        output += $"    {item}" + Environment.NewLine;
                    }
                    output += "}" + Environment.NewLine + Environment.NewLine;
                }
                else
                {
                    foreach (var item in name)
                    {
                        output += $"{item}" + Environment.NewLine;
                    }
                    output += Environment.NewLine;
                }

            }

            return output;
        }
    }
}
