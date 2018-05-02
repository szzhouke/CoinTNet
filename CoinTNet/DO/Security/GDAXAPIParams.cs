﻿
namespace CoinTNet.DO.Security
{
    /// <summary>
    /// Holds the parameters for connecting to BTC-e's private API
    /// </summary>
    public class GDAXAPIParams
    {
        /// <summary>
        /// Gets or sets the API key
        /// </summary>
        public string APIKey { get; set; }
        /// <summary>
        /// Gets or sets the API secret
        /// </summary>
        public string APISecret { get; set; }
        /// <summary>
        /// Gets or sets the API passphrase
        /// </summary>
        public string APIPassphrase { get; set; }
    }
}
