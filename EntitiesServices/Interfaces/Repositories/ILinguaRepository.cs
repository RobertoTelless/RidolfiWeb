using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILinguaRepository : IRepositoryBase<LINGUA>
    {
        List<LINGUA> GetAllItens();
        LINGUA GetItemById(Int32 id);
    }
}
