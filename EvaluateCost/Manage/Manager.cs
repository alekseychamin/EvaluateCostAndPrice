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

        public void LoadProject(string filename)
        {
            //listProject.Clear();

            //List<Project> listT = new List<Project>();
            //listT = ReadFile.GetObjects<Project>(filename, Properties.Value);
            //listProject.AddRange(listT);
            ReadFile.Load<Project>(filename, listProject);
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
        // TODO добавить алгоритм расчета фактической рентабельности проекта равной заданной                       
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

        public void ShowCost()
        {
            foreach (var project in listProject)
            {                
                project.ShowCost();
            }
        }
    }
}
