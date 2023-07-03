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
    public class TipoAcaoRepository : RepositoryBase<TIPO_ACAO>, ITipoAcaoRepository
    {
        public TIPO_ACAO CheckExist(TIPO_ACAO conta, Int32 idAss)
        {
            IQueryable<TIPO_ACAO> query = Db.TIPO_ACAO;
            query = query.Where(p => p.TIAC_NM_NOME == conta.TIAC_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_ACAO GetItemById(Int32 id)
        {
            IQueryable<TIPO_ACAO> query = Db.TIPO_ACAO;
            query = query.Where(p => p.TIAC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_ACAO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_ACAO> query = Db.TIPO_ACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_ACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_ACAO> query = Db.TIPO_ACAO.Where(p => p.TIAC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 