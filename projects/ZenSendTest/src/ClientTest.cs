using Xunit;
using System;
using ZenSend;
using System.Net;
using System.Collections.Generic;

namespace ZenSendTest
{
    
    public class ClientTest : IDisposable
    {
        private HttpServer server;
        public ClientTest() {
            server = new HttpServer();
        }
        
        [Fact]
        public void GetPricesTest()
        {
            server.SetResponse("application/json", 200, @"
                {""success"":{
                    ""prices_in_pence"": {
                        ""GB"":1.23,
                        ""US"":1.24
                    }
                }}

            ");

            var client = new Client("apikey", server.Url);
            Assert.Equal(new Dictionary<string, decimal>
                             {
                                { "GB", 1.23m }, 
                                { "US", 1.24m }
                             }, client.GetPrices());
        }

        [Fact]
        public void CheckBalanceTest()
        {
            
            server.SetResponse("application/json", 200, @"
                {""success"": {
                    ""balance"":100.4
                }
            }");
            
            var client = new Client("apikey", server.Url);
            Assert.Equal(100.4m, client.CheckBalance());
            Assert.Equal("apikey", server.LastRequest.Headers["X-API-KEY"]);
            
        }

        [Fact]
        public void SendSmsTest()
        {
            
            server.SetResponse("application/json", 200, @"
{""success"":{""txguid"":""f8223367-64a8-4f97-856d-56d57432eaf9"",""numbers"":2,""smsparts"":1,""encoding"":""gsm"",""cost_in_pence"":12.34, ""new_balance_in_pence"":10.2}}");
            
            var client = new Client("apikey", server.Url);
            var result = client.SendSms(originator: "orig", body: "bODY", numbers: new string[]{"447796354848", "447796354847"});
            
            Assert.Equal(2, result.Numbers);
            Assert.Equal(1, result.SmsParts);
            Assert.Equal(12.34m, result.CostInPence);
            Assert.Equal(10.2m, result.NewBalanceInPence);
            Assert.Equal("gsm", result.Encoding);
            Assert.Equal("apikey", server.LastRequest.Headers["X-API-KEY"]);
            Assert.Equal("application/x-www-form-urlencoded", server.LastRequest.Headers["content-type"]);
            Assert.Equal("BODY=bODY&ORIGINATOR=orig&NUMBERS=447796354848%2c447796354847", server.LastBodyAsString);
            
            
        }

        [Fact]
        public void CreateKeywordTest()
        {
            
            server.SetResponse("application/json", 200, @"
{""success"":{""cost_in_pence"":12.34, ""new_balance_in_pence"":10.2}}");
            
            var client = new Client("apikey", server.Url);
            var result = client.CreateKeyword(shortcode: "SC", keyword: "KW");
            

            Assert.Equal(12.34m, result.CostInPence);
            Assert.Equal(10.2m, result.NewBalanceInPence);
            Assert.Equal("apikey", server.LastRequest.Headers["X-API-KEY"]);
            Assert.Equal("application/x-www-form-urlencoded", server.LastRequest.Headers["content-type"]);
            Assert.Equal("SHORTCODE=SC&KEYWORD=KW&IS_STICKY=false", server.LastBodyAsString);
            
            
        }

        [Fact]
        public void CreateKeywordWithOptionsTest()
        {
            
            server.SetResponse("application/json", 200, @"
{""success"":{""cost_in_pence"":12.34, ""new_balance_in_pence"":10.2}}");
            
            var client = new Client("apikey", server.Url);
            var result = client.CreateKeyword(shortcode: "SC", keyword: "KW", is_sticky: true, mo_url: "http://mo");
            

            Assert.Equal(12.34m, result.CostInPence);
            Assert.Equal(10.2m, result.NewBalanceInPence);
            Assert.Equal("apikey", server.LastRequest.Headers["X-API-KEY"]);
            Assert.Equal("application/x-www-form-urlencoded", server.LastRequest.Headers["content-type"]);
            Assert.Equal("SHORTCODE=SC&KEYWORD=KW&IS_STICKY=true&MO_URL=http%3a%2f%2fmo", server.LastBodyAsString);
            
            
        }

        [Fact]
        public void SendSmsPoundTest()
        {
            
            server.SetResponse("application/json", 200, @"
{""success"":{""txguid"":""f8223367-64a8-4f97-856d-56d57432eaf9"",""numbers"":2,""smsparts"":1,""encoding"":""gsm"",""cost_in_pence"":12.34, ""new_balance_in_pence"":10.2}}");
            
            var client = new Client("apikey", server.Url);
            var result = client.SendSms(originator: "orig", body: "£pound", numbers: new string[]{"447796354848", "447796354847"});
            

            Assert.Equal("application/x-www-form-urlencoded", server.LastRequest.Headers["content-type"]);
            Assert.Equal("BODY=%c2%a3pound&ORIGINATOR=orig&NUMBERS=447796354848%2c447796354847", server.LastBodyAsString);
            
            
        }

        [Fact]
        public void SendSmsWithOptionalParametersTest()
        {
            
            server.SetResponse("application/json", 200, @"
{""success"":{""txguid"":""f8223367-64a8-4f97-856d-56d57432eaf9"",""numbers"":2,""smsparts"":1,""encoding"":""gsm"",""cost_in_pence"":123.4, ""new_balance_in_pence"":10.2}}");
            
            var client = new Client("apikey", server.Url);
            var result = client.SendSms(originator: "orig", body: "bODY", numbers: new string[]{"447796354848", "447796354847"}, originatorType: OriginatorType.Alphanumeric, timeToLiveInMinutes: 60, encoding: SmsEncoding.GSM);
            
            Assert.Equal(2, result.Numbers);
            Assert.Equal(1, result.SmsParts);
            Assert.Equal(123.4m, result.CostInPence);
            Assert.Equal(10.2m, result.NewBalanceInPence);
            Assert.Equal("gsm", result.Encoding);
            Assert.Equal("apikey", server.LastRequest.Headers["X-API-KEY"]);
            Assert.Equal("application/x-www-form-urlencoded", server.LastRequest.Headers["content-type"]);
            Assert.Equal("BODY=bODY&ORIGINATOR=orig&ORIGINATOR_TYPE=alpha&NUMBERS=447796354848%2c447796354847&TIMETOLIVE=60&ENCODING=gsm", server.LastBodyAsString);
        
            
        }

        [Fact]
        public void OperatorLookupTest() {
            server.SetResponse("application/json", 200, @"
{""success"":{""mcc"":""123"",""mnc"":""456"",""operator"":""o2-uk"",""cost_in_pence"":2.5,""new_balance_in_pence"":10.2}}");
            
            var client = new Client("apikey", server.Url);
            var result = client.LookupOperator("441234567890");

            Assert.Equal("123", result.MCC);
            Assert.Equal("456", result.MNC);
            Assert.Equal("o2-uk", result.Operator);
            Assert.Equal(2.5m, result.CostInPence);
            Assert.Equal(10.2m, result.NewBalanceInPence);

            Assert.Equal("apikey", server.LastRequest.Headers["X-API-KEY"]);

            var collection = server.LastRequest.QueryString;
            Assert.Equal("441234567890", collection["NUMBER"]);
        }  

        [Fact]
        public void OperatorLookupErrorTest() {
            server.SetResponse("application/json", 503, @"
{""failure"":{""failcode"":""DATA_MISSING"",""cost_in_pence"":2.5,""new_balance_in_pence"":10.2}}");
            

            var client = new Client("apikey", server.Url);
            
            var e = Assert.Throws<ZenSendException>(() => client.LookupOperator("441234567890"));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, e.HttpStatus);
            Assert.Equal("DATA_MISSING", e.FailCode);
            Assert.Equal(null, e.Parameter);
            Assert.Equal(2.5m, e.CostInPence.Value);
            Assert.Equal(10.2m, e.NewBalanceInPence);

        }  
        

        [Fact]
        public void ShouldNotBeAbleToPutCommaInNumbersTest() {
            var client = new Client("apikey", server.Url);
            
            Assert.Throws<InvalidOperationException>(() => client.SendSms(originator: "orig", body: "bODY", numbers: new string[]{"447796,354848"}));
            
        }
        
        [Fact]
        public void HandleAnErrorTest() {

            server.SetResponse("application/json", 400, @"{
      ""failure"": {
          ""failcode"": ""GENERIC_ERROR""
      }
    }");

            var client = new Client("apikey", server.Url);
            

            var e = Assert.Throws<ZenSendException>(() => client.SendSms(originator: "orig", body: "bODY", numbers: new string[]{"447796354848", "447796354847"}));
            
            Assert.Equal(HttpStatusCode.BadRequest, e.HttpStatus);
            Assert.Equal("GENERIC_ERROR", e.FailCode);
            Assert.Equal(null, e.Parameter);
            

        }
        
        [Fact]
        public void HandleParameterErrorTest() {

            server.SetResponse("application/json", 400, @"{
      ""failure"": {
          ""failcode"": ""IS_EMPTY"",
          ""parameter"": ""BODY""
      }
    }");

            var client = new Client("apikey", server.Url);
            

            var e = Assert.Throws<ZenSendException>(() => client.SendSms(originator: "orig", body: "bODY", numbers: new string[]{"447796354848", "447796354847"}));
            
            Assert.Equal(HttpStatusCode.BadRequest, e.HttpStatus);
            Assert.Equal("IS_EMPTY", e.FailCode);
            Assert.Equal("BODY", e.Parameter);
            

        }        
 
         [Fact]
        public void HandleNonJsonResponseTest() {

            server.SetResponse("text/plain", 503, "Gateway Timeout");

            var client = new Client("apikey", server.Url);
            

            var e = Assert.Throws<ZenSendException>(() => client.SendSms(originator: "orig", body: "bODY", numbers: new string[]{"447796354848", "447796354847"}));
            
            Assert.Equal(HttpStatusCode.ServiceUnavailable, e.HttpStatus);
            Assert.Equal(null, e.FailCode);
            Assert.Equal(null, e.Parameter);
            

        }  
               
        public void Dispose() {
            server.Dispose();
        }
    }
}
