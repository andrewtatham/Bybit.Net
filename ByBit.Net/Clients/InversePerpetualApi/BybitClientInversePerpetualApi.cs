﻿using Bybit.Net.Objects;
using Bybit.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bybit.Net.Clients.Rest.Futures
{
    /// <inheritdoc cref="IBybitClientInversePerpetualApi" />
    public class BybitClientInversePerpetualApi : RestApiClient, IBybitClientInversePerpetualApi
    {
        private readonly BybitClient _baseClient;
        private readonly BybitClientOptions _options;
        private readonly Log _log;

        internal static TimeSpan TimeOffset;
        internal static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        internal static DateTime LastTimeSync;

        internal BybitClientOptions ClientOptions { get; }

        /// <inheritdoc />
        public IBybitClientInversePerpetualApiAccount Account { get; }
        /// <inheritdoc />
        public IBybitClientInversePerpetualApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public IBybitClientInversePerpetualApiTrading Trading { get; }

        #region ctor
        internal BybitClientInversePerpetualApi(Log log, BybitClient baseClient, BybitClientOptions options) 
            : base(options, options.InversePerpetualApiOptions)
        {
            _baseClient = baseClient;
            _log = log;
            _options = options;
            ClientOptions = options;

            Account = new BybitClientInversePerpetualApiAccount(this);
            ExchangeData = new BybitClientInversePerpetualApiExchangeData(this);
            Trading = new BybitClientInversePerpetualApiTrading(this);
        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BybitAuthenticationProvider(credentials);

        /// <summary>
        /// Get url for an endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        internal Uri GetUrl(string endpoint)
        {
            return new Uri(BaseAddress.AppendPath(endpoint));
        }

        internal async Task<WebCallResult<BybitResult<T>>> SendRequestWrapperAsync<T>(
             Uri uri,
             HttpMethod method,
             CancellationToken cancellationToken,
             Dictionary<string, object>? parameters = null,
             bool signed = false,
             JsonSerializer? deserializer = null) where T : class
        {
            var result = await _baseClient.SendRequestInternal<BybitResult<T>>(this, uri, method, cancellationToken, parameters, signed, deserializer: deserializer).ConfigureAwait(false);
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
            var result = await _baseClient.SendRequestInternal<BybitResult<T>>(this, uri, method, cancellationToken, parameters, signed, deserializer: deserializer).ConfigureAwait(false);
            if (!result)
                return result.As<T>(default);

            if (result.Data.ReturnCode != 0)
                return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, default, new ServerError(result.Data.ReturnCode, result.Data.ReturnMessage));

            return result.As(result.Data.Result);
        }

        /// <inheritdoc />
        protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
        {
            return ExchangeData.GetServerTimeAsync();
        }

        /// <inheritdoc />
        protected override TimeSyncModel GetTimeSyncParameters()
        {
            return new TimeSyncModel(_options.InversePerpetualApiOptions.AutoTimestamp, SemaphoreSlim, LastTimeSync);
        }

        /// <inheritdoc />
        protected override void UpdateTimeOffset(TimeSpan timestamp)
        {
            LastTimeSync = DateTime.UtcNow;
            if (timestamp.TotalMilliseconds > 0 && timestamp.TotalMilliseconds < 500)
                return;

            _log.Write(LogLevel.Information, $"Time offset set to {Math.Round(timestamp.TotalMilliseconds)}ms");
            TimeOffset = timestamp;
        }

        /// <inheritdoc />
        public override TimeSpan GetTimeOffset() => TimeOffset;
    }
}
