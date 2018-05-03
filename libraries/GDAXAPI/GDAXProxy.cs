using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
namespace GDAXAPI
{
    /// <summary>
    /// Constant for error codes
    /// </summary>
    public class ErrorCodes
    {
        /// <summary>
        /// We don't handle that error 
        /// </summary>
        public const int UnknownError = 0;
        /// <summary>
        /// Invalid/empty API keys
        /// </summary>
        public const int InvalidAPIKeys = -1;
    }

    /// <summary>
    /// Proxy for making calls to the  Bitsamp API
    /// </summary>
    public class GDAXProxy
    {
        #region Private Members
        /// <summary>
        /// The Secret key
        /// </summary>
        private string _secretKey;
        /// <summary>
        /// The API Key
        /// </summary>
        private string _apiKey;
        /// <summary>
        /// The client ID
        /// </summary>
        private string _passphrase;
        /// <summary>
        /// The API's base URL
        /// </summary>
        private string _baseURL;
        /// <summary>
        /// The current fee
        /// </summary>
        private decimal _fee = 0.0M;
        /// <summary>
        /// Used to compute hmac signature to sign data sent to the private API
        /// </summary>
        /// <summary>
        /// Message displayed when there are no API keys
        /// </summary>
        private const string InvalidKeysErrMsg = "API key not found";
        #endregion

        /// <summary>
        /// Initialises a new instance of the GDAXProxy class
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="privateKey"></param>
        /// <param name="passphrase"></param>
        public GDAXProxy(string baseURL, string apiKey, string privateKey, string passphrase)
        {
            authenticator = new Authenticator(apiKey, privateKey, passphrase);
            this.clock = new Clock();
            _baseURL = baseURL;
            _passphrase = passphrase;
            _apiKey = apiKey;
            _secretKey = privateKey;
        }

        /// <summary>
        /// Gets GDAX's ticker
        /// </summary>
        /// <returns></returns>
        public CallResult<Ticker> GetTicker()
        {
            return MakeGetRequest<Ticker>("/products/BTC-USD/ticker", result => Ticker.CreateFromJObject(result as JObject));
        }

        /// <summary>
        /// Gets the full order book
        /// </summary>
        /// <returns></returns>
        public CallResult<OrderBook> GetOrderBook()
        {
            return MakeGetRequest<OrderBook>("/products/BTC-USD/book/?level=3", result => OrderBook.CreateFromJObject(result as JObject));
        }

        /// <summary>
        /// Retrives all public transactions for the past minute/hour
        /// </summary>
        /// <param name="lastMinuteOnly">True if we want the transactions for the last minute only</param>
        /// <returns></returns>
        public CallResult<TransactionList> GetTransactions(bool lastMinuteOnly = false)
        {
            string url = "/products/BTC-USD/trades" + (lastMinuteOnly ? "?time=minute" : string.Empty);
            return MakeGetRequest(url, result => TransactionList.ReadFromJObject(result as JArray));
        }

        /// <summary>
        /// Gets the user's balance
        /// </summary>
        /// <returns></returns>
        public CallResult<Balance> GetBalance()
        {
            return MakeGetRequest("/accounts", r =>
            {
                Balance balance = Balance.CreateFromJArray(r as JArray);
                _fee = balance != null ? balance.Fee : 0.0M;
                return balance;
            });
        }

        /// <summary>
        /// Places a buy order
        /// </summary>
        /// <param name="amount">The amount of BTCs to buy</param>
        /// <param name="price">The price  per BTC</param>
        /// <returns></returns>
        public CallResult<OrderDetails> PlaceBuyOrder(decimal amount, decimal price)
        {
            return MakePostRequest("/orders",
                r => OrderDetails.CreateFromJObject(r as JObject),

                new Dictionary<string, string> {
                    {"size", amount.ToString(CultureInfo.InvariantCulture)},
                    {"price",price.ToString(CultureInfo.InvariantCulture)},
                    {"side","buy"},
                    { "product_id","BTC-USD"}
                });
        }

