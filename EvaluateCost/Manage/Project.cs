using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class Project : IGetTypeObject, IGetTypeEnumObject<TypeProject>, 
                    IGetCValuesByType, ISetCurrency, IGetKprofByType,
                    ISetTax, ITax, ICPValues, IGetPValuesByType
    {
        private List<GroupCost> listNameGroupCost = new List<GroupCost>();
        private List<Profitability> listProfitability = new List<Profitability>();
        private Dictionary<TypeCost, Values> costValuesByType = new Dictionary<TypeCost, Values>();
        private Dictionary<TypeCost, Values> priceValuesByType = new Dictionary<TypeCost, Values>();
        private Dictionary<TypeCost, double?> kprof = new Dictionary<TypeCost, double?>();
        private double? costTax;
        private double? socialTax;
        private Values cost;
        private Values price;

        public string Name { get; set; }
        public string FileNameCost { get; set; }
        public string FileNameProf { get; set; }
        public string ProjectFolder { get; set; }
        public TypeProject TypeEnumObject { get; set; }
        public Func<Enum, string> GetTypeObject { get; set; }
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
        public Values CostValues => cost;
        public Values PriceValues => price;
        public Dictionary<TypeCost, Values> CostValuesByType => costValuesByType;
        public Dictionary<TypeCost, Values> PriceValuesByType => priceValuesByType;
        public double? Kprofitability { get; set; }
        public Dictionary<TypeCost, double?> Kprof => kprof;

        //public void Load<T>(string filename, string folder, List<T> listToAdd) where T : IGetTypeObject
        //{
        //    listToAdd.Clear();
        //    string projectFileName = folder + "\\" + filename;
        //    List<T> listT = new List<T>();
        //    listT = ReadFile.GetObjects<T>(projectFileName, Properties.Value);
        //    listToAdd.AddRange(listT);
        //}        

        public void GetPriceValues(List<Profitability> listProf = null)
        {
            price.Tax = 0;
            price.WithNoTax = 0;
            price.WithTax = 0;
            price.Currency = TypeCurrency.None;

            foreach (var group in listNameGroupCost)
            {
                group.GetPriceValues(listProfitability);

                price.Tax += group.PriceValues.Tax;
                price.WithTax += group.PriceValues.WithTax;
                price.WithNoTax += group.PriceValues.WithNoTax;
                price.Currency = group.Currency;
            }
            
        }

        public void GetKprofByType()
        {
            foreach (var item in CostValuesByType.Keys)
            {
                double? k = null;
                if (PriceValuesByType[item].WithNoTax.HasValue && PriceValuesByType[item].WithNoTax != 0) 
                    k = (PriceValuesByType[item].WithNoTax - CostValuesByType[item].WithNoTax) / PriceValuesByType[item].WithNoTax;

                kprof.Add(item, k);
            }
        }
        public void GetPriceValuesByType()
        {
            priceValuesByType.Clear();

            foreach (var group in listNameGroupCost)
                group.GetPriceValuesByType();

            foreach (var group in listNameGroupCost)
            {
                foreach (var item in group.PriceValuesByType)
                {
                    if (priceValuesByType.ContainsKey(item.Key))
                    {
                        Values price = priceValuesByType[item.Key];
                        price.Tax += item.Value.Tax;
                        price.WithNoTax += item.Value.WithNoTax;
                        price.WithTax += item.Value.WithTax;
                        priceValuesByType[item.Key] = price;
                    }
                    else
                        priceValuesByType.Add(item.Key, item.Value);
                }
            }
        }

        public void GetCostValues()
        {
            cost.Tax = 0;
            cost.WithNoTax = 0;
            cost.WithTax = 0;
            cost.Currency = TypeCurrency.None;

            foreach (var item in listNameGroupCost)
            {
                item.GetCostValues();

                cost.Tax += item.CostValues.Tax;
                cost.WithTax += item.CostValues.WithTax;
                cost.WithNoTax += item.CostValues.WithNoTax;
                cost.Currency = item.Currency;
            }            
        }

        public void GetCostValuesByType()
        {
            costValuesByType.Clear();

            foreach (var group in listNameGroupCost)
                group.GetCostValuesByType();

            foreach (var group in listNameGroupCost)
            {
                foreach (var item in group.CostValuesByType)
                {
                    if (costValuesByType.ContainsKey(item.Key))
                    {
                        Values cost = costValuesByType[item.Key];
                        cost.Tax += item.Value.Tax;
                        cost.WithNoTax += item.Value.WithNoTax;
                        cost.WithTax += item.Value.WithTax;
                        costValuesByType[item.Key] = cost;
                    }
                    else
                        costValuesByType.Add(item.Key, item.Value);
                }
            }

            //foreach (TypeCost typeCost in Enum.GetValues(typeof(TypeCost)))
            //{
            //    Values costBN;
            //    costBN.WithNoTax = 0;
            //    costBN.WithTax = 0;
            //    costBN.Tax = 0;
            //    costBN.Currency = TypeCurrency.None;

            //    bool isExist = false;
            //    foreach (GroupCost groupCost in listNameGroupCost)
            //    {
            //        if (groupCost.CostValuesByType.ContainsKey(typeCost))
            //        {
            //            isExist = true;
            //            costBN.WithNoTax += groupCost.CostValuesByType[typeCost].WithNoTax;
            //            costBN.Tax += groupCost.CostValuesByType[typeCost].Tax;
            //            costBN.WithTax += groupCost.CostValuesByType[typeCost].WithTax;
            //            costBN.Currency = groupCost.CostValuesByType[typeCost].Currency;
            //        }
            //    }
            //    if (isExist) costValuesByType.Add(typeCost, costBN);
            //}
        }

        public void ShowCost()
        {
            //foreach (var item in listProfitability)
            //{
            //    Console.WriteLine($"{item.Name} = {item.Value * 100} %");
            //}            

            Console.WriteLine(this.Name);
            Console.WriteLine($"Рентабельность заданная: {Kprofitability * 100} %");
            Console.WriteLine($"Рентабельность фактическая: {KoefCalcProf * 100:0} %");
            Console.WriteLine($"НДС %: {CostTax * 100} %");
            Console.WriteLine($"Соц. налог %: {SocialTax * 100} %");
            Console.WriteLine($"Общая стоимость без НДС: {CostValues.WithNoTax:N} {CostValues.Currency}\n" +
                              $"НДС: {CostValues.Tax:N} {CostValues.Currency}\n" +
                              $"Общая стоимость с НДС: {CostValues.WithTax:N} {CostValues.Currency}");
            Console.WriteLine(new string('-', 50));

            foreach (var item in CostValuesByType.Keys)
            {
                Console.WriteLine($"Наименование типа затрат: {TypeObject.GetTypeObject(item)}");
                Console.WriteLine($"Стоимость без НДС: {CostValuesByType[item].WithNoTax:N} {CostValuesByType[item].Currency}");
                Console.WriteLine($"НДС: {CostValuesByType[item].Tax:N} {CostValuesByType[item].Currency}");
                Console.WriteLine($"Стоимость с НДС: {CostValuesByType[item].WithTax:N} {CostValuesByType[item].Currency}");
                Console.WriteLine();
                Console.WriteLine($"Рентабельность фактическая: {Kprof[item] * 100:0} %");
                Console.WriteLine($"Цена без НДС: {PriceValuesByType[item].WithNoTax:N} {PriceValuesByType[item].Currency}");
                Console.WriteLine($"НДС: {PriceValuesByType[item].Tax:N} {PriceValuesByType[item].Currency}");
                Console.WriteLine($"Цена с НДС: {PriceValuesByType[item].WithTax:N} {PriceValuesByType[item].Currency}");
                Console.WriteLine();
            }
            Console.WriteLine(new string('-', 50));
            foreach (var group in listNameGroupCost)
            {
                group.ShowCost();
                Console.WriteLine(new string('-', 50));
            }
        }

        public void SetTax()
        {
            foreach (var item in listNameGroupCost)
            {
                item.CostTax = this.CostTax;
                item.SocialTax = this.SocialTax;
                item.SetTax();
            }
        }

        public void SetCurrency()
        {
            foreach (var item in listNameGroupCost)
                item.SetCurrency();
        }

        public void EvaluateTaxWorkCost()
        {
            foreach (var group in listNameGroupCost)
                group.EvaluateTaxWorkCost();
        }
        public void EvaluateCost()
        {
            foreach (var group in listNameGroupCost)
                group.EvaluateCost();
        }

        public void LoadGroupCost(string filename)
        {
            //listNameGroupCost.Clear();
            //string projectFileName = folder + "\\" + filename;
            //List<GroupCost> listT = new List<GroupCost>();
            //listT = ReadFile.GetObjects<GroupCost>(projectFileName, Properties.Value);
            //listNameGroupCost.AddRange(listT);
            ReadFile.Load<GroupCost>(filename, listNameGroupCost, this.ProjectFolder);
            foreach (var item in listNameGroupCost)
            {
                //item.CostTax = this.CostTax;
                LoadCostInGroup(item.Name, item.FileNameCost, this.ProjectFolder);
            }
        }

        public void LoadProjectProfitability()
        {
            ReadFile.Load<Profitability>(this.FileNameProf, listProfitability, this.ProjectFolder);            
        }

        //public void LoadListProfitability(string filename, string folder)
        //{
        //    listProfitability.Clear();
        //    string projectFileName = folder + "\\" + filename;
        //}

        public void AddGroupCost(String nameGroup)
        {
            var gc = listNameGroupCost.Find(x => x.Name.Equals(nameGroup));
            if (gc == null)
            {
                GroupCost groupCost = new GroupCost { Name = nameGroup };
                listNameGroupCost.Add(groupCost);
            }
            else
                Console.WriteLine($"Центр затрат с именем {nameGroup} уже существует.");
        }
        public void LoadCostInGroup(string nameGroup, string filename, string folder)
        {
            string projectFileName = folder + "\\" + filename;
            var gc = listNameGroupCost.Find(x => x.Name.Equals(nameGroup));
            if (gc != null)                            
                gc.LoadCost(projectFileName);            
            else
                Console.WriteLine($"Центр затрат с именем {nameGroup} не существует.");
        }
        public void AddCostInGroup(String nameGroup, TypeCost typeCost,
                                   String nameCost, double unitCost, TypeCurrency typeCurrency, double count)
        {
            var gc = listNameGroupCost.Find(x => x.Name.Equals(nameGroup));
            if (gc != null)                            
                gc.AddCost(typeCost, nameCost, unitCost, typeCurrency, count);            
            else
                Console.WriteLine($"Центр затрат с именем {nameGroup} не существует.");
        }
        
    }
}
