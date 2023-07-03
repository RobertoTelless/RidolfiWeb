using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVideoComentarioRepository : IRepositoryBase<VIDEO_COMENTARIO>
    {
        List<VIDEO_COMENTARIO> GetAllItens();
        VIDEO_COMENTARIO GetItemById(Int32 id);
    }
}
