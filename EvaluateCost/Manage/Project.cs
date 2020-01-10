using System;
using System.Collections.Generic;
using System.IO;
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
        private List<Profitability> listEvaluateProf = new List<Profitability>();
        private Dictionary<TypeCost, Values> costValuesByType = new Dictionary<TypeCost, Values>();
        private Dictionary<TypeCost, Values> priceValuesByType = new Dictionary<TypeCost, Values>();
        private Dictionary<TypeCost, double?> kprof = new Dictionary<TypeCost, double?>();        
        private Dictionary<string, Dictionary<TypeCost, Values>> costValuesByCommentType = 
                                                                    new Dictionary<string, Dictionary<TypeCost, Values>>();
        private Dictionary<string, Dictionary<TypeCost, Values>> priceValuesByCommentType = 
                                                                    new Dictionary<string, Dictionary<TypeCost, Values>>();
        private Dictionary<string, Values> costValuesByComment = new Dictionary<string, Values>();
        private Dictionary<string, Values> priceValuesByComment = new Dictionary<string, Values>();
        private Report report;

        private double? costTax;
        private double? socialTax;
        private Values cost;
        private Values price;
        
        public string Name { get; set; }
        public string FileNameCost { get; set; }
        public string FileNameProf { get; set; }
        public string ProjectFolder { get; set; }
        public TypeProject TypeEnumObject { get; set; }
        public List<GroupCost> ListNameGroupCost { get => listNameGroupCost; }
        public Func<Enum, string> GetTypeObject { get; set; }
        public List<Profitability> ListEvaluateProf { get => listEvaluateProf; }
        public List<Profitability> ListProfitability { get => listProfitability; }
        public Dictionary<string, Dictionary<TypeCost, Values>> CostValuesByCommentType { get => costValuesByCommentType; }
        public Dictionary<string, Dictionary<TypeCost, Values>> PriceValuesByCommentType { get => priceValuesByCommentType; }
        public Dictionary<string, Values> CostValuesByComment { get => costValuesByComment; }
        public Dictionary<string, Values> PriceValuesByComment { get => priceValuesByComment; }
        public Report Report { get => report; }

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

        public Project()
        {
            report = new Report(this);
        }
        
        public void GetPriceWithProf()
        {
            InitListProf();
            bool done = false;

            while (!done)
            {
                GetPriceValues(listEvaluateProf);
                GetPriceValuesByType();
                GetKprofByType();
                done = SetStepToListProf(0.01);
            }
        }
        public void GetCostValues()
        {
            cost.Tax = 0;
            cost.WithNoTax = 0;
            cost.WithTax = 0;
            cost.Currency = TypeCurrency.None;

            foreach (var group in listNameGroupCost)
            {
                group.GetCostValues();

                cost.Tax += group.CostValues.Tax;
                cost.WithTax += group.CostValues.WithTax;
                cost.WithNoTax += group.CostValues.WithNoTax;
                cost.Currency = group.Currency;
            }
        }

        public void GetPriceValues(List<Profitability> listProf = null)
        {
            price.Tax = 0;
            price.WithNoTax = 0;
            price.WithTax = 0;
            price.Currency = TypeCurrency.None;

            foreach (var group in listNameGroupCost)
            {
                if (listProf == null)
                    group.GetPriceValues(listProfitability);
                else
                    group.GetPriceValues(listProf);

                price.Tax += group.PriceValues.Tax;
                price.WithTax += group.PriceValues.WithTax;
                price.WithNoTax += group.PriceValues.WithNoTax;
                price.Currency = group.Currency;
            }

        }

        private void InitListProf()
        {
            listEvaluateProf.Clear();
            
            foreach (var item in listProfitability)
            {
                Profitability prof = new Profitability
                {
                    Name = item.Name,
                    TypeCost = item.TypeCost,
                    TypeEnumObject = item.TypeEnumObject,
                    GetTypeObject = item.GetTypeObject,
                    Value = 0
                };
                listEvaluateProf.Add(prof);
            }

        }

        private bool SetStepToListProf(double step)
        {            
            int count = 0;

            foreach (var key in kprof.Keys)
            {
                var profValue = (listProfitability.Find(x => x.TypeCost == key));
                var evalProf = (listEvaluateProf.Find(x => x.TypeCost == key));

                if (profValue != null && evalProf != null)
                {
                    if (evalProf.Value + step <= profValue.Value)
                        evalProf.Value += step;
                    else
                        count++;
                }
            }

            if ((count == listEvaluateProf.Count) || (KoefCalcProf >= Kprofitability))
                return true;
            else
                return false;            
        }

        public void GetKprofByType()
        {
            kprof.Clear();

            foreach (var item in CostValuesByType.Keys)
            {
                double? k = null;
                if (PriceValuesByType[item].WithNoTax.HasValue && PriceValuesByType[item].WithNoTax != 0)
                    k = (PriceValuesByType[item].WithNoTax - CostValuesByType[item].WithNoTax) / PriceValuesByType[item].WithNoTax;

                kprof.Add(item, k);
            }
        }

        private void AddValues(Dictionary<string, Dictionary<TypeCost, Values>> valuesByCommentType,                               
                               Values costValues, Cost cost, TypeCurrency currency)
        {
            if (valuesByCommentType.ContainsKey(cost.Comment))
            {
                if (valuesByCommentType[cost.Comment].ContainsKey(cost.TypeEnumObject))
                {
                    Values values = valuesByCommentType[cost.Comment][cost.TypeEnumObject];
                    values.WithNoTax += costValues.WithNoTax;
                    values.WithTax += costValues.WithTax;
                    values.Tax += costValues.Tax;
                    values.Currency = currency;
                    valuesByCommentType[cost.Comment][cost.TypeEnumObject] = values;
                }
                else
                {
                    Values values = costValues;
                    values.Currency = currency;
                    valuesByCommentType[cost.Comment].Add(cost.TypeEnumObject, values);
                }
            }
            else
            {
                Values values = costValues;
                values.Currency = currency;
                valuesByCommentType.Add(cost.Comment, new Dictionary<TypeCost, Values>());
                valuesByCommentType[cost.Comment].Add(cost.TypeEnumObject, values);
            }           
        }

        private void GetCostPriceValuesByCommentType()
        {            
            foreach (var group in listNameGroupCost)
            {
                foreach (var cost in group.ListCost)
                {
                    AddValues(costValuesByCommentType, cost.CostValues, cost, group.Currency);
                    AddValues(priceValuesByCommentType, cost.PriceValues, cost, group.Currency);
                }
            }
        }

        private void GetValuesByComment(Dictionary<string, Values> valuesComment, 
                                        Dictionary<string, Dictionary<TypeCost, Values>> valuesCommentType)
        {
            foreach (var keyName in valuesCommentType.Keys)
            {
                Values values;
                values.WithNoTax = 0;
                values.WithTax = 0;
                values.Tax = 0;
                values.Currency = TypeCurrency.None;
                foreach (var keyType in valuesCommentType[keyName].Keys)
                {
                    values.Currency = valuesCommentType[keyName][keyType].Currency;
                    values.WithNoTax += valuesCommentType[keyName][keyType].WithNoTax;
                    values.WithTax += valuesCommentType[keyName][keyType].WithTax;
                    values.Tax += valuesCommentType[keyName][keyType].Tax;
                }
                valuesComment.Add(keyName, values);
            }
        }
        public void GetCostPriceValuesByComment()
        {
            GetCostPriceValuesByCommentType();
            GetValuesByComment(costValuesByComment, costValuesByCommentType);
            GetValuesByComment(priceValuesByComment, priceValuesByCommentType);
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
            ReadFile.Load<GroupCost>(filename, listNameGroupCost, this.ProjectFolder);
            foreach (var item in listNameGroupCost)                           
                LoadCostInGroup(item.Name, item.FileNameCost, this.ProjectFolder);
            
        }

        private void AddTypeCostToListProf(List<Profitability> listProf)
        {
            foreach (var item in listProf)
            {
                try
                {
                    TypeCost typeCost = (TypeCost)TypeObject.GetTypeObject(item.Name);
                    item.TypeCost = typeCost;
                }
                catch (InvalidCastException)
                {
                    Exception ex = new InvalidCastException($"Неверное задание типа затрат в файле {this.FileNameProf}");
                    //throw;
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void LoadProjectProfitability()
        {
            ReadFile.Load<Profitability>(this.FileNameProf, listProfitability, this.ProjectFolder);
            AddTypeCostToListProf(listProfitability);
        }        

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

        public void ShowCost()
        {
            report.MakeFinancialReport();
            report.SaveReport();
            report.Show();
        }
    }
}
