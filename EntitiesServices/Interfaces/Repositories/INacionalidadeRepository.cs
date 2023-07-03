using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INacionalidadeRepository : IRepositoryBase<NACIONALIDADE>
    {
        List<NACIONALIDADE> GetAllItens();
        NACIONALIDADE GetItemById(Int32 id);
    }
}
