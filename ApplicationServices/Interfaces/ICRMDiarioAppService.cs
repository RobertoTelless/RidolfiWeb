using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICRMDiarioAppService : IAppServiceBase<DIARIO_PROCESSO>
    {
        Int32 ValidateCreate(DIARIO_PROCESSO perfil);

        DIARIO_PROCESSO GetItemById(Int32 id);
        DIARIO_PROCESSO GetByDate(DateTime data);
        List<DIARIO_PROCESSO> GetAllItens(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);

        Int32 ExecuteFilter(Int32? processo, DateTime? inicio, DateTime? final, Int32? usuario, String operacao, String descricao, Int32 idAss, out List<DIARIO_PROCESSO> objeto);
    }
}
