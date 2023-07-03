using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPeriodicidadePlanoRepository : IRepositoryBase<PLANO_PERIODICIDADE>
    {
        List<PLANO_PERIODICIDADE> GetAllItens();
        PLANO_PERIODICIDADE GetItemById(Int32 id);
        List<PLANO_PERIODICIDADE> GetAllItensAdm();
    }
}
