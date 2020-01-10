using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{    
    /// <summary>
    /// Перечисление для указания типа затрат 
    /// </summary>
    enum TypeCost { Work, Tax, Material, Other, Service };

    /// <summary>
    /// структура для хранения типа объекта GetType() и наименования типа затраты для UI
    /// </summary>
    class TypeCostName
    {
        // поле для хранения типа затарты typeof(Cost)
        public Type SystemType { get; set; }
        public string Name { get; set; }
    }
    
}
