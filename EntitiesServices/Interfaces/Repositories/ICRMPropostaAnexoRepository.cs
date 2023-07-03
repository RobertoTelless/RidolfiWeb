using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPropostaAnexoRepository : IRepositoryBase<CRM_PROPOSTA_ANEXO>
    {
        List<CRM_PROPOSTA_ANEXO> GetAllItens();
        CRM_PROPOSTA_ANEXO GetItemById(Int32 id);
    }
}
