using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class ServiceCost : Cost
    {
        public override string ToString()
        {
            return String.Format($"Тип затраты: {this.GetTypeObject(TypeEnumObject)}\n" +
                                 $"Имя затраты: {Name}\n" +
                                 $"Количество ед.: {Count}\n" +
                                 $"Стоимость ед. с НДС: {UnitTaxCost:N} {Currency}\n" +
                                 $"Стоимость ед. без НДС: {UnitNoTaxCost:N} {Currency}\n") +
                                 Cost.ShowCostValues(this.CostValues);
        }
    }
}
