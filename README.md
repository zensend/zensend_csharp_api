# ZenSend DotNet Bindings

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
 
