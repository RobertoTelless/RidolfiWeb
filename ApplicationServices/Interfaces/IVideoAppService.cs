using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IVideoAppService : IAppServiceBase<VIDEO>
    {
        Int32 ValidateCreate(VIDEO item, USUARIO usuario);
        Int32 ValidateEdit(VIDEO item, VIDEO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(VIDEO item, VIDEO itemAntes);
        Int32 ValidateDelete(VIDEO item, USUARIO usuario);
        Int32 ValidateReativar(VIDEO item, USUARIO usuario);

        VIDEO GetItemById(Int32 id);
        List<VIDEO> GetAllItens(Int32 idAss);
        List<VIDEO> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<VIDEO>, Boolean> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss);
        List<VIDEO> GetAllItensValidos(Int32 idAss);
        VIDEO_COMENTARIO GetComentarioById(Int32 id);

    }
}
