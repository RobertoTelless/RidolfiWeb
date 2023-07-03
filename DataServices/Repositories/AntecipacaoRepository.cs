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
using CrossCutting;

namespace DataServices.Repositories
{
    public class AntecipacaoRepository : RepositoryBase<ANTECIPACAO>, IAntecipacaoRepository
    {
        public ANTECIPACAO CheckExist(ANTECIPACAO tarefa, Int32 idEmpr)
        {
            IQueryable<ANTECIPACAO> query = Db.ANTECIPACAO;
            query = query.Where(p => p.MAQN_CD_ID == tarefa.MAQN_CD_ID);
            query = query.Where(p => p.ANTE_DT_INICIO == tarefa.ANTE_DT_INICIO);
            query = query.Where(p => p.EMPR_CD_ID == tarefa.EMPR_CD_ID);
            return query.FirstOrDefault();
        }

        public ANTECIPACAO GetItemById(Int32 id)
        {
            IQueryable<ANTECIPACAO> query = Db.ANTECIPACAO;
            query = query.Where(p => p.ANTE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<ANTECIPACAO> GetAllItens(Int32 idUsu)
        {
            IQueryable<ANTECIPACAO> query = Db.ANTECIPACAO.Where(p => p.ANTE_IN_ATIVO == 1);
            query = query.Where(p => p.EMPR_CD_ID == idUsu);
            return query.ToList();
        }

    }
}
 