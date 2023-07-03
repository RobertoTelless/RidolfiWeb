using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IVideoService : IServiceBase<VIDEO>
    {
        Int32 Create(VIDEO item, LOG log);
        Int32 Create(VIDEO item);
        Int32 Edit(VIDEO item, LOG log);
        Int32 Edit(VIDEO item);
        Int32 Delete(VIDEO item, LOG log);

        VIDEO GetItemById(Int32 id);
        List<VIDEO> GetAllItens(Int32 idAss);
        List<VIDEO> GetAllItensAdm(Int32 idAss);
        List<VIDEO> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss);
        List<VIDEO> GetAllItensValidos(Int32 idAss);
        VIDEO_COMENTARIO GetComentarioById(Int32 id);

    }
}
