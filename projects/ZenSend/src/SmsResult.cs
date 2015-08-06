using Newtonsoft.Json;
namespace ZenSend {
  public class SmsResult {
    public string TxGuid;
    public int Numbers;
    public int SmsParts;
    public string Encoding;
    [JsonProperty("cost_in_pence")]
    public decimal CostInPence;

    [JsonProperty("new_balance_in_pence")]
    public decimal NewBalanceInPence;
    
  }
}
