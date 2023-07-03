using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICustoHistoricoRepository : IRepositoryBase<CUSTO_HISTORICO>
    {
        CUSTO_HISTORICO GetItemById(Int32 id);
        List<CUSTO_HISTORICO> GetAllItens(Int32 idAss);
        List<CUSTO_HISTORICO> ExecuteFilter(DateTime? data, Int32? idAss);
    }
}

