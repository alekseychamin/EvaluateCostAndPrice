using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class Report
    {
        private Project project;
        private List<StringBuilder> listReport = new List<StringBuilder>();
        public List<StringBuilder> ListReport { get => listReport; }


        private void RemoveReport(string filename)
        {
            StringBuilder str = listReport.Find(x => Convert.ToString(x).Contains(filename));
            if (str != null)
                listReport.Remove(str);
        }

        public Report(Project project)
        {
            this.project = project;
        }

        public void SaveReport()
        {
            foreach (var report in listReport)
            {
                string[] rep = Convert.ToString(report).Split('\n');
                string filename = rep[0].Trim();
                if (rep.Length > 1)
                {
                    for (int i = 1; i < rep.Length; i++)
                        File.AppendAllText(filename, rep[i].Trim() + Environment.NewLine);
                }
            }
        }

        private Values GetValues(Dictionary<TypeCost, Values> valuesByType, TypeCost key)
        {
            return valuesByType[key];
        }

        private Values GetValues(Dictionary<string, Values> valuesByComment, string key)
        {
            return valuesByComment[key];
        }

        private Values GetValues(Dictionary<string, Dictionary<TypeCost, Values>> valuesByCommentType, string keyName, TypeCost keyType)
        {
            return valuesByCommentType[keyName][keyType];
        }

        public void MakeFinancialReport()
        {
            Values values;
            string filename = project.ProjectFolder + "\\" + "Финансовый отчет.txt";
            RemoveReport(filename);

            StringBuilder reportAllProject = new StringBuilder();
            reportAllProject.AppendLine(filename);
            reportAllProject.AppendLine(DateTime.Now.ToString());

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine($"###### {project.Name} ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();

            reportAllProject.AppendLine($"Рентабельность заданная: {project.Kprofitability * 100} %");
            reportAllProject.AppendLine($"Рентабельность фактическая: {project.KoefCalcProf * 100:0} %");
            reportAllProject.AppendLine($"НДС %: {project.CostTax * 100} %");
            reportAllProject.AppendLine($"Соц. налог %: {project.SocialTax * 100} %\n");
            reportAllProject.AppendLine(Cost.ShowCostValues(project.CostValues));
            reportAllProject.AppendLine(Cost.ShowPriceValues(project.PriceValues));
            reportAllProject.AppendLine($"Прибыль без НДС: {(project.PriceValues.WithNoTax - project.CostValues.WithNoTax):N} " +
                                        $"{project.CostValues.Currency}\n");

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine("###### Группировка по типам затрат ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();

            foreach (var keyType in project.CostValuesByType.Keys)
            {
                values = GetValues(project.CostValuesByType, keyType);
                reportAllProject.AppendLine($"###### {TypeObject.GetTypeObject(keyType)} ######");
                reportAllProject.AppendLine(Cost.ShowCostValues(values));

                values = GetValues(project.PriceValuesByType, keyType);
                reportAllProject.AppendLine($"Рентабельность фактическая: {project.Kprof[keyType] * 100:0} %");
                reportAllProject.AppendLine(Cost.ShowPriceValues(values));
            }

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine("###### Группировка по центрам затрат ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();

            foreach (var group in project.ListNameGroupCost)
            {
                reportAllProject.AppendLine($"###### {group.Name} ######");
                reportAllProject.AppendLine(Cost.ShowCostValues(group.CostValues));
                reportAllProject.AppendLine(Cost.ShowPriceValues(group.PriceValues));
            }

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine("###### Группировка по комментариям затрат ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();

            foreach (var keyName in project.CostValuesByComment.Keys)
            {
                values = GetValues(project.CostValuesByComment, keyName);
                reportAllProject.AppendLine($"###### {keyName} ######");
                reportAllProject.AppendLine(Cost.ShowCostValues(values));

                values = GetValues(project.PriceValuesByComment, keyName);
                reportAllProject.AppendLine(Cost.ShowPriceValues(values));

                //foreach (var keyType in costValuesByCommentType[keyName].Keys)
                //{
                //    values = GetValues(CostValuesByCommentType, keyName, keyType);
                //    reportAllProject.AppendLine($"Наименование типа затрат: {TypeObject.GetTypeObject(keyType)}");
                //    reportAllProject.AppendLine(Cost.ShowCostValues(values));

                //    values = GetValues(PriceValuesByCommentType, keyName, keyType);
                //    reportAllProject.AppendLine(Cost.ShowPriceValues(values));
                //}
            }
            listReport.Add(reportAllProject);
        }

        public void Show()
        {
            foreach (var item in listReport)
                Console.WriteLine(item.ToString());
        }

    }
}
