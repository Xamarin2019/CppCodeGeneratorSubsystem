using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CppCodeGeneratorSubsystem
{
    public class Сompiler
    {
        public IRepository Repository { get; set; }
        //includes input
        public List<string> Includes { get; set; } = new List<string>();

        //forward declarations input
        public List<string> Declarations { get; set; } = new List<string>();

        List<string> IncludesOutput { get; set; } = new List<string>();

        List<Element> DeclarationsOutput { get; set; } = new List<Element>();

        public Сompiler(IRepository repository)
        {
            Repository = repository;
        }


        public void Compile()
        {
            // Здесь добавляю заголовки для включения
            GetFilenamess(Includes);

            // Здесь добавляю типы для объявления
            GetElements(Declarations);
        }

        void GetFilenamess(IEnumerable<String> includes)
        {
            foreach (var typeName in includes)
            {
                var fileName = Repository.GetFilename(typeName);
                IncludesOutput.Add(fileName);
                IncludesOutput = IncludesOutput.Distinct().OrderBy(k => k).ToList();
            }
        }

        void GetElements(IEnumerable<String> declarations)
        {         
            foreach (var typeName in declarations.OrderBy(n => n))
            {

                var fileName = Repository.GetFilename(typeName);
                if (fileName == null) throw new NullReferenceException("Type was not found in the repository!");

                // Если уже есть включение, пропускаем
                if (IncludesOutput.Contains(fileName))
                {
                    continue;
                }

                // Если содержится в списке для обязательного включения, делаем включение и переходим на следующую итерацию
                if (Repository.IncludeOnlyTypes.Contains(typeName))
                {
                    IncludesOutput.Add(fileName);
                    continue;
                }

                // Получаем элемент для указанного типа
                //var element = Repository.GetType(typeName);
                var element = Repository.GetFilenameType(fileName, typeName);

                // Если типа нет в репозитории, выбрасывам исключение
                if (element == null) throw new NullReferenceException("Type was not found in the repository!");

                // Если тип уже попадался, пропускаем
                if (DeclarationsOutput.Contains(element)) continue;

                // Пробежимся по вложенным типам
                GetElements(element.NestedTypes.Select(t => t.QualifiedName));

                // И добавляем к ним зависимый тип
                if (element.Namespace == null)
                {
                    // Лучше переместить в формирование строки
                    DeclarationsOutput = new Element[]{ element }.Concat(DeclarationsOutput).ToList();
                }
                else
                {
                    // DeclarationsOutput.Add(element);

                    // Находим первый такой же namespace
                    var namespaceIndex = DeclarationsOutput.FindIndex(e => e.Namespace == element.Namespace);

                    // Список уже добавленных имен
                    var qualifiedNames1 = DeclarationsOutput.Select(t => t.QualifiedName).ToList();

                    // Список вложенных имен нового элемента
                    var qualifiedNames2 = element.NestedTypes.Select(t => t.QualifiedName).ToList();

                    // Находим индекс последнего вхожления одного из вложенных имен нового элемента
                    int index = -1; int tmp = -1;
                    if (namespaceIndex >= 0)
                    {
                        foreach (var qualifiedName in qualifiedNames2)
                        {
                            // Здесь косяк, нужно проверять уже включенные элементы, а не любой из списка для включения
                            if (Repository.IncludeOnlyTypes.Contains(qualifiedName)) continue;
                            tmp = qualifiedNames1.FindLastIndex(n => qualifiedName == n);
                            index = (tmp > index) || (tmp < 0) ? tmp : index;
                        }  
                    }

                    // Если имеется такой же namespace и он расположен после всех вхождений вложенных элементов
                    if (namespaceIndex >= 0 && index >= 0 && index < namespaceIndex)
                    {
                        DeclarationsOutput.Insert(namespaceIndex + 1, element);
                    }
                    else DeclarationsOutput.Add(element);

                }

            }

            IncludesOutput = IncludesOutput.Distinct().OrderBy(k => k).ToList();
        }

        
        // Генерация кода С++
        public string BuildOutput() 
        {
            string output = "";

            // Получаем списки включений и предварительных объявлений
            Compile();

            // Генерируем список включений
            foreach (var item in IncludesOutput)
            {
                output += $"#include {item}" + Environment.NewLine;
            }

            output += Environment.NewLine;

            // Генерируем предварительные объявления 
            string currentNmespace = null;
            foreach (var element in DeclarationsOutput)
            {
                // Если имеется пространство имен, делаем обертку для него
                if (!string.IsNullOrEmpty(currentNmespace) && currentNmespace != element.Namespace) output += "}" + Environment.NewLine + Environment.NewLine;
                if (!string.IsNullOrEmpty(element.Namespace))
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
            if (!string.IsNullOrEmpty(currentNmespace)) output += "}" + Environment.NewLine + Environment.NewLine;

            return output;
        }
    }
}
