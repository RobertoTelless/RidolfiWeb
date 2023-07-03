using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMensagemEnviadaSistemaAppService : IAppServiceBase<MENSAGENS_ENVIADAS_SISTEMA>
    {
        Int32 ValidateCreate(MENSAGENS_ENVIADAS_SISTEMA perfil);

        MENSAGENS_ENVIADAS_SISTEMA GetItemById(Int32 id);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetByDate(DateTime data, Int32 idAss);
        List<MENSAGENS_ENVIADAS_SISTEMA> GetAllItens(Int32 idAss);
        Tuple<Int32, List<MENSAGENS_ENVIADAS_SISTEMA>, Boolean> ExecuteFilterTuple(Int32? escopo, Int32? tipo, DateTime? inicio, DateTime? final, Int32? usuario, String titulo, String origem, Int32 idAss);
    }
}
