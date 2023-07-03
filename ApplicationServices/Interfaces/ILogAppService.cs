using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ILogAppService : IAppServiceBase<LOG>
    {
        LOG GetById(Int32 id);
        List<LOG> GetAllItens(Int32 idAss);
        Tuple<Int32, List<LOG>, Boolean> ExecuteFilterTuple(Int32? usuId, DateTime? data, String operacao, Int32 idAss);
        List<LOG> GetAllItensDataCorrente(Int32 idAss);
        List<LOG> GetAllItensMesCorrente(Int32 idAss);
        List<LOG> GetAllItensMesAnterior(Int32 idAss);
        List<LOG> GetAllItensUsuario(Int32 id, Int32 idAss);

    }
}
