using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CppCodeGeneratorSubsystem
{
    public class Сompiler
    {
        public Repository Repository { get; set; }
        //includes input
        public List<string> Includes { get; set; } = new List<string>();

        //forward declarations input
        public List<string> Declarations { get; set; } = new List<string>();

        List<string> IncludesOutput { get; set; } = new List<string>();

        List<Element> DeclarationsOutput { get; set; } = new List<Element>();

        public Сompiler(Repository repository)
        {
            Repository = repository;
        }


        public void Compile()
        {
            // Здесь добавляю заголовки для включения
            foreach (var item in Includes)
            {
                var tmp = Repository.AvailableTypes.Where(d => d.Value.Exists(e => e.QualifiedName == item)).FirstOrDefault().Key;
                IncludesOutput.Add(tmp);
            }

            // Здесь добавляю элементы предварительные объявления с группированные по постранствам имен
            GetElements(Declarations);
        }

        
        void GetElements(IEnumerable<String> Declarations)
        {
            foreach (var item in Declarations)
            {

                var tmp = Repository.AvailableTypes.Where(d => d.Value.Exists(e => e.QualifiedName == item)).FirstOrDefault().Key;

                // Если уже есть включение, пропускаем
                if (IncludesOutput.Contains(tmp))
                {
                    continue;
                }

                // Если содержится в списке для обязательного включения, делаем включение и переходим на следующую итерацию
                if (Repository.IncludeOnlyTypes.Contains(item))
                {
                    IncludesOutput.Add(tmp);
                    continue;
                }

                // Получаем элемент для указанного типа
                var element = Repository.AvailableTypes.Select(d => d.Value)
                                                       .Where(l => l.Exists(e => e.QualifiedName == item))
                                                       .FirstOrDefault()?.FirstOrDefault(e => e.QualifiedName == item);

                if (element == null) throw new NullReferenceException("Typewas not found in the repository!");

                if (DeclarationsOutput.Contains(element)) continue;

                // Пробежимся по вложенным типам
                GetElements(element.Types.Skip(1));

                // И добавляем к ним зависимый тип
                if (element.Namespace == null)
                {
                    // Лучше переместить в формирование строки
                    DeclarationsOutput = new Element[]{ element }.Concat(DeclarationsOutput).ToList();
                }
                else
                {
                    DeclarationsOutput.Add(element);
                }

            }
        }

        
        // Генерация кода С++
        public string BuildOutput() 
        {
            string output = "";

            // Получаем списки включений и предварительных объявлений
            Compile();
            
            // Сортируем и удаляем дубликаты из списка включений
            foreach (var item in IncludesOutput.Distinct().OrderBy(k => k))
            {
                // и добавляем в выходной текст
                output += $"#include {item}" + Environment.NewLine;
            }

            output += Environment.NewLine;

            // Генерируем предварительные объявления 
            string currentNmespace = null;
            foreach (var element in DeclarationsOutput)
            {
                // Если имеется пространство имен, делаем обертку для него
                if (currentNmespace != null && currentNmespace != element.Namespace) output += "}" + Environment.NewLine + Environment.NewLine;
                if (element.Namespace != null)
                {
                    if (currentNmespace != element.Namespace) output +=  "namespace " + element.Namespace + Environment.NewLine + "{" + Environment.NewLine;
 
                    output += $"    {element}" + Environment.NewLine;
                }
                else
                {
                   output += $"{element}" + Environment.NewLine;
 
                   output += Environment.NewLine;
                }
                
                currentNmespace = element.Namespace;

               

            }
            if (currentNmespace != null) output += "}" + Environment.NewLine + Environment.NewLine;

            return output;
        }
    }
}
