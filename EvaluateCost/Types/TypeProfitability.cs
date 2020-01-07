using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class Profitability : IGetTypeEnumObject<TypeProfitability>, IGetTypeObject
    {
        public string Name { get; set; }
        public double? Value { get; set; }
        public TypeProfitability TypeEnumObject { get; set; }
        public Func<Enum, string> GetTypeObject { get; set; }

    }
    public enum TypeProfitability
    {
        MainProfitability
    }
}
