using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    interface IGetDurationId : IComparable
    {
        string Id { get; set; }
        StringProperty<double?> Duration { get; set; }        
    }
}
