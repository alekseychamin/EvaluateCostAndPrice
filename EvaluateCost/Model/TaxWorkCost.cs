using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class TaxWorkCost : Cost
    {
        private Cost workCost;

        public override string Name
        {
            get
            {                
                return "Налог на " + workCost?.Name;
            }
        }

        public override string PartSystem
        {
            get
            {
                return workCost?.PartSystem;
            }
        }

        public override string Comment
        {
            get
            {
                return workCost?.Comment;
            }
        }

        public override TypeCurrency Currency
        {
            get
            {
                return workCost?.Currency ?? TypeCurrency.None;
            }
        }

        public TaxWorkCost(Cost wCost)
        {
            workCost = wCost;                        
            this.TypeEnumObject = TypeCost.Tax;
            this.socialTax = wCost.SocialTax;
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
