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


        private void ClearReports(string filename)
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
                FileInfo fileInfo = new FileInfo(filename);
                Directory.CreateDirectory(fileInfo.Directory.FullName);

                if (fileInfo.Exists)
                    fileInfo.Delete();

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

        private (string, string) GetAmountWorkAndNote(string comment, TypeCost typeCost)
        {
            var result = (ReportWorkAmount: string.Empty, Note: string.Empty);
            double? amountWork = 0;
            double? countHuman = 0;
            Cost costFilter = null;

            foreach (var group in project.ListNameGroupCost)
            {
                foreach (var cost in group.ListCost)
                {
                    if (cost.Comment != null)
                    {
                        if (cost.Comment.Value.Equals(comment) && cost.TypeEnumObject == typeCost)
                        {
                            if (!string.IsNullOrEmpty(cost.Note))
                                result.Note += cost.Note + "\n";
                            countHuman += cost.CountHuman.Value;
                            amountWork += cost.AmountWork();
                            costFilter = cost;
                        }
                    }
                }
            }
            string reportWork = string.Format($"Выполнение работ со следующими условиями: \n" +
                                              $"{costFilter?.Count.Name}: {amountWork}\n");
            if (!string.IsNullOrEmpty(costFilter.Koef.Name))
                reportWork += string.Format($"{costFilter.Koef.Name}: {costFilter.Koef.Value}\n");
            reportWork += string.Format($"{costFilter.CountHuman.Name}: {countHuman}");            

            result.ReportWorkAmount = reportWork;
            return result;
        }

        private string GetNoteByComment(string comment)
        {
            string result = string.Empty;

            var q = from nameGroup in project.ListNameGroupCost
                    from cost in nameGroup.ListCost
                    where (string.IsNullOrEmpty(cost.Note) == false) && (cost.Comment.Value.Equals(comment))
                    select cost.Note;

            if (q != null)
            {
                foreach (var item in q)
                    result += item + "\n" ?? string.Empty;

            }
            return result;
        }
        private string GetNote(string comment, TypeCost typeCost)
        {
            string result = string.Empty;

            foreach (var group in project.ListNameGroupCost)
            {
                foreach (var cost in group.ListCost)
                {
                    if (cost.Comment != null)
                    {
                        if (cost.Comment.Value.Equals(comment) && cost.TypeEnumObject == typeCost)
                        {
                            if (!string.IsNullOrEmpty(cost.Note))
                                result += cost.Note + "\n";
                        }
                    }
                }
            }
            return result;
        }

        private string PrepareReports(string folderName, string fileNameReport)
        {
            fileNameReport = string.Format(fileNameReport, DateTime.Now.ToString("dd-MM-yyyy-HH-mm"));
            string filename = project.ProjectFolder + "\\" + folderName + "\\" + fileNameReport;
            ClearReports(fileNameReport);
            return filename;
        }

        public void MakeDuratinReport(string folderName, string fileNameReport)
        {
            string name = null;
            string filename = PrepareReports(folderName, fileNameReport);

            StringBuilder reportAllProject = new StringBuilder();
            reportAllProject.AppendLine(filename);

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine($"###### {project.Name} ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();

            name = project.Duration.Name ?? "Длительность раб. дней";
            reportAllProject.AppendLine($"{name}: {project.Duration.Value.ToString()}\n");

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine("###### Группировка по центрам затрат ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();

            int i = 1;
            foreach (var group in project.ListNameGroupCost)
            {
                reportAllProject.AppendLine($"###### {i}. {group.Name} ######");
                name = group.Duration?.Name ?? "Длительность раб. дней";
                reportAllProject.AppendLine($"{name}: {group.Duration.Value.ToString()}\n");
                i++;
            }

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine("###### Группировка по комментариям затрат ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();
            
            foreach (var keyName in project.DurationByComment.Keys)
            {
                reportAllProject.AppendLine($"###### {keyName} ######");
                name = project.DurationByComment[keyName].Name ?? "Длительность раб. дней";
                reportAllProject.AppendLine($"{name}: {project.DurationByComment[keyName].Value.ToString()}\n");
                reportAllProject.AppendLine($"Заметка: {GetNoteByComment(keyName)}\n");
            }

            listReport.Add(reportAllProject);
        }

        public void MakeForOtherCompanyReport(string folderName, string fileNameReport)
        {
            void ShowTypeCost(StringBuilder st, TypeCost keyType, string keyName, 
                              int n, Values v, 
                              string note, (string, string) rep)
            {
                st.AppendLine($"{n}. Тип затрат: {TypeObject.GetTypeObject(keyType)}");
                
                if (!string.IsNullOrEmpty(rep.Item1))
                {                    
                    if (!string.IsNullOrEmpty(rep.Item2)) st.AppendLine($"Заметка: {rep.Item2}");                    
                    st.AppendLine(rep.Item1);
                    //if (keyType == TypeCost.WorkOnSite) st.AppendLine();
                    string name = project.Duration.Name ?? "Длительность раб. дней";
                    st.AppendLine($"{name}: {Evaluate.GetDuration(project, keyName, keyType).Value}\n");
                }
                else
                {                    
                    if (!string.IsNullOrEmpty(note)) st.AppendLine($"Заметка: {note}");
                    if (v.WithTax.HasValue && v.WithNoTax.HasValue && v.Tax.HasValue) st.AppendLine(Cost.ShowPriceValues(v));                    
                }                                
            }
            

            Values values;
            string filename = PrepareReports(folderName, fileNameReport);            

            StringBuilder reportAllProject = new StringBuilder();
            reportAllProject.AppendLine(filename);

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine($"###### {project.Name} ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();

            
            foreach (var keyName in project.CostValuesByComment.Keys)
            {
                bool isMat = project.CostValuesByCommentType[keyName].Keys.ToList().Contains<TypeCost>(TypeCost.Material);
                bool isServ = project.CostValuesByCommentType[keyName].Keys.ToList().Contains<TypeCost>(TypeCost.Service);
                bool isWorkOffice = project.CostValuesByCommentType[keyName].Keys.ToList().Contains<TypeCost>(TypeCost.WorkOffice);
                bool isWorkOnSite = project.CostValuesByCommentType[keyName].Keys.ToList().Contains<TypeCost>(TypeCost.WorkOnSite);

                string note = string.Empty;

                if (isMat || isServ || isWorkOffice || isWorkOnSite)
                {
                    values = GetValues(project.CostValuesByComment, keyName);                    
                    reportAllProject.AppendLine($"###### {keyName} ######");                    

                    int j = 1;
                    foreach (var keyType in project.CostValuesByCommentType[keyName].Keys)
                    {
                        if (keyType == TypeCost.Material)
                        {
                            values = GetValues(project.CostValuesByCommentType, keyName, keyType);
                            note = GetNote(keyName, keyType);
                            ShowTypeCost(reportAllProject, keyType, keyName,
                                         j, values,
                                         note, (null, null));
                            j++;
                        }

                        if (keyType == TypeCost.Service)
                        {
                            values = GetValues(project.PriceValuesByCommentType, keyName, keyType);
                            note = GetNote(keyName, keyType);
                            ShowTypeCost(reportAllProject, keyType, keyName, 
                                         j, values, 
                                         note, (null, null));
                            j++;
                        }
                        
                        if ((keyType == TypeCost.WorkOffice) || (keyType == TypeCost.WorkOnSite))
                        {                            
                            var report = GetAmountWorkAndNote(keyName, keyType);
                            ShowTypeCost(reportAllProject, keyType, keyName,
                                         j, values,
                                         null, report);                           
                            j++;
                        }                                                        
                    }
                }
            }
            listReport.Add(reportAllProject);
        }

        public void MakeFinancialReport(string folderName, string fileNameReport)
        {
            Values values;
            string filename = PrepareReports(folderName, fileNameReport);

            StringBuilder reportAllProject = new StringBuilder();
            reportAllProject.AppendLine(filename);

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

            int i = 1;
            foreach (var keyType in project.CostValuesByType.Keys)
            {
                values = GetValues(project.CostValuesByType, keyType);
                reportAllProject.AppendLine($"###### {i}. {TypeObject.GetTypeObject(keyType)} ######");
                reportAllProject.AppendLine(Cost.ShowCostValues(values));

                values = GetValues(project.PriceValuesByType, keyType);
                reportAllProject.AppendLine($"Рентабельность фактическая: {project.Kprof[keyType] * 100:0} %");
                reportAllProject.AppendLine(Cost.ShowPriceValues(values));

                i++;
            }

            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine("###### Группировка по центрам затрат ######");
            reportAllProject.AppendLine(new string('#', 50));
            reportAllProject.AppendLine();

            i = 1;
            foreach (var group in project.ListNameGroupCost)
            {
                reportAllProject.AppendLine($"###### {i}. {group.Name} ######");
                reportAllProject.AppendLine(Cost.ShowCostValues(group.CostValues));
                reportAllProject.AppendLine(Cost.ShowPriceValues(group.PriceValues));
                i++;
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
