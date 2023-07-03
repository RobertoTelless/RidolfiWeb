using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IGrupoService : IServiceBase<GRUPO>
    {
        Int32 Create(GRUPO item, LOG log);
        Int32 Create(GRUPO item);
        Int32 Edit(GRUPO item, LOG log);
        Int32 Edit(GRUPO item);
        Int32 Delete(GRUPO item, LOG log);

        GRUPO CheckExist(GRUPO item, Int32 idAss);
        GRUPO GetItemById(Int32 id);
        List<GRUPO> GetAllItens(Int32 idAss);
        List<GRUPO> GetAllItensAdm(Int32 idAss);

        GRUPO_CLIENTE GetContatoById(Int32 id);
        Int32 CreateContato(GRUPO_CLIENTE item);
        Int32 EditContato(GRUPO_CLIENTE item);
        GRUPO_CLIENTE CheckExistContato(GRUPO_CLIENTE item);
        List<CLIENTE> FiltrarContatos(GRUPO grupo, Int32 idAss);
    }
}
