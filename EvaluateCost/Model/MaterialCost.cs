using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class MaterialCost : Cost
    {
        public MaterialCost()
        {

        }
        public MaterialCost(string name, TypeCost typeCost,
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
                                 $"Стоимость единицы без НДС: {UnitNoTaxCost:N} {Currency}\n" +
                                 $"Общая стоимость материала без НДС: {CostValues.WithNoTax:N} {Currency}\n" +
                                 $"НДС: {CostValues.Tax:N} {Currency}\n" +
                                 $"Общая стоимость материала с НДС: {CostValues.WithTax:N} {Currency}");
        }
    }
}
