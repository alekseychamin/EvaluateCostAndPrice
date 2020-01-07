﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    /// <summary>
    /// коллекция для хранения группы затрат с общим центром затрат (индивидульное имя, % рентабельности и т.д.)
    /// </summary>
    class GroupCost : IGetTypeEnumObject<TypeGroupCost>,
                      IGetTypeObject, ISetTax, 
                      ISetCurrency, ITax,
                      IGetCValuesByType, ICPValues, 
                      IGetPValuesByType                      
    {
        private List<Cost> listCost = new List<Cost>();
        private Dictionary<TypeCost, Values> costValuesByType = new Dictionary<TypeCost, Values>();
        private Dictionary<TypeCost, Values> priceValuesByType = new Dictionary<TypeCost, Values>();
        private double? costTax;
        private double? socialTax;
        private Values cost;
        private Values price;

        public String Name { get; set; }
        public List<Cost> ListCost { get => listCost; }
        public TypeCurrency Currency { get; set; }
        public TypeGroupCost TypeEnumObject { get; set; }
        public string FileNameCost { get; set; }
        public Func<Enum, string> GetTypeObject { get; set; }
        public Dictionary<TypeCost, Values> CostValuesByType => costValuesByType;
        public Dictionary<TypeCost, Values> PriceValuesByType => priceValuesByType;
        public double? CostTax
        {
            get => costTax;
            set
            {
                if (value <= 0)
                    costTax = 0.2;
                else
                    costTax = value;
            }
        }
        public double? SocialTax
        {
            get => socialTax;
            set
            {
                if (value < 0)
                    socialTax = 0;
                else
                    socialTax = value;
            }
        }
        public Values CostValues => cost;
        public Values PriceValues => price;
        public double? KoefCalcProf
        {
            get
            {
                if (PriceValues.WithNoTax.HasValue && PriceValues.WithNoTax != 0)
                    return (PriceValues.WithNoTax - CostValues.WithNoTax) / PriceValues.WithNoTax;
                else
                    return null;
            }
        }
        public double? Kprofitability { get; set; }
                

        /// <summary>
        /// метод для загрузки перечня затрат в составе цента затрат
        /// </summary>
        /// <param name="filename"></param>
        public void LoadCost(string filename)
        {
            List<Cost> listT = new List<Cost>();
            listT = ReadFile.GetObjects<Cost>(filename, Properties.Value);
            listCost.AddRange(listT);
        }

        public void SetTax()
        {
            foreach (var item in listCost)
            {
                item.CostTax = this.CostTax;
                item.SocialTax = this.SocialTax;
            }
        }
        public void SetCurrency()
        {
            CurrencyNameValue groupCurrencyName =
                    Manager.ListCurrencyNameValue.Find(x => x.TypeEnumObject.Equals(this.Currency));

            foreach (var item in listCost)
            {
                CurrencyNameValue itemCurrencyName =
                    Manager.ListCurrencyNameValue.Find(x => x.TypeEnumObject.Equals(item.Currency));

                if ((itemCurrencyName != null) && (groupCurrencyName != null))
                {
                    double itemValueCurrency = itemCurrencyName.Value;
                    double groupValueCurrency = groupCurrencyName.Value;
                    item.KoefCurrency = itemValueCurrency / groupValueCurrency;
                    item.Currency = this.Currency;
                }
            }
        }

        public void GetPriceValues(List<Profitability> listProf = null)
        {
            price.Tax = 0;
            price.WithNoTax = 0;
            price.WithTax = 0;
            price.Currency = TypeCurrency.None;

            foreach (var item in listCost)
            {
                item.GetPriceValues(listProf);

                price.Tax += item.PriceValues.Tax;
                price.WithTax += item.PriceValues.WithTax;
                price.WithNoTax += item.PriceValues.WithNoTax;
                price.Currency = item.Currency;
            }
        }

        public void GetPriceValuesByType()
        {
            priceValuesByType.Clear();

            foreach (var item in listCost)
            {
                if (priceValuesByType.ContainsKey(item.TypeEnumObject))
                {
                    Values cost = priceValuesByType[item.TypeEnumObject];
                    cost.Tax += item.PriceValues.Tax;
                    cost.WithNoTax += item.PriceValues.WithNoTax;
                    cost.WithTax += item.PriceValues.WithTax;
                    cost.Currency = item.Currency;
                    priceValuesByType[item.TypeEnumObject] = cost;
                }
                else
                    priceValuesByType.Add(item.TypeEnumObject, item.PriceValues);
            }
        }

        public void GetCostValues()
        {
            cost.Tax = 0;
            cost.WithNoTax = 0;
            cost.WithTax = 0;
            cost.Currency = TypeCurrency.None;

            foreach (var item in listCost)
            {
                cost.Tax += item.CostValues.Tax;
                cost.WithTax += item.CostValues.WithTax;
                cost.WithNoTax += item.CostValues.WithNoTax;
                cost.Currency = item.Currency;
            }
        }

        public void GetCostValuesByType()
        {
            costValuesByType.Clear();

            foreach (var item in listCost)
            {
                if (costValuesByType.ContainsKey(item.TypeEnumObject))
                {
                    Values cost = costValuesByType[item.TypeEnumObject];
                    cost.Tax += item.CostValues.Tax;
                    cost.WithNoTax += item.CostValues.WithNoTax;
                    cost.WithTax += item.CostValues.WithTax;
                    cost.Currency = item.Currency;
                    costValuesByType[item.TypeEnumObject] = cost;
                }
                else
                    costValuesByType.Add(item.TypeEnumObject, item.CostValues);
            }

            //foreach (TypeCost typeCost in Enum.GetValues(typeof(TypeCost)))
            //{
            //    Values costBN;
            //    costBN.WithNoTax = 0;
            //    costBN.WithTax = 0;
            //    costBN.Tax = 0;
            //    costBN.Currency = this.Currency;

            //    bool isExist = false;
            //    foreach (Cost cost in listCost)
            //    {
            //        if (cost.TypeEnumObject == typeCost)
            //        {
            //            isExist = true;
            //            costBN.WithNoTax += cost.CostValues.WithNoTax;
            //            costBN.Tax += cost.CostValues.Tax;
            //            costBN.WithTax += cost.CostValues.WithTax;
            //        }
            //    }
            //    if (isExist) costValuesByType.Add(typeCost, costBN);
            //}
        }

        public void EvaluateCost()
        {
            foreach (var item in listCost)
            {
                item.EvaluateCost();
            }
        }

        public void EvaluateTaxWorkCost()
        {
            List<Cost> listT = new List<Cost>();
            foreach (var item in listCost)
            {
                if (item.TypeEnumObject == TypeCost.Work)
                {
                    CostWork costWork = item as CostWork;
                    if (costWork != null)
                    {
                        TaxWorkCost taxWC = costWork.AddTaxWorkCost();
                        if (taxWC != null) listT.Add(taxWC);
                    }
                }
            }
            listCost.AddRange(listT);
        }
        
        public void AddCost(TypeCost typeCost, string name, double unitCost, TypeCurrency typeCurrency, double count)
        {
            Type type = TypeObject.Value[typeCost].SystemType;
            Cost cost = (Cost)ReadFile.CreateType(type);
            cost.TypeEnumObject = typeCost;
            cost.GetTypeObject = TypeObject.GetTypeObject;
            //cost.ChangeValue += (sender, e) => { Console.WriteLine(e.NewValue); };
            cost.Name = name;
            cost.UnitTaxCost = unitCost;
            cost.Currency = typeCurrency;
            cost.Count = count;
            listCost.Add(cost);
        }
        public void ShowCost()
        {
            Console.WriteLine($"Наименование центра затрат: {Name}");
            Console.WriteLine($"Рентабельность фактическая: {KoefCalcProf * 100:0} %");
            Console.WriteLine($"Общая стоимость без НДС: {CostValues.WithNoTax:N} {Currency}\n" +
                              $"НДС: {CostValues.Tax:N} {Currency}\n" +
                              $"Общая стоимость с НДС: {CostValues.WithTax:N} {Currency}");

            //foreach (var item in costValuesByType)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine($"Наименование типа затрат: {TypeObject.GetTypeObject(item.Key)}\n" +                                  
            //                      $"Общая стоимость без НДС: {item.Value.WithNoTax:N} {Currency}\n" +
            //                      $"НДС: {item.Value.Tax:N} {Currency}\n" +
            //                      $"Общая стоимость с НДС: {item.Value.WithTax:N} {Currency}");
            //}

            //foreach (var item in listCost)
            //{
            //    Console.WriteLine(item.ToString());
            //    Console.WriteLine(new string('-', 20));
            //}
        }
    }
}