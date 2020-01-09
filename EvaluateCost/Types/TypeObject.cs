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
            { TypeCost.Material, new TypeName {SystemType = typeof(MaterialCost), Name = "ПКИ" } },
            { TypeCost.Tax, new TypeName {SystemType = typeof(MaterialCost), Name = "Налог" } },
            { TypeCost.Work, new TypeName {SystemType = typeof(CostWork), Name = "ФОТ" } },
            { TypeCost.Other, new TypeName {SystemType = typeof(OtherCost), Name = "Прочее" } },
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
                    Enum key = Value.First(x => x.Value.Name == lines[i].Trim()).Key;
                    return Value[key].SystemType;
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
          { "Стоимость ед. с НДС", "UnitTaxCost" },
          { "Стоимость ед. без НДС", "UnitNoTaxCost" },
          { "Стоимость часа с НДС", "UnitTaxCost" },
          { "Стоимость часа без НДС", "UnitNoTaxCost" },
          { "Валюта", "Currency" },
          { "Кол-во ед.", "Count" },
          { "Кол-во чел.", "CountHuman" },
          { "Кол-во часов", "Count" },
          { "Часть системы", "PartSystem" },
          { "Комментарий", "Comment" },
          { "Значение", "Value" },
          { "Рег. коэф.", "Koef" },
          { "Коэф. цены", "Koef" },
          { "Файл затрат", "FileNameCost" },
          { "Файл рентабельность", "FileNameProf" },
          { "Папка проекта", "ProjectFolder" },
          { "Рентабельность", "Kprofitability" },
          { "НДС", "CostTax" },
          { "Соц. налог", "SocialTax" }
        };
    }   
}
