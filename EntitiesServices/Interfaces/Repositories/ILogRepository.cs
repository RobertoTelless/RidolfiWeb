using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILogRepository : IRepositoryBase<LOG>
    {
        LOG GetById(Int32 id);
        List<LOG> GetAllItens(Int32 idAss);
        List<LOG> ExecuteFilter(Int32? usuId, DateTime? data, String operacao, Int32 idAss);
        List<LOG> GetAllItensDataCorrente(Int32 idAss);
        List<LOG> GetAllItensMesCorrente(Int32 idAss);
        List<LOG> GetAllItensMesAnterior(Int32 idAss);
        List<LOG> GetAllItensUsuario(Int32 id, Int32 idAss);
        List<LOG> GetLogByFaixa(DateTime inicio, DateTime final, Int32 idAss);
    }
}
