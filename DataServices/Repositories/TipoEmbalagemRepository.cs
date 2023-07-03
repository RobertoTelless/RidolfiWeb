using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class TipoEmbalagemRepository : RepositoryBase<TIPO_EMBALAGEM>, ITipoEmbalagemRepository
    {
        public TIPO_EMBALAGEM CheckExist(TIPO_EMBALAGEM conta, Int32 idAss)
        {
            IQueryable<TIPO_EMBALAGEM> query = Db.TIPO_EMBALAGEM;
            query = query.Where(p => p.TIEM_NM_NOME == conta.TIEM_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_EMBALAGEM GetItemById(Int32 id)
        {
            IQueryable<TIPO_EMBALAGEM> query = Db.TIPO_EMBALAGEM;
            query = query.Where(p => p.TIEM_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_EMBALAGEM> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_EMBALAGEM> query = Db.TIPO_EMBALAGEM;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_EMBALAGEM> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_EMBALAGEM> query = Db.TIPO_EMBALAGEM.Where(p => p.TIEM_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 