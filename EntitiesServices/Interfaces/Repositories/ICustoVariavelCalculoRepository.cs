using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICustoVariavelCalculoRepository : IRepositoryBase<CUSTO_VARIAVEL_CALCULO>
    {
        CUSTO_VARIAVEL_CALCULO GetItemById(Int32 id);
        List<CUSTO_VARIAVEL_CALCULO> GetAllItens(Int32 idAss);
        List<CUSTO_VARIAVEL_CALCULO> ExecuteFilter(DateTime? data, Int32? idAss);
    }
}

