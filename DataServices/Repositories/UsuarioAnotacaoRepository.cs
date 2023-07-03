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
    public class UsuarioAnotacaoRepository : RepositoryBase<USUARIO_ANOTACAO>, IUsuarioAnotacaoRepository
    {
        public List<USUARIO_ANOTACAO> GetAllItens()
        {
            return Db.USUARIO_ANOTACAO.ToList();
        }

        public USUARIO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<USUARIO_ANOTACAO> query = Db.USUARIO_ANOTACAO.Where(p => p.USAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 