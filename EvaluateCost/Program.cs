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
        private static void Update(Manager manager)
        {
            try
            {
                manager.GetTypeCurrencyNameValue("Курс валют.csv");
                manager.LoadProject("Проекты.csv");
                manager.LoadProjectProfitability();
                manager.EvaluateTaxWorkCost();
                manager.EvaluateCost();
                manager.SetTaxAndCurrency();
                manager.GetCostValues();
                //manager.GetPriceValues();
                manager.GetPriceWithProfProject();
                manager.GetCostPriceValuesByComment();

                manager.ShowCost();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }            
        }
        static void Main(string[] args)
        {
            Manager manager = new Manager();
            string input;
            bool done = false;

            while (!done)
            {
                input = Console.ReadLine();

                switch (input.ToLower())
                {
                    case "u":
                        Update(manager);
                        break;
                    case "q":
                        done = true;
                        break;
                    default:
                        break;
                }                
            }
            

            
        }
    }
}
