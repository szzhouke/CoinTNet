using CoinTNet.Common.Constants;
using CoinTNet.DAL;
using CoinTNet.DO.Security;
using CoinTNet.UI.Common;
using CoinTNet.UI.Common.EventAggregator;
using System.Windows.Forms;

namespace CoinTNet.UI.Controls.Options
{
    /// <summary>
    /// Enables the user to enter the authentication info for Btc-e's API
    /// </summary>
    public partial class GDAXKeysControl : UserControl, Interfaces.IOptionControl
    {
        /// <summary>
        /// The previous key values
        /// </summary>
        private GDAXAPIParams _apiParams;

        /// <summary>
        /// Initialises a new instance of the class
        /// </summary>
        public GDAXKeysControl()
        {
            InitializeComponent();

            _apiParams = SecureStorage.GetEncryptedData<GDAXAPIParams>(SecuredDataKeys.GDAXAPI);
            txtKey.Text = _apiParams.APIKey;
            txtSecret.Text = _apiParams.APISecret;
            txtPassphrase.Text = _apiParams.APIPassphrase;
        }

        /// <summary>
        /// Saves the new keys
        /// </summary>
        /// <returns>True if the data was saved correctly</returns>
        public bool Save()
        {
            if (txtSecret.Text != _apiParams.APISecret || txtKey.Text != _apiParams.APIKey || txtPassphrase.Text != _apiParams.APIPassphrase)
            {
                var p = new GDAXAPIParams
                {
                    APIKey = txtKey.Text,
                    APISecret = txtSecret.Text,
                    APIPassphrase = txtPassphrase.Text,
                };
                SecureStorage.SaveEncryptedData(p, SecuredDataKeys.GDAXAPI);
                ExchangeProxyFactory.NotifySettingsChanged(ExchangesInternalCodes.GDAX);
                EventAggregator.Instance.Publish(new SecuredDataChanged { DataKey = ExchangesInternalCodes.GDAX });

            }
            return true;

        }
    }
}
