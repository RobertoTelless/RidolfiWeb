using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITabelaIRRFService : IServiceBase<TABELA_IRRF>
    {
        Int32 Create(TABELA_IRRF perfil, LOG log);
        Int32 Create(TABELA_IRRF perfil);
        Int32 Edit(TABELA_IRRF perfil, LOG log);
        Int32 Edit(TABELA_IRRF perfil);
        Int32 Delete(TABELA_IRRF perfil, LOG log);

        TABELA_IRRF CheckExist(TABELA_IRRF conta);
        TABELA_IRRF GetItemById(Int32 id);
        List<TABELA_IRRF> GetAllItens();
        List<TABELA_IRRF> GetAllItensAdm();
    }
}
