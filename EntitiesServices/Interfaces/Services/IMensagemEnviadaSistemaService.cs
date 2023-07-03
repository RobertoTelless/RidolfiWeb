using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMensagemEnviadaSistemaService : IServiceBase<MENSAGENS_ENVIADAS_SISTEMA>
    {
        Int32 Create(MENSAGENS_ENVIADAS_SISTEMA perfil, LOG log);
        Int32 Create(MENSAGENS_ENVIADAS_SISTEMA perfil);
        Int32 Edit(MENSAGENS_ENVIADAS_SISTEMA perfil, LOG log);
        Int32 Edit(MENSAGENS_ENVIADAS_SISTEMA perfil);
        Int32 Delete(MENSAGENS_ENVIADAS_SISTEMA perfil, LOG log);

        MENSAGENS_ENVIADAS_SISTEMA GetItemById(Int32 id);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetByDate(DateTime data, Int32 idAss);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetAllItens(Int32 idAss);
        List<MENSAGENS_ENVIADAS_SISTEMA> ExecuteFilter(Int32? escopo, Int32? tipo, DateTime? inicio, DateTime? final, Int32? usuario, String titulo, String origem, Int32 idAss);
    }
}
