using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class TypeObject
    {
        public static Dictionary<Enum, TypeName> Value = new Dictionary<Enum, TypeName>()
        {
            { TypeProject.MainProject, new TypeName {SystemType = typeof(Project), Name = "Проект" } },
            { TypeProfitability.MainProfitability, new TypeName {SystemType = typeof(Profitability), Name = "Рентабельность" } },
            { TypeGroupCost.MainGroup, new TypeName {SystemType = typeof(GroupCost), Name = "Центр затрат" } },
            { TypeCost.Material, new TypeName {SystemType = typeof(CostMaterial), Name = "ПКИ" } },
            { TypeCost.Tax, new TypeName {SystemType = typeof(CostMaterial), Name = "Налог" } },
            { TypeCost.WorkOffice, new TypeName {SystemType = typeof(CostWorkOffice), Name = "ФОТ в офисе" } },
            { TypeCost.WorkOnSite, new TypeName {SystemType = typeof(CostWorkOnSite), Name = "ФОТ на объекте" } },
            { TypeCost.Other, new TypeName {SystemType = typeof(CostOther), Name = "Прочее" } },
            { TypeCost.Service, new TypeName {SystemType = typeof(CostService), Name = "Услуги" } },
            { TypeCurrency.Rub, new TypeName {SystemType = typeof(CurrencyNameValue), Name = "рубль" } },
            { TypeCurrency.Usd, new TypeName {SystemType = typeof(CurrencyNameValue), Name = "доллар" } },
            { TypeCurrency.Eur, new TypeName {SystemType = typeof(CurrencyNameValue), Name = "евро" } }
        };        

        public static Type GetTypeObject(string line, string[] headersEn)
        {
            string[] lines = ReadFile.GetSplitString(line);
            var propName = typeof(IGetTypeEnumObject<int>).GetProperties();

            for (int i = 0; i < headersEn.Length; i++)
            {
                if (headersEn[i] != null && headersEn[i].Equals(propName[0].Name))
                {
                    try
                    {
                        Enum key = Value.First(x => x.Value.Name == lines[i].Trim()).Key;
                        return Value[key].SystemType;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(line);
                        throw;
                    }
                    
                }                    
            }
            return null;
        }
        public static Enum GetTypeObject(string name)
        {
            return Value.First(x => x.Value.Name == name).Key;
        }
        public static string GetTypeObject(Enum value)
        {
            return Value[value].Name;
        }        
    }

    class TypeName
    {
        // поле для хранения типа затарты typeof(Cost)
        public Type SystemType { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// класс для описания соответствия свойств объекта и наименования свойства в файле на русском
    /// </summary>
    class Properties
    {
        public static Dictionary<string, string> Value = new Dictionary<string, string>()
        {
          { "Наименование" , "Name" },
          { "Тип", "TypeEnumObject" },
          { "Ст-ть ед. с НДС", "UnitTaxCost" },
          { "Ст-ть ед. без НДС", "UnitNoTaxCost" },
          { "Ст-ть часа с НДС", "UnitTaxCost" },
          { "Ст-ть часа без НДС", "UnitNoTaxCost" },
          { "Валюта", "Currency" },
          { "Кол-во ед.", "Count" },
          { "Кол-во чел.", "CountHuman" },
          { "Кол-во часов", "Count" },
          { "Кол-во дней", "Count" },
          { "Часть системы", "PartSystem" },
          { "Специалист", "NameWorker" },
          { "Комментарий", "Comment" },
          { "Значение", "Value" },
          { "Рег. коэф.", "Koef" },
          { "Коэф. цены", "Koef" },
          { "Коэф. участия", "Koef" },
          { "Расчет", "isCalculate" },
          { "Файл затрат", "FileNameCost" },
          { "Файл рентабельность", "FileNameProf" },
          { "Папка проекта", "ProjectFolder" },
          { "Рентабельность", "Kprofitability" },
          { "Длительность раб. дней", "Duration" },
          { "Id", "Id" },
          { "Заметка", "Note" },
          { "НДС", "CostTax" },
          { "Соц. налог", "SocialTax" }
        };
    }   
}
