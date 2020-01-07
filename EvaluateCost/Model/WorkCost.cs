﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class CostWork : Cost
    {
        private TaxWorkCost taxWorkCost;

        public TaxWorkCost TaxWorkCost { get => taxWorkCost; }
        public CostWork()
        {

        }
        public CostWork(string name, TypeCost typeCost, 
                              Func<Cost, string> getTypeCostName) : base(name, typeCost)
        {

        }
        public TaxWorkCost AddTaxWorkCost()
        {
            if (taxWorkCost == null)
            {
                TaxWorkCost taxWC = new TaxWorkCost(this);
                taxWorkCost = taxWC;
                return taxWC;
            }
            return null;
        }
        public override void EvaluateCost()
        {
            base.EvaluateCost();
            cost.Tax = 0;

            if (UnitNoTaxCost.HasValue)                                         
                cost.WithTax = cost.WithNoTax;            
            else if (UnitTaxCost.HasValue)                                            
                cost.WithNoTax = cost.WithTax;

            taxWorkCost?.EvaluateCost();
        }

        public override string ToString()
        {
            return String.Format($"Тип затраты: {this.GetTypeObject(TypeEnumObject)}\n" +
                                 $"Имя затраты: {Name}\n" +
                                 $"Количество часов: {Count}\n" +
                                 $"Стоимость часа с НДС: {UnitTaxCost:N} {Currency}\n" +
                                 $"Стоимость часа без НДС: {UnitNoTaxCost:N} {Currency}\n" +
                                 $"Общая стоимость работ без НДС: {CostValues.WithNoTax:N} {Currency}\n" +
                                 $"НДС: {CostValues.Tax:N} {Currency}\n" +
                                 $"Общая стоимость работ с НДС: {CostValues.WithTax:N} {Currency}");
        }
    }
}