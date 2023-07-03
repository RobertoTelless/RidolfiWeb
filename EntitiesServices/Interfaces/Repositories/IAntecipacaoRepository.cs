using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAntecipacaoRepository : IRepositoryBase<ANTECIPACAO>
    {
        ANTECIPACAO CheckExist(ANTECIPACAO item, Int32 idEmpr);
        List<ANTECIPACAO> GetAllItens(Int32 idEmpr);
        ANTECIPACAO GetItemById(Int32 id);

    }
}
