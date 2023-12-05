using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILogBackupRepository : IRepositoryBase<LOG_BACKUP>
    {
        LOG_BACKUP GetById(Int32 id);
        List<LOG_BACKUP> GetAllItens(Int32 idAss);
        List<LOG_BACKUP> ExecuteFilter(Int32? usuId, DateTime? data, String operacao, Int32 idAss);
        List<LOG_BACKUP> GetAllItensUsuario(Int32 id, Int32 idAss);
    }
}
