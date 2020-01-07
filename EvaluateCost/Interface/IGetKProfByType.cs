using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    interface IGetKprofByType
    {
        Dictionary<TypeCost, double?> Kprof { get; }
        void GetKprofByType();
    }
}
