using Newtonsoft.Json.Linq;

namespace GDAXAPI
{
    /// <summary>
    /// Represents the user's balance
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// The amount of available USD for orders
        /// </summary>
        public decimal AvailableUSD { get; private set; }
        /// <summary>
        /// The amount of available BTC for orders
        /// </summary>
        public decimal AvailableBTC { get; private set; }
        /// <summary>
        /// The total amount of USD
        /// </summary>
        public decimal BalanceUSD { get; private set; }
        /// <summary>
        /// The total amount of BTC
        /// </summary>
        public decimal BalanceBTC { get; private set; }
        /// <summary>
        /// Amount of USD reserved in open orders
        /// </summary>
        public decimal ReservedUSD { get; private set; }
        /// <summary>
        /// Amount of BTC reserved in open orders
        /// </summary>
        public decimal ReservedBTC { get; private set; }
        /// <summary>
        /// The fee
        /// </summary>
        public decimal Fee { get; private set; }

        public static Balance CreateFromJArray(JArray a)
        {
            if (a == null)
            {
                return null;
            }

            var r = new Balance()
            {
                BalanceUSD = a.Value<decimal>("usd_balance"),
                BalanceBTC = a.Value<decimal>("btc_balance"),
                AvailableUSD = a.Value<decimal>("usd_available"),
                AvailableBTC = a.Value<decimal>("btc_available"),
                ReservedUSD = a.Value<decimal>("usd_reserved"),
                ReservedBTC = a.Value<decimal>("btc_reserved"),
                Fee = a.Value<decimal>("fee")
            };

            return r;
        }

    }
}
