using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEstadoCivilRepository : IRepositoryBase<ESTADO_CIVIL>
    {
        List<ESTADO_CIVIL> GetAllItens();
        ESTADO_CIVIL GetItemById(Int32 id);
    }
}
