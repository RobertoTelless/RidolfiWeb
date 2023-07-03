using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrupoContatoRepository : IRepositoryBase<GRUPO_CLIENTE>
    {
        List<GRUPO_CLIENTE> GetAllItens();
        GRUPO_CLIENTE GetItemById(Int32 id);
        GRUPO_CLIENTE CheckExist(GRUPO_CLIENTE item);

    }
}
