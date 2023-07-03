using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMDiarioRepository : IRepositoryBase<DIARIO_PROCESSO>
    {
        DIARIO_PROCESSO GetItemById(Int32 id);
        DIARIO_PROCESSO GetByDate(DateTime data);
        List<DIARIO_PROCESSO> GetAllItens(Int32 idAss);
        List<DIARIO_PROCESSO> ExecuteFilter(Int32? processo, DateTime? inicio, DateTime? final, Int32? usuario, String operacao, String descricao, Int32 idAss);
    }
}
