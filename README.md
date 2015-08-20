[![Build Status](https://travis-ci.org/zensend/zensend_csharp_api.svg?branch=master)](https://travis-ci.org/zensend/zensend_csharp_api)

# ZenSend DotNet Bindings

ZenSend DotNet bindings for sending SMS messages and performing operator lookups. You can sign up for an account at https://zensend.io/

## Installation

To install ZenSend API, run the following command in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console)

    PM> Install-Package ZenSend -Version 1.0.0

## Sending SMS

    using ZendSend;

    var client = new Client("api_key");
    var result = client.SendSms(originator: "SPINTOWIN", body: "Welcome to spin to win", numbers: new string[]{"4477111222333", "4477222333444"});

    // with all the options

    result = client.SendSms(originator: "SPINTOWIN", body: "Welcome to spin to win", numbers: new string[]{"4477111222333", "4477222333444"}, originatorType: OriginatorType.Alphanumeric, timeToLiveInMinutes: 100, encoding: SmsEncoding.GSM);

    Console.WriteLine(result.Numbers); // => 2
    Console.WriteLine(result.SmsParts); // => 1
    Console.WriteLine(result.Encoding); // => "gsm"
    Console.WriteLine(result.TxGuid); // => "7CDEB38F-4370-18FD-D7CE-329F21B99209"
    Console.WriteLine(result.CostInPence); // => 2.3
    Console.WriteLine(result.NewBalanceInPence); // => 500.0

    // handling errors

    try {
      client.SendSms(originator: "SPINTOWIN", body: "Welcome to spin to win", numbers: new string[]{"4477111222333", "4477222333444"});
    } catch (System.Net.WebException e) {
      Console.WriteLine(e); // see https://msdn.microsoft.com/en-us/library/system.net.webexception(v=vs.110).aspx
    } catch (ZendSendException e) {
      Console.WriteLine(e.HttpStatus); // => http status code
      Console.WriteLine(e.FailCode); // => zensend error code (might be null)
      Console.WriteLine(e.Parameter); // => the parameter the failcode is related to (might be null)
    }

## Operator Lookup

    using ZendSend;

    var client = new Client("api_key");
    var response = client.LookupOperator("441234567890");

    Console.WriteLine(response.MNC) // => "34"
    Console.WriteLine(response.MCC) // => "234"
    Console.WriteLine(response.Operator) // => "eeora-uk"
    Console.WriteLine(response.CostInPence) // => 2.0
    Console.WriteLine(response.NewBalanceInPence) // => 190.006

## SMS Prices

    using ZendSend;

    var client = new Client("api_key");
    var prices = client.GetPrices();

    Console.WriteLine(prices) // => System.Collections.Generic.Dictionary`2[System.String,System.Decimal]
    Console.WriteLine(string.Join(";", prices.Select(x => x.Key + "=" + x.Value))) // => GB=100.0 (price in pence)
    


## Check Balance

    using ZendSend;

    var client = new Client("api_key");
    var balance = client.CheckBalance();

    Console.WriteLine(balance) // => 500.0 (balance in pence)

## Developing

### OSX

Install [DNX](https://github.com/aspnet/homebrew-dnx)

    brew tap aspnet/dnx
    brew install dnvm

## Building

    dnu restore
    cd projects/ZenSend
    dnu pack

## Example 

    csharp -r:bin/Debug/dnx451/ZenSend.dll -r:$HOME/.dnx/packages/Newtonsoft.Json/6.0.8/lib/net45/Newtonsoft.Json.dll

    using ZenSend;
    var client = new Client("h5IDtEL05ky6FWqc9MCA0g", "http://localhost:8084");
    var result = client.SendSms(originator: "originator", body: "BODY", numbers: new string[]{"447796354848"});

    Console.WriteLine(result.Numbers);
    => 1
    Console.WriteLine(result.SmsParts);
    => 1
    Console.WriteLine(result.Encoding);
    => gsm
    Console.WriteLine(result.TxGuid);
    => d5486bc8-a88a-4e3b-9c56-2d14f68d5f29

## Tests

    cd projects/ZenSendTest
    dnx . test
 
