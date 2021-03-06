﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    struct Values
    {
        public double? WithNoTax;
        public double? Tax;
        public double? WithTax;
        public TypeCurrency Currency;        
    }
    interface ICPValues
    {
        Values CostValues { get; }
        Values PriceValues { get; }
        double? Kprofitability { get; set; }

        Dictionary<TypeCost, Values> CostValuesByType { get; }
        Dictionary<TypeCost, Values> PriceValuesByType { get; }
    }
}
