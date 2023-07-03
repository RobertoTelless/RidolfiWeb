using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITabelaIPCAService : IServiceBase<TABELA_IPCA>
    {
        Int32 Create(TABELA_IPCA perfil, LOG log);
        Int32 Create(TABELA_IPCA perfil);
        Int32 Edit(TABELA_IPCA perfil, LOG log);
        Int32 Edit(TABELA_IPCA perfil);
        Int32 Delete(TABELA_IPCA perfil, LOG log);

        TABELA_IPCA CheckExist(TABELA_IPCA conta);
        TABELA_IPCA GetItemById(Int32 id);
        List<TABELA_IPCA> GetAllItens();
        List<TABELA_IPCA> GetAllItensAdm();
        List<TABELA_IPCA> ExecuteFilter(DateTime? data);
    }
}
