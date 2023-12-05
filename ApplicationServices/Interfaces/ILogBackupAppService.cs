using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ILogBackupAppService : IAppServiceBase<LOG_BACKUP>
    {
        LOG_BACKUP GetById(Int32 id);
        List<LOG_BACKUP> GetAllItens(Int32 idAss);
        Tuple<Int32, List<LOG_BACKUP>, Boolean> ExecuteFilterTuple(Int32? usuId, DateTime? data, String operacao, Int32 idAss);
        List<LOG_BACKUP> GetAllItensUsuario(Int32 id, Int32 idAss);
        Int32 ValidateCreate(LOG_BACKUP perfil);
    }
}
