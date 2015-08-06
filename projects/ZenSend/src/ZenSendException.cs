using System.Net;
using System;
namespace ZenSend {
  public class ZenSendException : Exception {
    
    public readonly HttpStatusCode HttpStatus;
    public readonly string FailCode;
    public readonly string Parameter;
    
    public readonly decimal? CostInPence;
    public readonly decimal? NewBalanceInPence;

    public ZenSendException(HttpStatusCode httpStatus, string failcode, string parameter, decimal? costInPence, decimal? newBalanceInPence) {
      this.HttpStatus = httpStatus;
      this.FailCode = failcode;
      this.Parameter = parameter;
      this.CostInPence = costInPence;
      this.NewBalanceInPence = newBalanceInPence;
    }
  }
}