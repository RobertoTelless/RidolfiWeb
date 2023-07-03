using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IVendaMensalService : IServiceBase<VENDA_MENSAL>
    {
        Int32 Create(VENDA_MENSAL perfil);
        Int32 Edit(VENDA_MENSAL perfil, LOG log);
        Int32 Edit(VENDA_MENSAL perfil);
        Int32 Delete(VENDA_MENSAL perfil, LOG log);

        VENDA_MENSAL CheckExist(VENDA_MENSAL conta, Int32 idAss);
        VENDA_MENSAL GetItemById(Int32 id);
        List<VENDA_MENSAL> GetAllItens(Int32 idAss);
        List<VENDA_MENSAL> GetAllItensAdm(Int32 idAss);
        VENDA_MENSAL_ANEXO GetAnexoById(Int32 id);
        VENDA_MENSAL_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 EditAnotacao(VENDA_MENSAL_ANOTACAO item);

        List<VENDA_MENSAL> ExecuteFilter(DateTime? dataRef, Int32? tipo, Int32 idAss);
        
        VENDA_MENSAL_COMISSAO GetComissaoById(Int32 id);
        Int32 EditComissao(VENDA_MENSAL_COMISSAO item);
        Int32 CreateComissao(VENDA_MENSAL_COMISSAO item);

    }
}
