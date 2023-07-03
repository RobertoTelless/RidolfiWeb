using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IQualificacaoRepository : IRepositoryBase<QUALIFICACAO>
    {
        QUALIFICACAO CheckExist(String quali);
        List<QUALIFICACAO> GetAllItens();
        QUALIFICACAO GetItemById(Int32 id);
        List<QUALIFICACAO> GetAllItensAdm();
    }
}
