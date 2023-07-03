using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAssinantePlanoRepository : IRepositoryBase<ASSINANTE_PLANO>
    {
        List<ASSINANTE_PLANO> GetAllItens();
        ASSINANTE_PLANO GetItemById(Int32 id);
        ASSINANTE_PLANO GetByAssPlan(Int32 plan, Int32 assi);
    }
}
