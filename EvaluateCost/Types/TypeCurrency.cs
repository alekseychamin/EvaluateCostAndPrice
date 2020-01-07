using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class CurrencyNameValue : IGetTypeObject, IGetTypeEnumObject<TypeCurrency>
    {        
        public string Name { get; set; }
        public double Value { get; set; }
        public Func<Enum, string> GetTypeObject { get ; set; }
        public TypeCurrency TypeEnumObject { get; set; }
    }    
    
    enum TypeCurrency { None, Rub, Usd, Eur };
}
