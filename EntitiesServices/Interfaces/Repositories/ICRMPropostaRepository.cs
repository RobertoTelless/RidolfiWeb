using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPropostaRepository : IRepositoryBase<CRM_PROPOSTA>
    {
        List<CRM_PROPOSTA> GetAllItens(Int32 idAss);
        CRM_PROPOSTA GetItemById(Int32 id);
    }
}
