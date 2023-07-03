using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICustoCalendarioRepository : IRepositoryBase<CUSTO_FIXO_CALENDARIO>
    {
        CUSTO_FIXO_CALENDARIO CheckExist(CUSTO_FIXO_CALENDARIO item, Int32 idAss);
        List<CUSTO_FIXO_CALENDARIO> GetAllItens(Int32 idAss);
        CUSTO_FIXO_CALENDARIO GetItemById(Int32 id);
    }
}
