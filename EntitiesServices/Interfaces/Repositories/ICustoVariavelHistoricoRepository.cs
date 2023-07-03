using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICustoVariavelHistoricoRepository : IRepositoryBase<CUSTO_VARIAVEL_HISTORICO>
    {
        CUSTO_VARIAVEL_HISTORICO GetItemById(Int32 id);
        List<CUSTO_VARIAVEL_HISTORICO> GetAllItens(Int32 idAss);
        List<CUSTO_VARIAVEL_HISTORICO> ExecuteFilter(DateTime? data, Int32? idAss);
    }
}

