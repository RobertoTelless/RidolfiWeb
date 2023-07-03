using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IComissaoService : IServiceBase<COMISSAO>
    {
        Int32 Create(COMISSAO perfil, LOG log);
        Int32 Create(COMISSAO perfil);
        Int32 Edit(COMISSAO perfil, LOG log);
        Int32 Edit(COMISSAO perfil);
        Int32 Delete(COMISSAO perfil, LOG log);

        COMISSAO GetItemById(Int32 id);
        List<COMISSAO> GetAllItens(Int32 idAss);
        List<COMISSAO> GetAllItensAdm(Int32 idAss);
        List<COMISSAO> ExecuteFilterComissao(DateTime? data, Int32? idAss);
    }
}
