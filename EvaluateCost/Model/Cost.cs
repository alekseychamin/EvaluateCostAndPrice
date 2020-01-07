using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvaluateCost
{
    /// <summary>
    /// класс для описания абстрактной затраты
    /// </summary>
    abstract class Cost : IGetTypeEnumObject<TypeCost>, 
                          IChangeValue, IGetTypeObject,
                          ITax, ICPValues, IGetPValuesByType
    {
        // поле себестоиомости затраты, выполняется пересчет в производных классах        

        protected Values cost;
        protected Values price;
        private Dictionary<TypeCost, Values> costValuesByType = new Dictionary<TypeCost, Values>();
        private Dictionary<TypeCost, Values> priceValuesByType = new Dictionary<TypeCost, Values>();
        // поле стоимости ед. затраты
        protected double? unitTaxCost;
        protected double? unitNoTaxCost;
        protected double? koefCurrency = 1;
        // НДС значение        
        protected double? costTax;
        protected double? socialTax;
        // тип валюты
        protected TypeCurrency currency;
        // тип затраты
        protected TypeCost typeCost;
        // поле количество затраты
        protected double? count;
        // поле для указания части системы (при расчете стоимости аналогично сводным таблицам)
        protected string partSystem;
        // поле для указания комментария
        protected string comment;
        // региональный коэффициент, учет скидки на материалы, ставка налога для ФОТ
        protected double? koef = 1;

        public Values CostValues => cost;
        public Values PriceValues => price;
        public virtual string Name { get; set; }
        public double? UnitTaxCost
        {
            get => unitTaxCost;
            set
            {
                IsValidDoubleValue(value, "UnitTaxCost");
                SetValue(value, ref unitTaxCost);
            }
        }
        public double? UnitNoTaxCost
        {
            get => unitNoTaxCost;
            set
            {
                IsValidDoubleValue(value, "UnitNoTaxCost");
                SetValue(value, ref unitNoTaxCost);
            }
        }
        public double? Count
        {
            get => count;
            set
            {
                IsValidDoubleValue(value, "Count");
                SetValue(value, ref count);
            }
        }
        public double? CostTax
        {
            get => costTax;
            set
            {
                if (value <= 0)
                    costTax = 0.2;
                else
                    costTax = value;
            }
        }
        public double? SocialTax
        {
            get => socialTax;
            set
            {
                if (value < 0)
                    socialTax = 0;
                else
                    socialTax = value;
            }
        }
        public double? KoefCurrency
        {
            get => koefCurrency;
            set
            {
                if (value > 0)
                {
                    koefCurrency = value;
                    ChangeUnitCostCurrency();
                    EvaluateCost();
                }
            }
        }
        public virtual TypeCurrency Currency { get => currency; set => currency = value; }
        public virtual string Comment { get => comment; set => comment = value; }
        public virtual string PartSystem { get => partSystem; set => partSystem = value; }
        public TypeCost TypeEnumObject { get => typeCost; set => typeCost = value; }
        public double? Koef { get => koef; set => koef = value; }
        public Func<Enum, string> GetTypeObject { get; set; }
        public double? Kprofitability { get; set; }
        public Dictionary<TypeCost, Values> CostValuesByType => costValuesByType;
        public Dictionary<TypeCost, Values> PriceValuesByType => priceValuesByType;

        /// <summary>
        /// поле события класса уведомляющее об изменении стоимости затраты
        /// </summary>
        public event EventHandler<ChangeValueEventArgs> ChangeValue;


        public Cost()
        {

        }
        public Cost(string name, TypeCost typeCost)
        {
            this.Name = name;
            this.TypeEnumObject = typeCost;
        }
        /// <summary>
        /// метод проверки корректности значения value перед записью в поле класса
        /// </summary>
        /// <param name="value">значение для провеоки</param>
        /// <param name="paramName">имя параметра для формирования сообщения исключения</param>        

        public void GetPriceValues(List<Profitability> listProf = null)
        {
            if (this.Kprofitability.HasValue)
                this.price.WithNoTax = this.CostValues.WithNoTax / (1 - this.Kprofitability);
            else
            {
                if (listProf != null)
                {
                    foreach (var prof in listProf)
                    {
                        string nameTypeCost = TypeObject.GetTypeObject(this.TypeEnumObject);
                        if (nameTypeCost == prof.Name)
                        {
                            this.price.WithNoTax = this.CostValues.WithNoTax / (1 - prof.Value);
                        }
                    }
                }
            }

            this.price.Tax = this.PriceValues.WithNoTax * this.CostTax;
            this.price.WithTax = this.PriceValues.WithNoTax + this.PriceValues.Tax;
        }

        protected virtual void IsValidDoubleValue(double? value, String paramName)
        {
            if (value.HasValue && value.Value < 0)
                throw new ArgumentOutOfRangeException($"Значение: {value} для параметра {paramName} не допустимо");
        }
        /// <summary>
        /// метод записи входного значения в поле класса
        /// </summary>
        /// <param name="inputValue">входное значение для записи в поле</param>
        /// <param name="outValue">переменная поля класса для записи</param>
        private void SetValue(double? inputValue, ref double? outValue)
        {
            outValue = inputValue;
            EvaluateCost();
            OnChangeValue(new ChangeValueEventArgs(outValue));
        }
        
        /// <summary>
        /// метод вызывающий событие
        /// </summary>
        protected virtual void OnChangeValue(ChangeValueEventArgs e)
        {
            EventHandler<ChangeValueEventArgs> temp = Volatile.Read(ref ChangeValue);
            //if (temp != null) temp(this, e);
            temp?.Invoke(this, e);
        }
        
        /// <summary>
        /// метод для пересчета стоимости затраты, реализовывается в производных классах
        /// </summary>
        private void ChangeUnitCostCurrency()
        {
            if (unitNoTaxCost.HasValue)
            {
                unitNoTaxCost = unitNoTaxCost * koefCurrency;
            }
            else if (unitTaxCost.HasValue)
            {
                unitTaxCost = unitTaxCost * koefCurrency;
            }
        }
        public virtual void EvaluateCost()
        {
            if (!koef.HasValue)
                koef = 1;

            if (unitNoTaxCost.HasValue)
            {
                cost.WithNoTax = count * unitNoTaxCost * koef;
                cost.Tax = cost.WithNoTax * costTax;
                cost.WithTax = cost.WithNoTax + cost.Tax;
            }
            else if (unitTaxCost.HasValue)
            {
                cost.WithTax = count * unitTaxCost * koef;
                cost.Tax = (cost.WithTax * costTax) / (1 + costTax);
                cost.WithNoTax = cost.WithTax - cost.Tax;
            }
        }

        public void GetPriceValuesByType()
        {
            priceValuesByType.Add(this.TypeEnumObject, price);
        }
    }
}
