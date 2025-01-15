using System.Text.RegularExpressions;

namespace Transactions_Api.Shared.Utils;

public class Helper : ITxidGenerator
{
    
    const string MicrocashIspb = "45756448";

    public  string GerarTxid()
    {
        var hora = DateTime.UtcNow;

        // Generate random alphanumeric string
        var randomChars = new Regex("[^a-zA-Z0-9]")
            .Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "")
            .Substring(0, 11);

        // Build the txid
        var txid = $"T{MicrocashIspb}{hora:yyyyMMddHHmm}{randomChars}";

        // Ensure txid length is between 26 and 35 characters
        if (txid.Length > 35)
        {
            txid = txid.Substring(0, 35);
        }

        return txid;
    }

    
    public static string GerarEndToEndId(DateTime hora)
    {
        // Generate random alphanumeric string
        var randomChars = new Regex("[^a-zA-Z0-9 -]")
            .Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "")
            .Substring(0, 11);
        return $"E{MicrocashIspb}{hora:yyyyMMddHHmm}{randomChars}";
    }
    
    
    


}