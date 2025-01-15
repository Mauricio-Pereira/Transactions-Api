namespace Transactions_Api.Application.DTOs;

public class LinkDTO
{
    public string Rel { get; set; } // Relação do link, ex: "self", "update", "delete"
    public string Href { get; set; } // URL do link
    public string Method { get; set; } // Método HTTP, ex: "GET", "PUT", "DELETE"

    public LinkDTO()
    {
    }

    public LinkDTO(string rel, string href, string method)
    {
        Rel = rel;
        Href = href;
        Method = method;
    }
}