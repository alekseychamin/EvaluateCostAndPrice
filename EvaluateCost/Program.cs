using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class Program
    {
        static void Main(string[] args)
        {
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //Console.WriteLine(Thread.CurrentThread.CurrentCulture);
            Manager manager = new Manager();
            manager.GetTypeCurrencyNameValue("Курс валют.csv");
            manager.LoadProject("Проекты.csv");
            manager.LoadProjectProfitability();
            manager.EvaluateTaxWorkCost();            
            manager.EvaluateCost();
            manager.SetTaxAndCurrency();
            manager.GetCostValues();
            manager.GetPriceValues();
            try
            {
                
                //manager.AddGroupCost("Разработка КД");
                //manager.AddGroupCost("Материалы");
                //manager.LoadCostInGroup("Материалы", "Материалы.csv");
                //manager.AddCostInGroup("Разработка КД", TypeCost.Work, "Разработка ТЗ", 500.0, TypeCurrency.Rub, 10.0);
                //manager.AddCostInGroup("Материалы", TypeCost.Material, "Лопата", 2100.0, TypeCurrency.Eur, 8.0);


                manager.ShowCost();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.ReadLine();
        }
    }
}
