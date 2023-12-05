using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IEmpresaService : IServiceBase<EMPRESA>
    {
        Int32 Create(EMPRESA perfil, LOG log);
        Int32 Create(EMPRESA perfil);
        Int32 Edit(EMPRESA perfil, LOG log);
        Int32 Edit(EMPRESA perfil);
        Int32 Delete(EMPRESA perfil, LOG log);

        EMPRESA CheckExist(EMPRESA item, Int32 idAss);
        EMPRESA GetItemById(Int32 id);
        EMPRESA GetItemByAssinante(Int32 id);
        List<EMPRESA> GetAllItens(Int32 idAss);
        List<EMPRESA> GetAllItensAdm(Int32 idAss);
        List<EMPRESA> ExecuteFilter(String nome, Int32? idAss);

        List<REGIME_TRIBUTARIO> GetAllRegimes();
        EMPRESA_ANEXO GetAnexoById(Int32 id);
        REGIME_TRIBUTARIO GetRegimeById(Int32 id);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        Int32 EditAnexo(EMPRESA_ANEXO item);            

   }
}
