using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITRFRepository : IRepositoryBase<TRF>
    {
        TRF CheckExist(TRF conta);
        TRF GetItemById(Int32 id);
        List<TRF> GetAllItens();
        List<TRF> GetAllItensAdm();
        List<TRF> ExecuteFilter(String nome, Int32? uf);
    }
}

