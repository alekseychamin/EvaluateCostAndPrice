using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class CostMaterial : Cost
    {
        public CostMaterial()
        {

        }
        public CostMaterial(string name, TypeCost typeCost,
                            Func<Cost, string> getTypeCostName) : base(name, typeCost)
        {

        }
        //protected override void EvaluateCost()
        //{
        //    costTaxPrice = UnitTaxCost * Count;
        //}
        public override string ToString()
        {
            return String.Format($"Тип затраты: {this.GetTypeObject(TypeEnumObject)}\n" +
                                 $"Имя затраты: {Name}\n" +
                                 $"Количество единиц: {Count}\n" +
                                 $"Стоимость единицы с НДС: {UnitTaxCost:N} {Currency}\n" +
                                 $"Стоимость единицы без НДС: {UnitNoTaxCost:N} {Currency}\n") +
                                 Cost.ShowCostValues(this.CostValues);
        }
    }
}
