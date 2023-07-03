using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IComissaoRepository : IRepositoryBase<COMISSAO>
    {
        COMISSAO GetItemById(Int32 id);
        List<COMISSAO> GetAllItens(Int32 idAss);
        List<COMISSAO> ExecuteFilter(DateTime? data, Int32? idAss);
    }
}

