using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INaturezaRepository : IRepositoryBase<NATUREZA>
    {
        List<NATUREZA> GetAllItens();
        NATUREZA GetItemById(Int32 id);
        List<NATUREZA> GetAllItensAdm();
        NATUREZA CheckExist(NATUREZA natureza);
        NATUREZA CheckExist(String natureza);
    }
}
