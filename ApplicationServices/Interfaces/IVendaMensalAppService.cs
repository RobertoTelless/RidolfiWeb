using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IVendaMensalAppService : IAppServiceBase<VENDA_MENSAL>
    {
        Int32 ValidateCreate(VENDA_MENSAL perfil, USUARIO usuario);
        Int32 ValidateEdit(VENDA_MENSAL perfil, VENDA_MENSAL perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(VENDA_MENSAL item, VENDA_MENSAL itemAntes);
        Int32 ValidateDelete(VENDA_MENSAL perfil, USUARIO usuario);
        Int32 ValidateReativar(VENDA_MENSAL perfil, USUARIO usuario);

        List<VENDA_MENSAL> GetAllItens(Int32 idAss);
        List<VENDA_MENSAL> GetAllItensAdm(Int32 idAss);
        VENDA_MENSAL GetItemById(Int32 id);
        VENDA_MENSAL CheckExist(VENDA_MENSAL conta, Int32 idAss);
        Tuple<Int32, List<VENDA_MENSAL>, Boolean> ExecuteFilterTuple(DateTime? dataRef, Int32? tipo, Int32 idAss);
        VENDA_MENSAL_ANEXO GetAnexoById(Int32 id);
        VENDA_MENSAL_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 ValidateEditAnotacao(VENDA_MENSAL_ANOTACAO item);

        VENDA_MENSAL_COMISSAO GetComissaoById(Int32 id);
        Int32 ValidateEditComissao(VENDA_MENSAL_COMISSAO item);
        Int32 ValidateCreateComissao(VENDA_MENSAL_COMISSAO item);
    }
}
