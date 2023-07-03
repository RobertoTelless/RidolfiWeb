using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFunilEtapaRepository : IRepositoryBase<FUNIL_ETAPA>
    {
        List<FUNIL_ETAPA> GetAllItens();
        FUNIL_ETAPA GetItemById(Int32 id);

        List<FUNIL_ETAPA> GetItensByFunil(Int32 funilId);

    }
}
