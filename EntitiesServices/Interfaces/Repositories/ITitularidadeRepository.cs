using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITitularidadeRepository : IRepositoryBase<TITULARIDADE>
    {
        TITULARIDADE CheckExist(TITULARIDADE item);
        List<TITULARIDADE> GetAllItens();
        TITULARIDADE GetItemById(Int32 id);
        List<TITULARIDADE> GetAllItensAdm();
    }
}
