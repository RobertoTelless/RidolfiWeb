using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRecursividadeDataRepository : IRepositoryBase<RECURSIVIDADE_DATA>
    {
        List<RECURSIVIDADE_DATA> GetAllItens();
        RECURSIVIDADE_DATA GetItemById(Int32 id);
        List<RECURSIVIDADE_DATA> GetItensByData(DateTime data);
    }
}
