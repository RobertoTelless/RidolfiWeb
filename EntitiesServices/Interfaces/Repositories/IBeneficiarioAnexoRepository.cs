using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IBeneficiarioAnexoRepository : IRepositoryBase<BENEFICIARIO_ANEXO>
    {
        List<BENEFICIARIO_ANEXO> GetAllItens();
        BENEFICIARIO_ANEXO GetItemById(Int32 id);
    }
}
