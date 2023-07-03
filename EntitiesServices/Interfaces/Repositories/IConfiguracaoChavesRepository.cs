using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IConfiguracaoChavesRepository : IRepositoryBase<CONFIGURACAO_CHAVES>
    {
        CONFIGURACAO_CHAVES GetItemById(Int32 id);
        List<CONFIGURACAO_CHAVES> GetAllItems(Int32 idAss);
    }
}
