using Newtonsoft.Json;
namespace ZenSend {
  public class OperatorLookupResult {
    [JsonProperty("mcc")]
    public string MCC;
    [JsonProperty("mnc")]
    public string MNC;
    public string Operator;


    [JsonProperty("cost_in_pence")]
    public decimal CostInPence;

    [JsonProperty("new_balance_in_pence")]
    public decimal NewBalanceInPence;
    
  }
}
