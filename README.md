 [nuget-url]: https://www.nuget.org/packages/GON.TlvExtensions/

# GON.TlvExtensions

## What is does?

.Net Extenstions to simplify work with SIMPLE-TLV and BER-TLV arrays


## Nuget Package

[NuGet package link][nuget-url]


## Examples

Splitting BER-TLV Array for TLV List

```csharp
var tlv = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA".SplitToTlvStrings();

foreach (var item in tlv)
{
    Console.WriteLine($"{item.GetTlvTagHex(),10} : {item.GetTlvLenHex(),5} : {item.GetTlvValHex()}");
}
//      9F10 :    03 : A1A2A3
//        5A :    05 : 5152535455
//      9F48 :    00 :
//      9F44 :  8101 : FF
//3F81828355 :    01 : BB
//        8F :    03 : AAAAAA
```

Splitting BER-TLV Array for TLV Dictionary with Tag as a Key

```csharp
var tlv = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA".ToTlvDictionary();

foreach (var item in tlv)
{
    Console.WriteLine($"{item.Key,10} : {item.Value.GetTlvValHex()}");
}
//      9F10 : A1A2A3
//        5A : 5152535455
//      9F48 :
//      9F44 : FF
//3F81828355 : BB
//        8F : AAAAAA
```

Getting by a tag name a specific TLV from the BER-TLV Array

```csharp
var tlv = "9f1003a1a2a3 5A055152535455 9f4800 9F448101FF 3F8182835501BB 8F03AAAAAA";
var tag = tlv.FindTag("5A");
Console.WriteLine($"{tag.GetTlvTagHex()} : {tag.GetTlvLenHex()} : {tag.GetTlvValHex()}");
//5A : 05 : 5152535455
```

Making a BER-TLV structure from a Tag name and Value

```csharp
var data = "0102030405".WrapTlv("50");
Console.WriteLine(data);
//50050102030405
```


