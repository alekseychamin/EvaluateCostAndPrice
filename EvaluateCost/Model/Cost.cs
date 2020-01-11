using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvaluateCost
{
    class StringProperty<T>
    {
        public T Value { get; set; }
        public string Name { get; set; }
    }
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
        protected StringProperty<double?> unitTaxCost;
        protected StringProperty<double?> unitNoTaxCost;

        protected double? koefCurrency = 1;
        // ставка НДС значение        
        protected double? costTax;
        protected double? socialTax;
        // тип валюты
        protected TypeCurrency currency;

        // тип затраты
        protected TypeCost typeCost;

        // поле количество затраты
        protected StringProperty<double?> count;

        // поле для указания части системы (при расчете стоимости аналогично сводным таблицам)
        protected StringProperty<string> partSystem;
        // поле для указания комментария
        protected StringProperty<string> comment;
        // региональный коэффициент, учет скидки на материалы, ставка налога для ФОТ
        protected StringProperty<double?> koef;

        // количество человек
        protected StringProperty<double?> countHuman;

        public Values CostValues => cost;
        public Values PriceValues => price;
        public virtual StringProperty<string> Name { get; set; }
        public StringProperty<double?> CountHuman { get => countHuman; set => countHuman = value; }
        public StringProperty<double?> UnitTaxCost
        {
            get => unitTaxCost;
            set
            {
                IsValidDoubleValue(value.Value, "UnitTaxCost");
                unitTaxCost = value;
                EvaluateCost();
                OnChangeValue(new ChangeValueEventArgs(value.Value));
            }
        }
        public StringProperty<double?> UnitNoTaxCost
        {
            get => unitNoTaxCost;
            set
            {
                IsValidDoubleValue(value.Value, "UnitNoTaxCost");
                unitNoTaxCost = value;
                EvaluateCost();
                OnChangeValue(new ChangeValueEventArgs(value.Value));
            }
        }
        public StringProperty<double?> Count
        {
            get => count;
            set
            {
                IsValidDoubleValue(value.Value, "Count");
                count = value;
                EvaluateCost();
                OnChangeValue(new ChangeValueEventArgs(value.Value));
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
        public virtual StringProperty<string> Comment { get => comment; set => comment = value; }
        public virtual StringProperty<string> PartSystem { get => partSystem; set => partSystem = value; }
        public TypeCost TypeEnumObject { get => typeCost; set => typeCost = value; }
        public StringProperty<double?> Koef { get => koef; set => koef = value; }
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
            this.Name.Value = name;
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
                        //string nameTypeCost = TypeObject.GetTypeObject(this.TypeEnumObject);
                        if (this.TypeEnumObject == prof.TypeCost)
                        {
                            this.price.WithNoTax = this.CostValues.WithNoTax / (1 - prof.Value);
                        }
                    }
                }
            }

            this.price.Tax = this.PriceValues.WithNoTax * this.CostTax;
            this.price.WithTax = this.PriceValues.WithNoTax + this.PriceValues.Tax;
            this.price.Currency = this.Currency;
        }

        protected virtual void IsValidDoubleValue(double? value, String paramName)
        {
            if (value.HasValue && value.Value < 0)
                throw new ArgumentOutOfRangeException($"Значение: {value} для параметра {paramName} не допустимо");
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
            if ((unitNoTaxCost != null) && unitNoTaxCost.Value.HasValue)
            {
                unitNoTaxCost.Value = unitNoTaxCost.Value * koefCurrency;
            }
            else if ((unitTaxCost != null) && unitTaxCost.Value.HasValue)
            {
                unitTaxCost.Value = unitTaxCost.Value * koefCurrency;
            }
        }

        private void SetDefValueToStringProperty<T>(ref StringProperty<T> value)
        {
            if (value == null)
            {
                value = new StringProperty<T>();
                value.Value = default(T);
            }            
        }
        public virtual void EvaluateCost()
        {
            SetDefValueToStringProperty<double?>(ref koef);
            if (!koef.Value.HasValue)
                koef.Value = 1;

            SetDefValueToStringProperty<double?>(ref countHuman);
            if (!countHuman.Value.HasValue)
                countHuman.Value = 1;

            cost.Currency = this.Currency;

            if ((unitNoTaxCost != null) && (unitNoTaxCost.Value.HasValue))
            {
                cost.WithNoTax = count?.Value * unitNoTaxCost.Value * koef?.Value * countHuman.Value;
                cost.Tax = cost.WithNoTax * costTax;
                cost.WithTax = cost.WithNoTax + cost.Tax;
            }
            else if ((unitTaxCost != null) && (unitTaxCost.Value.HasValue))
            {
                cost.WithTax = count?.Value * unitTaxCost.Value * koef?.Value * countHuman?.Value;
                cost.Tax = (cost.WithTax * costTax) / (1 + costTax);
                cost.WithNoTax = cost.WithTax - cost.Tax;
            }
        }

        public void GetPriceValuesByType()
        {
            priceValuesByType.Add(this.TypeEnumObject, price);
        }

        public static string ShowCostValues(Values values)
        {
            return string.Format($"Себестоимость без НДС: {values.WithNoTax:N} {values.Currency}\n" +
                                 $"НДС: {values.Tax:N} {values.Currency}\n" +
                                 $"Себестоимость с НДС: {values.WithTax:N} {values.Currency}\n");
        }

        public static string ShowPriceValues(Values values)
        {
            return string.Format($"Цена без НДС: {values.WithNoTax:N} {values.Currency}\n" +
                                 $"НДС: {values.Tax:N} {values.Currency}\n" +
                                 $"Цена с НДС: {values.WithTax:N} {values.Currency}\n");
        }
    }
}
