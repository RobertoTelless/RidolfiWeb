using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMensagemEnviadaSistemaRepository : IRepositoryBase<MENSAGENS_ENVIADAS_SISTEMA>
    {
        MENSAGENS_ENVIADAS_SISTEMA GetItemById(Int32 id);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetByDate(DateTime data, Int32 idAss);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetAllItens(Int32 idAss);
        List<MENSAGENS_ENVIADAS_SISTEMA> ExecuteFilter(Int32? escopo, Int32? tipo, DateTime? inicio, DateTime? final, Int32? usuario, String titulo, String origem, Int32 idAss);
    }
}
