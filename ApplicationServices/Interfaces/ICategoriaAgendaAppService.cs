using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaAgendaAppService : IAppServiceBase<CATEGORIA_AGENDA>
    {
        Int32 ValidateCreate(CATEGORIA_AGENDA item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_AGENDA item, CATEGORIA_AGENDA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_AGENDA item, CATEGORIA_AGENDA itemAntes);
        Int32 ValidateDelete(CATEGORIA_AGENDA item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_AGENDA item, USUARIO usuario);

        CATEGORIA_AGENDA CheckExist(CATEGORIA_AGENDA item, Int32 idAss);
        List<CATEGORIA_AGENDA> GetAllItens(Int32 idAss);
        CATEGORIA_AGENDA GetItemById(Int32 id);
        List<CATEGORIA_AGENDA> GetAllItensAdm(Int32 idAss);
    }
}
