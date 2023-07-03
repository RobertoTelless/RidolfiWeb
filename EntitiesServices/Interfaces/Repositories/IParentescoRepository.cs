using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IParentescoRepository : IRepositoryBase<PARENTESCO>
    {
        PARENTESCO CheckExist(PARENTESCO item);
        List<PARENTESCO> GetAllItens();
        PARENTESCO GetItemById(Int32 id);
        List<PARENTESCO> GetAllItensAdm();
    }
}
