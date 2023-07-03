using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICRMDiarioService : IServiceBase<DIARIO_PROCESSO>
    {
        Int32 Create(DIARIO_PROCESSO perfil, LOG log);
        Int32 Create(DIARIO_PROCESSO perfil);
        Int32 Edit(DIARIO_PROCESSO perfil, LOG log);
        Int32 Edit(DIARIO_PROCESSO perfil);
        Int32 Delete(DIARIO_PROCESSO perfil, LOG log);

        DIARIO_PROCESSO GetItemById(Int32 id);
        DIARIO_PROCESSO GetByDate(DateTime data);
        List<DIARIO_PROCESSO> GetAllItens(Int32 idAss);
        List<DIARIO_PROCESSO> ExecuteFilter(Int32? processo, DateTime? inicio, DateTime? final, Int32? usuario, String operacao, String descricao, Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
