using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITemplatePropostaAppService : IAppServiceBase<TEMPLATE_PROPOSTA>
    {
        Int32 ValidateCreate(TEMPLATE_PROPOSTA item, USUARIO usuario);
        Int32 ValidateEdit(TEMPLATE_PROPOSTA item, TEMPLATE_PROPOSTA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TEMPLATE_PROPOSTA item, USUARIO usuario);
        Int32 ValidateReativar(TEMPLATE_PROPOSTA item, USUARIO usuario);

        TEMPLATE_PROPOSTA GetByCode(String code, Int32 idAss);
        TEMPLATE_PROPOSTA CheckExist(TEMPLATE_PROPOSTA item, Int32 idAss);
        List<TEMPLATE_PROPOSTA> GetAllItens(Int32 idAss);
        TEMPLATE_PROPOSTA GetItemById(Int32 id);
        List<TEMPLATE_PROPOSTA> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<TEMPLATE_PROPOSTA>, Boolean> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss);
    }
}
