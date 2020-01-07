using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class ChangeValueEventArgs : EventArgs
    {
        private double? m_Value;
        public ChangeValueEventArgs(double? value)
        {
            this.m_Value = value;
        }
        public double? NewValue { get => m_Value; }
    }
    interface IChangeValue
    {
        event EventHandler<ChangeValueEventArgs> ChangeValue;
    }
}
