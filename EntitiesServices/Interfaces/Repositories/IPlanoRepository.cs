using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPlanoRepository : IRepositoryBase<PLANO>
    {
        PLANO CheckExist(PLANO item);
        PLANO GetItemById(Int32 id);
        List<PLANO> GetAllItens();
        List<PLANO> GetAllItensAdm();
        List<PLANO> GetAllValidos();
        List<PLANO> ExecuteFilter(String nome, String descricao);
    }
}
