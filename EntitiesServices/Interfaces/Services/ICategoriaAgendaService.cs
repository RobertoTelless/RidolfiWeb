using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaAgendaService : IServiceBase<CATEGORIA_AGENDA>
    {
        Int32 Create(CATEGORIA_AGENDA perfil, LOG log);
        Int32 Create(CATEGORIA_AGENDA perfil);
        Int32 Edit(CATEGORIA_AGENDA perfil, LOG log);
        Int32 Edit(CATEGORIA_AGENDA perfil);
        Int32 Delete(CATEGORIA_AGENDA perfil, LOG log);

        CATEGORIA_AGENDA CheckExist(CATEGORIA_AGENDA item, Int32 idAss);
        List<CATEGORIA_AGENDA> GetAllItens(Int32 idAss);
        CATEGORIA_AGENDA GetItemById(Int32 id);
        List<CATEGORIA_AGENDA> GetAllItensAdm(Int32 idAss);
    }
}
