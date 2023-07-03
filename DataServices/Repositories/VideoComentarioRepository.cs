using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class VideoComentarioRepository : RepositoryBase<VIDEO_COMENTARIO>, IVideoComentarioRepository
    {
        public List<VIDEO_COMENTARIO> GetAllItens()
        {
            return Db.VIDEO_COMENTARIO.ToList();
        }

        public VIDEO_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<VIDEO_COMENTARIO> query = Db.VIDEO_COMENTARIO.Where(p => p.VICO_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 