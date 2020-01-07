using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    /// <summary>
    /// свойство для хранения типа объекта (затрата, валюта и т.д.)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IGetTypeEnumObject<T> where T : struct
    {
        T TypeEnumObject { get; set; }
    }
}
