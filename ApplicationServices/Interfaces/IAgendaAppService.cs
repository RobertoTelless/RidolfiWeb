using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAgendaAppService : IAppServiceBase<AGENDA>
    {
        Int32 ValidateCreate(AGENDA perfil, USUARIO usuario);
        Int32 ValidateEdit(AGENDA perfil, AGENDA perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(AGENDA perfil, USUARIO usuario);
        Int32 ValidateEdit(AGENDA perfil);
        Int32 ValidateDelete(AGENDA perfil, USUARIO usuario);
        Int32 ValidateReativar(AGENDA perfil, USUARIO usuario);

        List<CATEGORIA_AGENDA> GetAllTipos(Int32 idAss);
        AGENDA_ANEXO GetAnexoById(Int32 id);

        List<AGENDA> GetByDate(DateTime data, Int32 idAss);
        List<AGENDA> GetByUser(Int32 id, Int32 idAss);
        List<AGENDA> GetAllItens(Int32 idAss);
        List<AGENDA> GetAllItensAdm(Int32 idAss);
        AGENDA GetItemById(Int32 id);
        Int32 ExecuteFilter(DateTime? data, Int32? cat, String titulo, String descricao, Int32 idAss, Int32 idUser, Int32 corp, out List<AGENDA> objeto);
        Task<IEnumerable<CATEGORIA_AGENDA>> GetAllItensAsync(Int32 idAss);
        Tuple<Int32, List<AGENDA>, Boolean> ExecuteFilterTuple(DateTime? data, Int32? cat, String titulo, String descricao, Int32 idAss, Int32 idUser, Int32 corp);
        Int32 ValidateEditAnexo(AGENDA_ANEXO item);

        AGENDA_CONTATO GetContatoById(Int32 id);
        Int32 ValidateEditContato(AGENDA_CONTATO item);
        Int32 ValidateCreateContato(AGENDA_CONTATO item);

    }
}
