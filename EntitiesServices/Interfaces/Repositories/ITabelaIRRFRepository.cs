using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITabelaIRRFRepository : IRepositoryBase<TABELA_IRRF>
    {
        TABELA_IRRF CheckExist(TABELA_IRRF item);
        TABELA_IRRF GetItemById(Int32 id);
        List<TABELA_IRRF> GetAllItens();
        List<TABELA_IRRF> GetAllItensAdm();
    }
}
