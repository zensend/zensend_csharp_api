using Newtonsoft.Json;
namespace ZenSend {
  public class CreateSubAccountResult {
    
    public string Name;
    [JsonProperty("api_key")]
    public string ApiKey;
    
    
  }
}
