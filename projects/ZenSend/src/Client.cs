using Newtonsoft.Json;
using System.Net;
using System.Collections.Specialized;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net;


namespace ZenSend {
  

  public class Client {
    private readonly string apiKey;
    private readonly string server;
    private readonly string verifyServer;
    
    public Client(string apiKey, string server = "https://api.zensend.io", string verifyServer = "https://verify.zensend.io") {
      this.apiKey = apiKey;
      this.server = server;
      this.verifyServer = verifyServer;
    }


    public string CreateMsisdnVerification(string number, string message = null, string originator = null) {
      var postParams = new NameValueCollection();
      postParams.Add("NUMBER", number);
      if (message != null) {
        postParams.Add("MESSAGE", message);
      }
      if (originator != null) {
        postParams.Add("ORIGINATOR", originator);
      }

      var result = UploadValues<CreateMsisdnVerificationResult>(this.verifyServer + "/api/msisdn_verify", postParams);
      return result.session;
    }

    public string MsisdnVerificationStatus(string session) {
      return Get<MsisdnVerificationStatusResult>(this.verifyServer + "/api/msisdn_verify?SESSION=" + WebUtility.UrlEncode(session)).msisdn;
    }

    public OperatorLookupResult LookupOperator(string number) {
      return Get<OperatorLookupResult>(this.server + "/v3/operator_lookup?NUMBER=" + WebUtility.UrlEncode(number));
    }
    
    public SmsResult SendSms(string originator, string body, string[] numbers, OriginatorType? originatorType = null, int? timeToLiveInMinutes = null, SmsEncoding? encoding = null) {
      
      assertNoCommas(numbers);
      var postParams = new NameValueCollection();
      postParams.Add("BODY", body);
      postParams.Add("ORIGINATOR", originator);
      if (originatorType.HasValue) {
        postParams.Add("ORIGINATOR_TYPE", ToString(originatorType.Value));
      }
      postParams.Add("NUMBERS", string.Join(",", numbers));
      if (timeToLiveInMinutes.HasValue) {
        postParams.Add("TIMETOLIVE", timeToLiveInMinutes.Value.ToString());
      }
      if (encoding.HasValue) {
        postParams.Add("ENCODING", ToString(encoding.Value));
      }
      
      return UploadValues<SmsResult>(this.server + "/v3/sendsms", postParams);
    }

    public Dictionary<string, decimal> GetPrices() {
      var result = Get<JsonPricesResult>(this.server + "/v3/prices");
      return result.pricesInPence;
    }

    public decimal CheckBalance() {
      
      var result = Get<JsonBalanceResult>(this.server + "/v3/checkbalance");
      return result.balance;
    }
    
    private T Get<T>(string url) {
      using (var client = new WebClient()) {
        client.Headers.Add("X-API-KEY", this.apiKey);
        try {
          var json = client.DownloadString(url);
          return ParseResult<T>(HttpStatusCode.OK, json, client.ResponseHeaders[HttpResponseHeader.ContentType]);
        } catch (WebException e) {
          if (e.Status == WebExceptionStatus.ProtocolError) {
            return ProcessException<T>(e);
          } else {
            throw;
          }         
        } 
      }     
    }
    private T UploadValues<T>(string url, NameValueCollection postParams) {
      using (var client = new WebClient()) {

        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        client.Headers.Add("X-API-KEY", this.apiKey);
        
        try {
          var bytes = client.UploadValues(this.server + "/v3/sendsms", postParams);

          return ParseResult<T>(HttpStatusCode.OK, Encoding.UTF8.GetString(bytes), client.ResponseHeaders[HttpResponseHeader.ContentType]);
        } catch (WebException e) {

          if (e.Status == WebExceptionStatus.ProtocolError) {
            return ProcessException<T>(e);
          } else {
            throw;
          }
        }
      }
              
    }
    
    private T ProcessException<T>(WebException e) {
      var response = e.Response as HttpWebResponse;
      var body = ResponseBodyToString(response);
      return ParseResult<T>(response.StatusCode, body, response.Headers[HttpResponseHeader.ContentType]);      
    }
    
    private string ResponseBodyToString(HttpWebResponse response) {
      using (Stream stream = response.GetResponseStream()) {
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        return  reader.ReadToEnd();
      }
    }
    
    private T ParseResult<T>(HttpStatusCode status, string json, string contentType) {
      
      if (contentType != null && contentType.Contains("application/json")) {
        var result = JsonConvert.DeserializeObject<JsonResult<T>>(json);
        
        if (result.failure != null) {
          throw new ZenSendException(status, result.failure.failcode, result.failure.parameter, result.failure.costInPence, result.failure.newBalanceInPence);
        }
        return result.success;
      } else {
        // not json .. :(
        throw new ZenSendException(status, null, null, null, null);
      }
    }
    

    private string ToString(SmsEncoding encoding) {
      switch(encoding) {
        case SmsEncoding.GSM:
          return "gsm";
        case SmsEncoding.UCS2:
          return "ucs2";
        default:
          throw new InvalidOperationException();
      }
    }
    
    private void assertNoCommas(string[] numbers) {
      foreach (var number in numbers) {
        if (number.Contains(",")) {
          throw new InvalidOperationException("Comma not allowed in numbers");
        }
      }
    }
    
    private string ToString(OriginatorType type) {
      switch(type) {
        case OriginatorType.Alphanumeric:
          return "alpha";
        case OriginatorType.Msisdn:
          return "msisdn";
        default:
          throw new InvalidOperationException();

      }
    }   
    
    private class JsonPricesResult {
      [JsonProperty("prices_in_pence")]
      public Dictionary<string, decimal> pricesInPence;
    }

    private class JsonBalanceResult {
      public decimal balance;
    }
    
    private class CreateMsisdnVerificationResult {
      public string session;
    }

    private class MsisdnVerificationStatusResult {
      public string msisdn;
    }

    private class JsonError {
      public string parameter;
      public string failcode;

      [JsonProperty("cost_in_pence")]
      public decimal? costInPence;

      [JsonProperty("new_balance_in_pence")]
      public decimal? newBalanceInPence;
      
    }
    private class JsonResult<T> {
      public T success;
      public JsonError failure;
    }
    

  }
}
