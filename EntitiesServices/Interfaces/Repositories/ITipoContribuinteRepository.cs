using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoContribuinteRepository : IRepositoryBase<TIPO_CONTRIBUINTE>
    {
        List<TIPO_CONTRIBUINTE> GetAllItens(Int32 idAss);
        TIPO_CONTRIBUINTE GetItemById(Int32 id);

    }
}
