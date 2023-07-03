using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPropostaComentarioRepository : IRepositoryBase<CRM_PROPOSTA_ACOMPANHAMENTO>
    {
        List<CRM_PROPOSTA_ACOMPANHAMENTO> GetAllItens();
        CRM_PROPOSTA_ACOMPANHAMENTO GetItemById(Int32 id);
    }
}
