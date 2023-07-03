using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICargoRepository : IRepositoryBase<CARGO>
    {
        CARGO CheckExist(CARGO item, Int32 idAss);
        List<CARGO> GetAllItens(Int32 idAss);
        CARGO GetItemById(Int32 id);
        List<CARGO> GetAllItensAdm(Int32 idAss);
    }
}
