using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class Evaluate
    {
        public static StringProperty<double?> GetDurationById<T>(List<T> listItem) where T : IGetDurationId
        {
            StringProperty<double?> duration = new StringProperty<double?>();
            duration.Value = 0;

            listItem.Sort();

            var query = from item in listItem
                        group item by item.Id
                        into g
                        select new
                        {
                            Id = g.Key,
                            maxDur = (from c in g select c.Duration.Value).Max(),
                            name = (from c in g select c.Duration.Name).First()
                        };

            foreach (var item in query)
            {
                if (item != null && item.maxDur != null)
                {
                    duration.Value += item.maxDur.Value;
                    duration.Name = item.name ?? null;
                }
            }

            return duration;
        }

        public static StringProperty<double?> GetDuration(Project project, string comment, TypeCost typeCost)
        {
            StringProperty<double?> result = new StringProperty<double?>() { Name = string.Empty, Value = 0 };

            var q = from g in project.ListNameGroupCost
                    from c in g.ListCost
                    where c.Comment.Value.Equals(comment) && c.TypeEnumObject == typeCost
                    select new { id = c.Id, comment = c.Comment.Value, name = c.Comment.Name, value = c.Duration.Value };

            var maxq = from item in q
                       group item by new { item.id }
                       into g
                       select new
                       {
                           id = g.Key.id,
                           maxDur = (from c in g
                                     select c.value).Max()
                       };

            double? sumDur = 0;            
            sumDur = maxq.Sum(d => d.maxDur);

            result.Value = sumDur;

            return result;
        }

        public static Dictionary<string, StringProperty<double?>> GetDurationByCommentId(List<GroupCost> listGroup)
        {
            Dictionary<string, StringProperty<double?>> durationByComment = new Dictionary<string, StringProperty<double?>>();

            foreach (var nameGroup in listGroup)
            {
                nameGroup.ListCost.Sort();

                var q = from cost in nameGroup.ListCost
                        group cost by new { cost.Comment.Value, cost.Id }
                        into g
                        select new
                        {
                            comment = g.Key.Value,
                            id = g.Key.Id,
                            maxDur = (from c in g
                                      select c.Duration.Value).Max(),
                            nameValue = (from c in g select c.Duration.Name).First()
                        };

                var sumQ = from item in q
                           group item by item.comment
                           into g
                           select new
                           {
                               comment = g.Key,
                               sum = g.Sum(x => x.maxDur),
                               name = (from i in g select i.nameValue).First()
                           };

                foreach (var item in sumQ)
                {
                    if (item != null)
                    {
                        StringProperty<double?> duration;

                        if (durationByComment.ContainsKey(item.comment))
                        {
                            duration = durationByComment[item.comment];
                            if (!duration.Value.HasValue) duration.Value = 0;
                            if (!string.IsNullOrEmpty(item.name)) duration.Name = item.name;
                            duration.Value += item.sum;
                            durationByComment[item.comment] = duration;
                        }
                        else
                        {
                            duration = new StringProperty<double?> { Value = item.sum, Name = item.name };
                            durationByComment.Add(item.comment, duration);
                        }
                    }
                }
            }

            return durationByComment;
        }
    }
}
