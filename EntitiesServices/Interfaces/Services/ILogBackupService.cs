using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ILogBackupService : IServiceBase<LOG_BACKUP>
    {
        LOG_BACKUP GetById(Int32 id);
        List<LOG_BACKUP> GetAllItens(Int32 idAss);
        List<LOG_BACKUP> ExecuteFilter(Int32? usuId, DateTime? data, String operacao, Int32 idAss);

        List<LOG_BACKUP> GetAllItensUsuario(Int32 id, Int32 idAss);
        Int32 Create(LOG_BACKUP perfil);
        Int32 Delete(LOG_BACKUP item);

    }
}
