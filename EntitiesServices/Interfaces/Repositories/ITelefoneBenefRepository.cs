using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITelefoneBenefRepository : IRepositoryBase<TELEFONE>
    {
        List<TELEFONE> GetAllItens();
        TELEFONE GetItemById(Int32 id);
    }
}
