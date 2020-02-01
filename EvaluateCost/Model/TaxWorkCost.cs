using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class CostTaxWork : Cost
    {
        private Cost workCost;

        public override StringProperty<string> Name
        {
            get
            {
                name.Value = "Налог на " + workCost?.Name.Value;
                name.Name = workCost?.Name.Name ?? null;
                return name;
            }
        }

        public override StringProperty<string> PartSystem
        {
            get
            {
                return workCost?.PartSystem ?? null;
            }
        }

        public override StringProperty<string> Comment
        {
            get
            {
                return workCost?.Comment ?? null;
            }
        }

        public override TypeCurrency Currency
        {
            get
            {
                return workCost?.Currency ?? TypeCurrency.None;
            }
        }

        public CostTaxWork(Cost wCost)
        {
            workCost = wCost;                        
            this.TypeEnumObject = TypeCost.Tax;
            this.socialTax = wCost.SocialTax;
            this.id = wCost.Id;
            //this.Currency = wCost.Currency;
            //this.Name = "Налог на " + wCost.Name;
            //this.Comment = wCost.Comment;
            //this.PartSystem = wCost.PartSystem;
            this.GetTypeObject = wCost.GetTypeObject;
        }

        public override void EvaluateCost()
        {
            if (workCost != null)
            {
                //workCost.EvaluateCost();
                cost.WithNoTax = this.socialTax * workCost.CostValues.WithNoTax;
                cost.Tax = 0;
                cost.WithTax = cost.WithNoTax;
            }
        }

        public override string ToString()
        {
            return String.Format($"Тип затраты: {this.GetTypeObject(TypeEnumObject)}\n" +
                                 $"Имя затраты: {Name}\n" +                                 
                                 $"Ставка налога: {Koef}\n") +
                                 Cost.ShowCostValues(this.CostValues);
        }
    }
}
