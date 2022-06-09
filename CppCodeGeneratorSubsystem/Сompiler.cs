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

        List<Element> DeclarationsOutput { get; set; }
 
        public void Compile()
        {
            // Здесь добавляю заголовки для включения
            foreach (var item in Includes)
            {
                var tmp = Repository.AvailableTypes.Where(d => d.Value.Exists(e => e.QualifiedName == item)).FirstOrDefault().Key;
                IncludeOutput.Add(tmp);
            }

            // Здесь добавляю элементы предварительные объявления с группированные по постранствам имен
            DeclarationsOutput = GetDeclarations(Declarations);
        }

        // Метод для получения элементов предварительные объявления с группированные по постранствам имен
        List<Element> GetDeclarations (IEnumerable<String> Declarations) // IEnumerable<IGrouping<string, Element>> 
        {
            List<Element> elements = new List<Element>();

            // Проверяем входной список для предварительных объявлений
            foreach (var item in Declarations)
            {

                var tmp = Repository.AvailableTypes.Where(d => d.Value.Exists(e => e.QualifiedName == item)).FirstOrDefault().Key;

                // Если уже есть включение, пропускаем
                if (IncludeOutput.Contains(tmp))
                {
                    continue;
                }

                // Если содержится в списке для обязательного включения, делаем включение и переходим на следующую итерацию
                if (includeOnly.Contains(item))
                {
                    IncludeOutput.Add(tmp);
                    continue;
                }

                // Получаем элемент для указанного типа
                var element = Repository.AvailableTypes.Select(d => d.Value)
                                                       .Where(l => l.Exists(e => e.QualifiedName == item))
                                                       .FirstOrDefault()?.FirstOrDefault(e => e.QualifiedName == item);
                // и добавляем его в выходной список
                elements.Add(element);
            }


            // Проверяем вложенные типы
            foreach (var element in elements.ToList())
            {
                // Если вложенные типы требуют включения, присоединяем их в начало
                elements = GetDeclarations(element.Types.Skip(1)).Concat(elements).ToList();        
            }


            // Немного упорядочиваем
            var elementGroups = elements.GroupBy(e => e.Namespace).OrderBy(g => g.Key) as IEnumerable<IGrouping<string, Element>>;

            return elements;
        }

        // Генерация кода С++
        public string BuildOutput() 
        {
            // Получаем списки включений и предварительных объявлений
            Compile();

            string output = "";
            
            // Сортируем и удаляем дубликаты из списка включений
            foreach (var item in IncludeOutput.Distinct().OrderBy(k => k))
            {
                // и добавляем в выходной текст
                output += $"#include {item}" + Environment.NewLine;
            }

            output += Environment.NewLine;

            // Генерируем предварительные объявления 
            foreach (var name in DeclarationsOutput.GroupBy(e => e.Namespace).OrderBy(g => g.Key) as IEnumerable<IGrouping<string, Element>>)
            {
                // Если имеется пространство имен, делаем обертку для него
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
