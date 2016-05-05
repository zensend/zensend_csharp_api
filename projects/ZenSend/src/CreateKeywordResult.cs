using Newtonsoft.Json;
namespace ZenSend {
  public class CreateKeywordResult {
    [JsonProperty("cost_in_pence")]
    public decimal CostInPence;

    [JsonProperty("new_balance_in_pence")]
    public decimal NewBalanceInPence;
    
  }
}
