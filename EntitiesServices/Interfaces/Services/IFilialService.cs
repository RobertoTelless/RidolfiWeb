using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFilialService : IServiceBase<FILIAL>
    {
        Int32 Create(FILIAL perfil, LOG log);
        Int32 Create(FILIAL perfil);
        Int32 Edit(FILIAL perfil, LOG log);
        Int32 Edit(FILIAL perfil);
        Int32 Delete(FILIAL perfil, LOG log);
        
        FILIAL CheckExist(FILIAL item, Int32 idAss);
        FILIAL GetItemById(Int32 id);
        List<FILIAL> GetAllItens(Int32 idAss);
        List<FILIAL> GetAllItensAdm(Int32 idAss);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        List<TIPO_PESSOA> GetAllTiposPessoa();
    }
}