        /// <summary>
        /// Places a sell order
        /// </summary>
        /// <param name="amount">The amount of BTCs to sell</param>
        /// <param name="price">The price per BTC</param>
        /// <returns></returns>
        public CallResult<OrderDetails> PlaceSellOrder(decimal amount, decimal price)
        {
            return MakePostRequest("/orders",
                r => OrderDetails.CreateFromJObject(r as JObject),

                new Dictionary<string, string> {
                {"amount", amount.ToString(CultureInfo.InvariantCulture)},
                    {"price",price.ToString(CultureInfo.InvariantCulture)},
                     {"side","sell"},
                    { "product_id","BTC-USD"}
                });
        }

        /// <summary>
        /// Cancels an open order
        /// </summary>
        /// <param name="orderId">The order's ID</param>
        /// <returns></returns>
        public CallResult<bool> CancelOrder(long orderId)
        {
            var args =new Dictionary<string,string>();
            args["id"] = orderId.ToString();
            var resultStr = SendDeleteRequest("/orders/"+orderId);
            var result = resultStr == "true" ? new JObject() : JObject.Parse(resultStr);
            return ParseCallResult<bool>(result, r => resultStr == "true");
        }

        /// <summary>
        /// Gets a list of all the user's open orders
        /// </summary>
        /// <returns></returns>
        public CallResult<OpenOrdersContainer> GetOpenOrders()
        {
            return MakeGetRequest("/orders", t => OpenOrdersContainer.CreateFromJObject(t as JArray));
        }

        #region Private methods

        /// <summary>
        /// Builds post data for a request
        /// </summary>
        /// <param name="dataDic"></param>
        /// <returns></returns>
        private static string BuildPostData(Dictionary<string, string> dataDic)
        {
            var p = dataDic.Keys.Select(key => String.Format("\"{0}\":\"{1}\"", key, dataDic[key])).ToArray();
            return "{"+string.Join(",", p)+"}";
        }

        /// <summary>
        /// Parses the result of a call to the API and converts that result into an object, wrapped in a CallResult object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private CallResult<T> ParseCallResult<T>(JToken token, Func<JToken, T> func)
        {
            JToken val;
            string error = null;
            var result = token as JObject;

            if (result != null && result.TryGetValue("error", out val))
            {
                var jValue = val as JValue;
                if (jValue != null)
                {
                    error = (string)jValue.Value;
                }
                else
                {
                    error = string.Join("\n", val["__all__"].Select(jt => ((JValue)jt).Value));
                }
            }

            var r = new CallResult<T>
            {
                ErrorMessage = error,
                ErrorCode = error == InvalidKeysErrMsg ? ErrorCodes.InvalidAPIKeys : ErrorCodes.UnknownError,
                Result = string.IsNullOrEmpty(error) ? func(token) : default(T)
            };
            return r;
        }

        /// <summary>
        /// Sends a post request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string SendPostRequest(string url, Dictionary<string, string> args)
        {
            var json = BuildPostData(args);
            //string json = "{\"side\":\"buy\",\"size\":\"0.001\",\"price\":\"8000.0\",\"type\":\"limit\",\"product_id\":\"BTC-USD\"}";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(new Uri(_baseURL), url));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var timeStamp = clock.GetTime().ToTimeStamp();
            var signedSignature = ComputeSignature(HttpMethod.Post, authenticator.UnsignedSignature, timeStamp, url, json);
            httpWebRequest.UserAgent = "CoinT.Net";
            AddHeaders(httpWebRequest, signedSignature, timeStamp);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }

        }

