using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    interface IGetTypeObject
    {
        /// <summary>
        /// свойство делегат для передачи метода возращающего название типа объекта, согласно типу Enum        
        /// </summary>
        Func<Enum, string> GetTypeObject { get; set; }
    }
}
