using Transactions_Api.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Transactions_Api.Shared.Utils;

public static class LinkHelper
{
    // Adiciona links HATEOAS a uma única TransacaoResourceDTO
    public static TransacaoResourceDTO AddLinks(this TransacaoResourceDTO resource, IUrlHelper urlHelper)
    {
        var txid = resource.Transacao.Txid;

        // Verifica se o link já existe antes de adicioná-lo
        if (!resource.Links.Any(link => link.Rel == "self" && link.Href == urlHelper.Link("GetTransacaoByTxid", new { txid })))
        {
            resource.Links.Add(new LinkDTO(
                rel: "self",
                href: urlHelper.Link("GetTransacaoByTxid", new { txid }),
                method: "GET"
            ));
        }
        return resource;
    }

    
    

   
}