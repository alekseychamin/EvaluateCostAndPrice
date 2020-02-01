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
            Manager manager = new Manager();
            string input;
            bool done = false;
            manager.ShowWelcomeString();

            while (!done)
            {
                Console.Write("Введите команду: ");
                input = Console.ReadLine();
                try
                {
                    switch (input.ToLower())
                    {
                        case "u":
                            manager.Update();
                            break;
                        case "s":                            
                            manager.SaveReports();
                            Console.WriteLine("Отчеты сохранены.");
                            break;
                        case "h":
                            manager.ShowNameColumn();
                            break;
                        case "c":
                            Console.Clear();
                            manager.ShowWelcomeString();
                            break;
                        case "q":
                            done = true;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
