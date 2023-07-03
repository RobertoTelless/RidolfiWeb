using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFilialRepository : IRepositoryBase<FILIAL>
    {
        FILIAL CheckExist(FILIAL item, Int32 idAss);
        FILIAL GetItemById(Int32 id);
        List<FILIAL> GetAllItens(Int32 idAss);
        List<FILIAL> GetAllItensAdm(Int32 idAss);
    }
}
