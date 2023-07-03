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
    public class EnderecoRepository : RepositoryBase<ENDERECO>, IEnderecoRepository
    {
        public List<ENDERECO> GetAllItens()
        {
            return Db.ENDERECO.ToList();
        }

        public ENDERECO GetItemById(Int32 id)
        {
            IQueryable<ENDERECO> query = Db.ENDERECO.Where(p => p.ENDE_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 