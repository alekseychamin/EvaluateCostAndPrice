using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class Evaluate
    {

        public static void GetWorkerInfoByCommetTypeCost(List<GroupCost> listGroupCost,
                  Dictionary<string, Dictionary<TypeCost, List<WorkerInfo>>> workerInfoByCommentTypeCost)
        {
            List<WorkerInfo> CreateListWorkerInfo(string nameWorker, double maxCount, double workHours, double maxDuration)
            {
                List<WorkerInfo> listWorkerInfo = new List<WorkerInfo>();
                WorkerInfo workerInfo = new WorkerInfo(nameWorker: nameWorker, maxCount: maxCount,
                                                       workHours: workHours, maxDuration: maxDuration);
                listWorkerInfo.Add(workerInfo);
                return listWorkerInfo;
            }
            var q = listGroupCost.
                SelectMany(x => x.ListCost).Where(t => !string.IsNullOrEmpty(t.NameWorker) && t.CountHuman.Value.HasValue).
                GroupBy(g =>
                new
                {
                    comment = g.Comment.Value,
                    typeCost = g.TypeEnumObject,
                    nameWorker = g.NameWorker
                }).
                Select(i =>
                new
                {
                    comment = i.Key.comment,
                    typeCost = i.Key.typeCost,
                    nameWorker = i.Key.nameWorker,
                    maxCount = i.Max(t => t.CountHuman.Value),
                    workHours = i.Sum(t => t.Count.Value * t.CountHuman.Value),
                    maxDuration = i.Max(t => t.Duration.Value)
                });

            //foreach (var item in q)
            //    Console.WriteLine($"item.nameWorker = {item.nameWorker}, item.maxCount = {item.maxCount}" +
            //        $"item.comment = {item.comment}, item.typeCost = {item.typeCost}");

            foreach (var item in q)
            {
                if (workerInfoByCommentTypeCost.ContainsKey(item.comment))
                {
                    if (workerInfoByCommentTypeCost[item.comment].ContainsKey(item.typeCost))
                    {
                        WorkerInfo workerInfo = new WorkerInfo(nameWorker: item.nameWorker, maxCount: item.maxCount.Value,
                                                               workHours: item.workHours.Value, maxDuration: item.maxDuration.Value);
                        workerInfoByCommentTypeCost[item.comment][item.typeCost].Add(workerInfo);
                    }
                    else
                    {
                        List<WorkerInfo> listWorkerInfo = CreateListWorkerInfo(item.nameWorker, item.maxCount.Value,
                                                                           item.workHours.Value, item.maxDuration.Value);
                        workerInfoByCommentTypeCost[item.comment].Add(item.typeCost, listWorkerInfo);
                    }
                    
                }
                else
                {
                    workerInfoByCommentTypeCost.Add(item.comment, new Dictionary<TypeCost, List<WorkerInfo>>());

                    List<WorkerInfo> listWorkerInfo = CreateListWorkerInfo(item.nameWorker, item.maxCount.Value,
                                                                           item.workHours.Value, item.maxDuration.Value);

                    workerInfoByCommentTypeCost[item.comment].Add(item.typeCost, listWorkerInfo);

                }
            }
        }
        public static void GetValuesByPartSystem(List<GroupCost> listGroupCost,
                                                 Dictionary<string, Values> costValuesByPartSystem,
                                                 Dictionary<string, Values> priceValuesByPartSystem)
        {
            var q = listGroupCost.
                SelectMany(x => x.ListCost).
                GroupBy(g => g.PartSystem.Value).
                Select(i =>
                new
                {
                    partSystem = i.Key,
                    costValues = new Values(withNoTax: i.Select(t => t.CostValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.CostValues.Tax).Sum(),
                                            withTax: i.Select(t => t.CostValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault()),
                    priceValues = new Values(withNoTax: i.Select(t => t.PriceValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.PriceValues.Tax).Sum(),
                                            withTax: i.Select(t => t.PriceValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault())
                });

            foreach (var item in q)
            {
                costValuesByPartSystem.Add(item.partSystem, item.costValues);
                priceValuesByPartSystem.Add(item.partSystem, item.priceValues);
            }
        }

        public static void GetValuesByPartSystemTypeCost(List<GroupCost> listGroupCost,
                                                  Dictionary<string, Dictionary<TypeCost, Values>> costValuesByPartSystemTypeCost,
                                                  Dictionary<string, Dictionary<TypeCost, Values>> priceValuesByPartSystemTypeCost)
        {
            var q = listGroupCost.
                SelectMany(x => x.ListCost).
                GroupBy(g =>
                new
                {
                    partSystem = g.PartSystem.Value,
                    typeCost = g.TypeEnumObject
                }).
                Select(i =>
                new
                {
                    partSystem = i.Key.partSystem,
                    typeCost = i.Key.typeCost,
                    costValues = new Values(withNoTax: i.Select(t => t.CostValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.CostValues.Tax).Sum(),
                                            withTax: i.Select(t => t.CostValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault()),
                    priceValues = new Values(withNoTax: i.Select(t => t.PriceValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.PriceValues.Tax).Sum(),
                                            withTax: i.Select(t => t.PriceValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault())
                });

            foreach (var item in q)
            {
                if (costValuesByPartSystemTypeCost.ContainsKey(item.partSystem))
                {
                    costValuesByPartSystemTypeCost[item.partSystem].Add(item.typeCost, item.costValues);
                }
                else
                {
                    costValuesByPartSystemTypeCost.Add(item.partSystem, new Dictionary<TypeCost, Values>());
                    costValuesByPartSystemTypeCost[item.partSystem].Add(item.typeCost, item.costValues);
                }

                if (priceValuesByPartSystemTypeCost.ContainsKey(item.partSystem))
                {
                    priceValuesByPartSystemTypeCost[item.partSystem].Add(item.typeCost, item.priceValues);
                }
                else
                {
                    priceValuesByPartSystemTypeCost.Add(item.partSystem, new Dictionary<TypeCost, Values>());
                    priceValuesByPartSystemTypeCost[item.partSystem].Add(item.typeCost, item.priceValues);
                }
            }
        }
        public static void GetValuesByCommentPartSystem(List<GroupCost> listGroupCost,
                                                        Dictionary<string, Dictionary<string, Values>> costValuesByCommentPart,
                                                        Dictionary<string, Dictionary<string, Values>> priceValuesByCommentPart)
        {
            var q = listGroupCost.
                SelectMany(x => x.ListCost).
                GroupBy(g =>
                new
                {
                    comment = g.Comment.Value,
                    partSystem = g.PartSystem.Value
                }).
                Select(i =>
                new
                {
                    comment = i.Key.comment,
                    partSystem = i.Key.partSystem,
                    costValues = new Values(withNoTax: i.Select(t => t.CostValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.CostValues.Tax).Sum(),
                                            withTax: i.Select(t => t.CostValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault()),
                    priceValues = new Values(withNoTax: i.Select(t => t.PriceValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.PriceValues.Tax).Sum(),
                                            withTax: i.Select(t => t.PriceValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault())
                });

            //foreach (var item in q)
            //{
            //    Console.WriteLine($"item.comment = {item.comment}, item.partSystem = {item.partSystem}");
            //}

            foreach (var item in q)
            {
                if (costValuesByCommentPart.ContainsKey(item.comment))
                {
                    costValuesByCommentPart[item.comment].Add(item.partSystem, item.costValues);
                }
                else
                {
                    costValuesByCommentPart.Add(item.comment, new Dictionary<string, Values>());
                    costValuesByCommentPart[item.comment].Add(item.partSystem, item.costValues);
                }

                if (priceValuesByCommentPart.ContainsKey(item.comment))
                {
                    priceValuesByCommentPart[item.comment].Add(item.partSystem, item.priceValues);
                }
                else
                {
                    priceValuesByCommentPart.Add(item.comment, new Dictionary<string, Values>());
                    priceValuesByCommentPart[item.comment].Add(item.partSystem, item.priceValues);
                }
            }
        }
        public static void GetValuesByType(List<GroupCost> listGroupCost,
                                           Dictionary<TypeCost, Values> costValuesByType,
                                           Dictionary<TypeCost, Values> priceValuesByType)
        {
            var q = listGroupCost.
                SelectMany(x => x.ListCost).
                GroupBy(g => g.TypeEnumObject).
                Select(i =>
                new
                {
                    typeCost = i.Key,
                    costValues = new Values(withNoTax: i.Select(t => t.CostValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.CostValues.Tax).Sum(),
                                            withTax: i.Select(t => t.CostValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault()),
                    priceValues = new Values(withNoTax: i.Select(t => t.PriceValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.PriceValues.Tax).Sum(),
                                            withTax: i.Select(t => t.PriceValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault())
                });

            //foreach (var item in q)
            //    Console.WriteLine($"item.typeCost = {item.typeCost}, item.costValues = {item.costValues.WithNoTax}");

            foreach (var item in q)
            {
                costValuesByType.Add(item.typeCost, item.costValues);
                priceValuesByType.Add(item.typeCost, item.priceValues);
            }
        }
        public static void GetValuesByComment(List<GroupCost> listGroupCost,
                                              Dictionary<string, Values> costValuesByComment,
                                              Dictionary<string, Values> priceValuesByComment)
        {
            var q = listGroupCost.
                SelectMany(x => x.ListCost).
                GroupBy(g => g.Comment.Value).
                Select(i =>
                new
                {
                    comment = i.Key,
                    costValues = new Values(withNoTax: i.Select(t => t.CostValues.WithNoTax).Sum(),
                                             tax: i.Select(t => t.CostValues.Tax).Sum(),
                                             withTax: i.Select(t => t.CostValues.WithTax).Sum(),
                                             typeCurrency: i.Select(t => t.Currency).FirstOrDefault()),
                    priceValues = new Values(withNoTax: i.Select(t => t.PriceValues.WithNoTax).Sum(),
                                             tax: i.Select(t => t.PriceValues.Tax).Sum(),
                                             withTax: i.Select(t => t.PriceValues.WithTax).Sum(),
                                             typeCurrency: i.Select(t => t.Currency).FirstOrDefault())
                });

            foreach (var item in q)
            {
                costValuesByComment.Add(item.comment, item.costValues);
                priceValuesByComment.Add(item.comment, item.priceValues);
            }
        }
        public static void GetValuesByCommentTypeCost(List<GroupCost> listGroupCost,
            Dictionary<string, Dictionary<TypeCost, Values>> costValuesByCommentType,
            Dictionary<string, Dictionary<TypeCost, Values>> priceValuesByCommentType)
        {

            var q = listGroupCost.
                SelectMany(x => x.ListCost).
                GroupBy(g =>
                new
                {
                    g.Comment.Value,
                    g.TypeEnumObject
                }).
                Select(i =>
                new
                {
                    comment = i.Key.Value,
                    typeCost = i.Key.TypeEnumObject,
                    costValues = new Values(withNoTax: i.Select(t => t.CostValues.WithNoTax).Sum(),
                                            tax: i.Select(t => t.CostValues.Tax).Sum(),
                                            withTax: i.Select(t => t.CostValues.WithTax).Sum(),
                                            typeCurrency: i.Select(t => t.Currency).FirstOrDefault()),
                    priceValues = new Values(withNoTax: i.Select(t => t.PriceValues.WithNoTax).Sum(),
                                             tax: i.Select(t => t.PriceValues.Tax).Sum(),
                                             withTax: i.Select(t => t.PriceValues.WithTax).Sum(),
                                             typeCurrency: i.Select(t => t.Currency).FirstOrDefault())
                });



            foreach (var item in q)
            {
                if (costValuesByCommentType.ContainsKey(item.comment))
                {
                    costValuesByCommentType[item.comment].Add(item.typeCost, item.costValues);
                }
                else
                {
                    costValuesByCommentType.Add(item.comment, new Dictionary<TypeCost, Values>());
                    costValuesByCommentType[item.comment].Add(item.typeCost, item.costValues);
                }

                if (priceValuesByCommentType.ContainsKey(item.comment))
                {
                    priceValuesByCommentType[item.comment].Add(item.typeCost, item.priceValues);
                }
                else
                {
                    priceValuesByCommentType.Add(item.comment, new Dictionary<TypeCost, Values>());
                    priceValuesByCommentType[item.comment].Add(item.typeCost, item.priceValues);
                }
            }
        }
        public static StringProperty<double?> GetDurationById<T>(List<T> listItem) where T : IGetDurationId
        {
            StringProperty<double?> duration = new StringProperty<double?>();
            duration.Value = 0;

            listItem.Sort();

            var sum = listItem.
                GroupBy(g => g.Id).
                Sum(g => g.Max(t => t.Duration.Value));

            var name = listItem.FirstOrDefault(t => !string.IsNullOrEmpty(t.Duration.Name));

            duration.Value = sum.Value;
            duration.Name = name?.Duration.Name ?? null;

            return duration;
        }

        public static StringProperty<double?> GetDuration(Project project, string comment, TypeCost typeCost)
        {
            StringProperty<double?> result = new StringProperty<double?>() { Name = string.Empty, Value = 0 };

            var sum = project.ListNameGroupCost.SelectMany(t => t.ListCost).
                Where(x => x.Comment.Value.Equals(comment) && x.TypeEnumObject == typeCost).
                GroupBy(i => i.Id).
                Sum(g => g.Max(t => t.Duration.Value));

            result.Value = sum.Value;

            return result;
        }

        public static Dictionary<string, StringProperty<double?>> GetDurationByCommentId(List<GroupCost> listGroup)
        {
            //Dictionary<string, StringProperty<double?>> durationByComment = new Dictionary<string, StringProperty<double?>>();

            listGroup.ForEach(x => x.ListCost.Sort());

            var sum = listGroup.SelectMany(x => x.ListCost).
                GroupBy(g =>
                new
                {
                    g.Comment.Value,
                    g.GroupCost
                }).
                Select(i =>
                new
                {
                    comment = i.Key.Value,
                    group = i.Key.GroupCost,
                    sumMaxDur = i.GroupBy(x => new { x.Id }).Sum(g => g.Max(t => t.Duration.Value)),
                    nameValue = (from c in i select c.Duration.Name).FirstOrDefault()
                }).
                GroupBy(g => g.comment).
                Select(i =>
                new
                {
                    comment = i.Key,
                    sum = i.Sum(t => t.sumMaxDur),
                    nameValue = i.Select(t => t.nameValue).FirstOrDefault()
                }).ToDictionary(key => key.comment, value => new StringProperty<double?>()
                { Value = value.sum, Name = value.nameValue });

            return sum;
        }
    }
}
