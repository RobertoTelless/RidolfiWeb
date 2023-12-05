using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IResultadoRobotRepository : IRepositoryBase<RESULTADO_ROBOT_CUSTO>
    {
        List<RESULTADO_ROBOT_CUSTO> GetAllItens(Int32 idAss);
        RESULTADO_ROBOT_CUSTO GetItemById(Int32 id);
        List<RESULTADO_ROBOT_CUSTO> ExecuteFilter(Int32? tipo, DateTime? inicio, DateTime? final, Int32? usuario, Int32 idAss);

    }
}