        /// <summary>
        /// Sends a GET Request
        /// </summary>
        /// <param name="url">The relative url to which the request will be sent</param>
        /// <returns>The request's result</returns>
        private string SendGetRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(new Uri(_baseURL), url));
            var timeStamp = clock.GetTime().ToTimeStamp();
            var signedSignature = ComputeSignature(HttpMethod.Get, authenticator.UnsignedSignature, timeStamp, url);
            request.UserAgent = "CoinT.Net";
            AddHeaders(request, signedSignature, timeStamp);
            var response = request.GetResponse();
            using (var resStream = response.GetResponseStream())
            {
                using (var resStreamReader = new StreamReader(resStream))
                {
                    string resString = resStreamReader.ReadToEnd();
                    return resString;
                }
            }
        }

        /// <summary>
        /// Sends a GET Request
        /// </summary>
        /// <param name="url">The relative url to which the request will be sent</param>
        /// <returns>The request's result</returns>
        private string SendDeleteRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(new Uri(_baseURL), url));
            var timeStamp = clock.GetTime().ToTimeStamp();
            var signedSignature = ComputeSignature(HttpMethod.Delete, authenticator.UnsignedSignature, timeStamp, url);
            request.UserAgent = "CoinT.Net";
            AddHeaders(request, signedSignature, timeStamp);
            var response = request.GetResponse();
            using (var resStream = response.GetResponseStream())
            {
                using (var resStreamReader = new StreamReader(resStream))
                {
                    string resString = resStreamReader.ReadToEnd();
                    return resString;
                }
            }
        }
        /// <summary>
        /// Makes a GET request and returns the response as an object wrapped in a CallResult
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <param name="url">The relative URL to make the call to</param>
        /// <param name="conversion">The function used to convert the response into an object</param>
        /// <returns></returns>
        private CallResult<T> MakeGetRequest<T>(string url, Func<JToken, T> conversion)
        {
            try
            {
                var resultStr = SendGetRequest(url);
                var result = JToken.Parse(resultStr);
                return ParseCallResult(result, r => conversion(result));
            }
            catch (Exception e)
            {
                return new CallResult<T> { ErrorMessage = e.Message, Exception = e };
            }
        }

        /// <summary>
        /// Makes a Post request and returns the response as an object wrapped in a CallResult
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <param name="url">The relative URL to make the call to</param>
        /// <param name="conversion">The function used to convert the response into an object</param>
        /// <param name="extraArgs">The extra parameters to pass to the POST request</param>
        /// <returns></returns>
        private CallResult<T> MakePostRequest<T>(string url, Func<JToken, T> conversion, Dictionary<string, string> args = null)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_secretKey) || string.IsNullOrEmpty(_passphrase))
            {
                return new CallResult<T> { ErrorMessage = "Missing API Keys/Client ID", ErrorCode = ErrorCodes.InvalidAPIKeys };
            }
            try
            {
                var resultStr = SendPostRequest(url, args);
                var result = JToken.Parse(resultStr);
                return ParseCallResult(result, r => conversion(result));
            }
            catch (Exception e)
            {
                return new CallResult<T> { ErrorMessage = e.Message, Exception = e };
            }
        }

        private readonly IAuthenticator authenticator;

        private readonly IClock clock;

        private static string ComputeSignature(HttpMethod httpMethod, string secret, double timestamp, string requestUri, string contentBody = "")
        {
            var convertedString = Convert.FromBase64String(secret);
            var prehash = timestamp.ToString("F0", CultureInfo.InvariantCulture) + httpMethod.ToString().ToUpper() + requestUri + contentBody;
            return HashString(prehash, convertedString);
        }

        private static string HashString(string str, byte[] secret)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var hmaccsha = new HMACSHA256(secret))
            {
                return Convert.ToBase64String(hmaccsha.ComputeHash(bytes));
            }
        }

        private void AddHeaders(WebRequest request, string signedSignature, double timeStamp)
        {
            request.Headers.Add("CB-ACCESS-KEY", authenticator.ApiKey);
            request.Headers.Add("CB-ACCESS-TIMESTAMP", timeStamp.ToString("F0", CultureInfo.InvariantCulture));
            request.Headers.Add("CB-ACCESS-SIGN", signedSignature);
            request.Headers.Add("CB-ACCESS-PASSPHRASE", authenticator.Passphrase);
        }
        #endregion
    }

    public interface IClock
    {
        DateTime GetTime();
    }
    public class Clock : IClock
    {
        public DateTime GetTime()
        {
            return DateTime.UtcNow;
        }
    }
    public interface IAuthenticator
    {
        string ApiKey { get; }

        string UnsignedSignature { get; }

        string Passphrase { get; }
    }
    public class Authenticator : IAuthenticator
    {
        public Authenticator(
            string apiKey,
            string unsignedSignature,
            string passphrase)
        {
            ApiKey = apiKey;
            UnsignedSignature = unsignedSignature;
            Passphrase = passphrase;
        }

        public string ApiKey { get; }

        public string UnsignedSignature { get; }

        public string Passphrase { get; }
    }
    public static class DateExtensions
    {
        public static double ToTimeStamp(this DateTime date)
        {
            return (date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }

}
