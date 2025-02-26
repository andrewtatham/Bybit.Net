---
title: IBybitSocketClientLinearStreams
has_children: false
parent: IBybitSocketClientV5
grand_parent: Socket API documentation
---
*[generated documentation]*  
`BybitSocketClient > V5 > LinearStreams`  
*Bybit linear data streams*
  

***

## SubscribeToTickerUpdatesAsync  

[https://bybit-exchange.github.io/docs/v5/websocket/public/ticker](https://bybit-exchange.github.io/docs/v5/websocket/public/ticker)  
<p>

*Subscribe to ticker updates*  

```csharp  
var client = new BybitSocketClient();  
var result = await client.V5.LinearStreams.SubscribeToTickerUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BybitLinearTickerUpdate>> handler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbols|The symbols to subscribe|
|handler|Data handler|
|_[Optional]_ ct|Cancellation token. Cancelling will cancel the subscription|

</p>

***

## SubscribeToTickerUpdatesAsync  

[https://bybit-exchange.github.io/docs/v5/websocket/public/ticker](https://bybit-exchange.github.io/docs/v5/websocket/public/ticker)  
<p>

*Subscribe to ticker updates*  

```csharp  
var client = new BybitSocketClient();  
var result = await client.V5.LinearStreams.SubscribeToTickerUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BybitLinearTickerUpdate>> handler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe|
|handler|Data handler|
|_[Optional]_ ct|Cancellation token. Cancelling will cancel the subscription|

</p>
