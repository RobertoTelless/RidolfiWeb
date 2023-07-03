using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITabelaIPCARepository : IRepositoryBase<TABELA_IPCA>
    {
        TABELA_IPCA CheckExist(TABELA_IPCA item);
        TABELA_IPCA GetItemById(Int32 id);
        List<TABELA_IPCA> GetAllItens();
        List<TABELA_IPCA> GetAllItensAdm();
        List<TABELA_IPCA> ExecuteFilter(DateTime? data);
    }
}
