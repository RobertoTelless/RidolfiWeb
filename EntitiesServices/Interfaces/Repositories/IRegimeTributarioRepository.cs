using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRegimeTributarioRepository : IRepositoryBase<REGIME_TRIBUTARIO>
    {
        REGIME_TRIBUTARIO CheckExist(REGIME_TRIBUTARIO item);
        List<REGIME_TRIBUTARIO> GetAllItens();
        REGIME_TRIBUTARIO GetItemById(Int32 id);
        List<REGIME_TRIBUTARIO> GetAllItensAdm();
    }
}
