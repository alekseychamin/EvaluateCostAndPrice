using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class Evaluate
    {
        protected Values cost;
        protected Values price;
        
        private static Dictionary<TypeCost, Values> costValuesByType = new Dictionary<TypeCost, Values>();
        private static Dictionary<TypeCost, Values> priceValuesByType = new Dictionary<TypeCost, Values>();
        public Values CostValues => cost;
        public Values PriceValues => price;
        
        public Dictionary<TypeCost, Values> PriceValuesByType => priceValuesByType;
        public Dictionary<TypeCost, Values> CostValuesByType => costValuesByType;
        

        public void GetCostValues()
        {
            throw new NotImplementedException();
        }

        public void GetPriceValues()
        {
            throw new NotImplementedException();
        }

        public static void GetCostValuesByType(GroupCost obj)
        {
            foreach (var item in obj.ListCost)
            {
                if (obj.CostValuesByType.ContainsKey(item.TypeEnumObject))
                {
                    Values cost = obj.CostValuesByType[item.TypeEnumObject];
                    cost.Tax += item.CostValues.Tax;
                    cost.WithNoTax += item.CostValues.WithNoTax;
                    cost.WithTax += item.CostValues.WithTax;
                    obj.CostValuesByType[item.TypeEnumObject] = cost;
                }
                else                
                    obj.CostValuesByType.Add(item.TypeEnumObject, item.CostValues);                
            }
        }
        public void GetPriceValuesByType()
        {
            throw new NotImplementedException();
        }
    }
}
