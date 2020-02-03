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
        private Dictionary<string, Dictionary<string, Values>> costValuesByCommentPartSystem = 
                                                                new Dictionary<string, Dictionary<string, Values>>();
        private Dictionary<string, Dictionary<string, Values>> priceValuesByCommentPartSystem =
                                                                new Dictionary<string, Dictionary<string, Values>>();
        private Dictionary<string, Dictionary<TypeCost, Values>> costValuesByPartSystemTypeCost =
                                                                new Dictionary<string, Dictionary<TypeCost, Values>>();
        private Dictionary<string, Dictionary<TypeCost, Values>> priceValuesByPartSystemTypeCost =
                                                                new Dictionary<string, Dictionary<TypeCost, Values>>();

        private Dictionary<string, Values> costValuesByPartSystem = new Dictionary<string, Values>();
        private Dictionary<string, Values> priceValuesByPartSystem = new Dictionary<string, Values>();
        private Dictionary<string, Dictionary<TypeCost, List<WorkerInfo>>> workerInfoByCommentTypeCost =
                                new Dictionary<string, Dictionary<TypeCost, List<WorkerInfo>>>();
        private List<WorkerInfo> workerInfoByProject = new List<WorkerInfo>();
        private Report report;

        private StringProperty<double?> duration = new StringProperty<double?>();
        private Dictionary<string, StringProperty<double?>> durationByComment = new Dictionary<string, StringProperty<double?>>();
        private double? costTax;
        private double? socialTax;
        private Values cost;
        private Values price;

        public StringProperty<double?> Duration { get => duration; set => duration = value; }
        public Dictionary<string, StringProperty<double?>> DurationByComment { get => durationByComment; }
        public double isCalculate { get; set; }
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
        public Dictionary<string, Dictionary<string, Values>> CostValuesByCommentPartSystem { get => costValuesByCommentPartSystem; }
        public Dictionary<string, Dictionary<string, Values>> PriceValuesByCommentPartSystem { get => priceValuesByCommentPartSystem; }
        public Dictionary<string, Dictionary<TypeCost, Values>> CostValuesByPartSystemTypeCost { get => costValuesByPartSystemTypeCost; }
        public Dictionary<string, Dictionary<TypeCost, Values>> PriceValuesByPartSystemTypeCost { get => priceValuesByPartSystemTypeCost; }

        public Dictionary<string, Values> CostValuesByPartSystem { get => costValuesByPartSystem; }
        public Dictionary<string, Values> PriceValuesByPartSystem { get => priceValuesByPartSystem; }

        public Dictionary<string, Dictionary<TypeCost, List<WorkerInfo>>> 
                                                WorkerInfoByCommentTypeCost { get => workerInfoByCommentTypeCost; }

        public List<WorkerInfo> WorkerInfoByProject { get => workerInfoByProject; }
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

        public void GetDurationById()
        {
            foreach (var group in listNameGroupCost)
                group.GetDurationById();
            
            duration = Evaluate.GetDurationById<GroupCost>(listNameGroupCost);
        }

        public void GetDurationByCommentId()
        {
            durationByComment = Evaluate.GetDurationByCommentId(listNameGroupCost);
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

        private void GetWorkerInfoByCommentTypeCost()
        {
            workerInfoByCommentTypeCost.Clear();
            Evaluate.GetWorkerInfoByCommetTypeCost(listNameGroupCost, workerInfoByCommentTypeCost);
        }
        private void GetCostPriceValuesByPartSystem()
        {
            costValuesByPartSystem.Clear();
            priceValuesByPartSystem.Clear();
            Evaluate.GetValuesByPartSystem(listNameGroupCost, costValuesByPartSystem, priceValuesByPartSystem);
        }

        private void GetCostPriceValuesByPartSystemTypeCost()
        {
            costValuesByPartSystemTypeCost.Clear();
            priceValuesByPartSystemTypeCost.Clear();
            Evaluate.GetValuesByPartSystemTypeCost(listNameGroupCost, costValuesByPartSystemTypeCost,
                                                   priceValuesByPartSystemTypeCost);
        }

        private void GetCostPriceValuesByCommentPartSystem()
        {
            costValuesByCommentPartSystem.Clear();
            priceValuesByCommentPartSystem.Clear();
            Evaluate.GetValuesByCommentPartSystem(listNameGroupCost, costValuesByCommentPartSystem, 
                                                  priceValuesByCommentPartSystem);
        }
        private void GetCostPriceValuesByCommentType()
        {            
            costValuesByCommentType.Clear();
            priceValuesByCommentType.Clear();
            Evaluate.GetValuesByCommentTypeCost(listNameGroupCost, costValuesByCommentType, priceValuesByCommentType);
        }

        private void GetValuesByComment(Dictionary<string, Values> costValuesByComment,
                                        Dictionary<string, Values> priceValuesByComment)
        {
            costValuesByComment.Clear();
            priceValuesByComment.Clear();
            Evaluate.GetValuesByComment(listNameGroupCost, costValuesByComment, priceValuesByComment);            
        }
        public void GetCostPriceValuesByComment()
        {
            GetCostPriceValuesByCommentType();
            GetCostPriceValuesByPartSystem();
            GetCostPriceValuesByCommentPartSystem();
            GetCostPriceValuesByPartSystemTypeCost();
            GetValuesByComment(costValuesByComment, priceValuesByComment);

            GetWorkerInfoByCommentTypeCost();
        }

        public void GetPriceValuesByType()
        {
            priceValuesByType.Clear();
            costValuesByType.Clear();
            Evaluate.GetValuesByType(listNameGroupCost, costValuesByType, priceValuesByType);            
        }        

        public void GetCostValuesByType()
        {
            costValuesByType.Clear();
            priceValuesByComment.Clear();
            Evaluate.GetValuesByType(listNameGroupCost, costValuesByType, priceValuesByType);
            
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

        public void SaveReports()
        {
            report.SaveReport();
        }

        public void ShowReports()
        {
            report.MakeFinancialReport("Финансовый отчет", "Фин_показатели проекта {0}.txt"); ;
            report.MakeForOtherCompanyReport("Отчет для ЗВЭК", "Отчет для ЗВЭК для проекта {0}.txt");
            report.MakeDuratinReport("Отчет по срокам", "Показатели длительности для проекта {0}.txt");            
            report.Show();
        }
    }
}
