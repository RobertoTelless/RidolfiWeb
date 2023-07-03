using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFunilRepository : IRepositoryBase<FUNIL>
    {
        FUNIL CheckExist(FUNIL item, Int32 idAss);
        FUNIL GetItemById(Int32 id);
        FUNIL GetItemBySigla(String sigla, Int32 idAss);
        List<FUNIL> GetAllItens(Int32 idAss);
        List<FUNIL> GetAllItensAdm(Int32 idAss);
    }
}
