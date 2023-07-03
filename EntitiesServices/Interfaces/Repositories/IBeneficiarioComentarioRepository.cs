using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IBeneficiarioComentarioRepository : IRepositoryBase<BENEFICIARIO_ANOTACOES>
    {
        List<BENEFICIARIO_ANOTACOES> GetAllItens();
        BENEFICIARIO_ANOTACOES GetItemById(Int32 id);
    }
}
