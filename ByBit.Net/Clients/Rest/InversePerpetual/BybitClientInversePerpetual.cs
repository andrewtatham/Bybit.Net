﻿using ByBit.Net.Objects;
using ByBit.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ByBit.Net.Clients.Rest.InversePerpetual
{
    public class BybitClientInversePerpetual : RestClient//, IBybitInversePerpetualClient
    {
        internal new BybitClientInversePerpetualOptions ClientOptions { get; }

        public BybitClientInversePerpetualAccount Account { get; }
        public BybitClientInversePerpetualExchangeData ExchangeData { get; }
        public BybitClientInversePerpetualTrading Trading { get; }

        #region ctor
        /// <summary>
        /// Create a new instance of BybitInversePerpetualClient using the default options
        /// </summary>
        public BybitClientInversePerpetual() : this(BybitClientInversePerpetualOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of BybitInversePerpetualClient using the provided options
        /// </summary>
        public BybitClientInversePerpetual(BybitClientInversePerpetualOptions options) : base("Bybit[InversePerpetual]", options, options.ApiCredentials == null ? null : new BybitAuthenticationProvider(options.ApiCredentials))
        {
            ClientOptions = options;
            Account = new BybitClientInversePerpetualAccount(this);
            ExchangeData = new BybitClientInversePerpetualExchangeData(this);
            Trading = new BybitClientInversePerpetualTrading(this);
        }
        #endregion

        #region methods
        /// <summary>
        /// Sets the default options to use for new clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(BybitClientInversePerpetualOptions options)
        {
            BybitClientInversePerpetualOptions.Default = options;
        }

        /// <summary>
        /// Set the API key and secret. Api keys can be managed at https://bittrex.com/Manage#sectionApi
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetAuthenticationProvider(new BybitAuthenticationProvider(new ApiCredentials(apiKey, apiSecret)));
        }

        /// <summary>
        /// Get url for an endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        internal Uri GetUrl(string endpoint)
        {
            return new Uri($"{ClientOptions.BaseAddress}v2/{endpoint}");
        }

        internal async Task<WebCallResult<BybitResult<T>>> SendRequestWrapperAsync<T>(
             Uri uri,
             HttpMethod method,
             CancellationToken cancellationToken,
             Dictionary<string, object>? parameters = null,
             bool signed = false,
             JsonSerializer? deserializer = null) where T : class
        {
            var result = await base.SendRequestAsync<BybitResult<T>>(uri, method, cancellationToken, parameters, signed, deserializer: deserializer);
            if (!result)
                return result.As<BybitResult<T>>(default);

            if (result.Data.ReturnCode != 0)
                return new WebCallResult<BybitResult<T>>(result.ResponseStatusCode, result.ResponseHeaders, default, new ServerError(result.Data.ReturnCode, result.Data.ReturnMessage));

            return result.As(result.Data);
        }

        internal async Task<WebCallResult<T>> SendRequestAsync<T>(
             Uri uri,
             HttpMethod method,
             CancellationToken cancellationToken,
             Dictionary<string, object>? parameters = null,
             bool signed = false,
             JsonSerializer? deserializer = null) 
        {
            var result = await base.SendRequestAsync<BybitResult<T>>(uri, method, cancellationToken, parameters, signed, deserializer: deserializer);
            if (!result)
                return result.As<T>(default);

            if (result.Data.ReturnCode != 0)
                return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, default, new ServerError(result.Data.ReturnCode, result.Data.ReturnMessage));

            return result.As(result.Data.Result);
        }

        #endregion

        /// <inheritdoc />
        public string GetSymbolName(string baseAsset, string quoteAsset) => $"{baseAsset}-{quoteAsset}".ToUpperInvariant();
    }
}