using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICustoFixoRepository : IRepositoryBase<CUSTO_FIXO>
    {
        CUSTO_FIXO CheckExist(CUSTO_FIXO item, Int32 idAss);
        CUSTO_FIXO GetItemById(Int32 id);
        List<CUSTO_FIXO> GetAllItens(Int32 idAss);
        List<CUSTO_FIXO> GetAllItensAdm(Int32 idAss);
        List<CUSTO_FIXO> ExecuteFilter(Int32? catId, String nome, DateTime? dataInicio, DateTime? dataFinal, Int32 idAss);

        Task<List<CUSTO_FIXO>> GetAllItensAsync(Int32 idAss);

    }
}
