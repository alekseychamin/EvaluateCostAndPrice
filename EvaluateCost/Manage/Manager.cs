using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{


    class Manager
    {
        private List<Project> listProject = new List<Project>();
        private static List<CurrencyNameValue> listCurrencyNameValue = new List<CurrencyNameValue>();
        public static List<CurrencyNameValue> ListCurrencyNameValue { get => listCurrencyNameValue; }
        
        /// <summary>
        /// Загрузить из файла валюты (наименование, тип, значение курса)
        /// </summary>
        /// <param name="fileName"></param>
        public void GetTypeCurrencyNameValue(string filename)
        {
            //listCurrencyNameValue.Clear();

            //List<CurrencyNameValue> listT = new List<CurrencyNameValue>();
            //listT = ReadFile.GetObjects<CurrencyNameValue>(filename, Properties.Value);
            //listCurrencyNameValue.AddRange(listT);
            ReadFile.Load<CurrencyNameValue>(filename, listCurrencyNameValue);
        }

        public void ShowWelcomeString()
        {
            string welcome = @"Программа для расчета коммерческих предложений." +
                              "{0} Для работы используются следующие команды:" +
                              "{0} -u (обновить расчет из описания проектов файл - Проекты.csv с учетом курсов валют из файла - Курс валют.csv," +
                              "{0}     файлы находятся в текущей директории программы и сохранить отчеты - отчет по срокам, финансовый отчет)" +
                              "{0} -s (сохранить отчеты - финансовый отчет, отчет длительности, отчет для ЗВЭК)" +
                              "{0} -h (вывести названия столбцов, которые можно использовать для ввода данных в csv файлах)" +
                              "{0} -c (очистить экран)" +
                              "{0} -q (выход из прогрммы) {0}";
            Console.WriteLine(welcome, Environment.NewLine);
        }

        public void ShowNameColumn()
        {
            Console.WriteLine("В файлах csv возможно использование следующих имен колонок: \n");
            foreach (var item in Properties.Value.Keys)
                Console.WriteLine($"{item}");
        }

        public void Update()
        {
            GetTypeCurrencyNameValue("Курс валют.csv");
            LoadProject("Проекты.csv");
            LoadProjectProfitability();
            EvaluateTaxWorkCost();
            EvaluateCost();
            SetTaxAndCurrency();
            GetCostValues();
            GetPriceWithProfProject();
            GetCostPriceValuesByComment();
            GetDuration();

            ShowReports();
        }

        private void RemoveProject()
        {
            int i = 0;
            while (i < listProject.Count)
            {
                if (listProject[i].isCalculate == 0)
                    listProject.RemoveAt(i);
                else
                    i++;
            }
        }

        public void LoadProject(string filename)
        {            
            ReadFile.Load<Project>(filename, listProject);
            RemoveProject();
            LoadProjectGroupCost();            
        }

        public void LoadProjectProfitability()
        {
            foreach (var project in listProject)
            {
                project.LoadProjectProfitability();
            }
        }

        private void LoadProjectGroupCost()
        {
            foreach (var project in listProject)            
                project.LoadGroupCost(project.FileNameCost);                                            
        }

        public void SetTaxAndCurrency()
        {
            foreach (var project in listProject)
            {
                project.SetTax();
                project.SetCurrency();
            }            
        }

        public void EvaluateTaxWorkCost()
        {
            foreach (var project in listProject)
                project.EvaluateTaxWorkCost();
        }
        public void EvaluateCost()
        {
            foreach (var project in listProject)
                project.EvaluateCost();
        }
        public void GetCostValues()
        {
            foreach (var project in listProject)
            {                
                project.GetCostValues();
                project.GetCostValuesByType();
            }
        }
        
        public void GetCostPriceValuesByComment()
        {
            foreach (var project in listProject)
                project.GetCostPriceValuesByComment();
        }

        public void GetPriceWithProfProject()
        {
            foreach (var project in listProject)
                project.GetPriceWithProf();
        }        
        /// <summary>
        /// Метод расчета цены проекта на основе заданных коэффициентов рентабельности по типам затрат
        /// </summary>
        public void GetPriceValues()
        {
            foreach (var project in listProject)
            {
                project.GetPriceValues();
                project.GetPriceValuesByType();
                project.GetKprofByType();
            }
        }

        public void GetDuration()
        {
            foreach (var project in listProject)
            {
                project.GetDurationById();
                project.GetDurationByCommentId();
            }
        }

        public void SaveReports()
        {
            foreach (var project in listProject)
            {
                project.SaveReports();
            }
        }

        public void ShowReports()
        {
            foreach (var project in listProject)
            {                
                project.ShowReports();
            }
        }
    }
}
