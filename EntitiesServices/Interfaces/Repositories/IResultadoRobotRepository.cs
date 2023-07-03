using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IResultadoRobotRepository : IRepositoryBase<RESULTADO_ROBOT>
    {
        List<RESULTADO_ROBOT> GetAllItens(Int32 idAss);
        RESULTADO_ROBOT GetItemById(Int32 id);
        List<RESULTADO_ROBOT> ExecuteFilter(Int32? tipo, DateTime? inicio, DateTime? final, String cliente, String email, String celular, Int32? status, Int32 idAss);

    }
}
