using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class QualificacaoRepository : RepositoryBase<QUALIFICACAO>, IQualificacaoRepository
    {
        public QUALIFICACAO CheckExist(String conta)
        {
            IQueryable<QUALIFICACAO> query = Db.QUALIFICACAO;
            query = query.Where(p => p.QUAL_NM_NOME == conta);
            return query.FirstOrDefault();
        }

        public QUALIFICACAO GetItemById(Int32 id)
        {
            IQueryable<QUALIFICACAO> query = Db.QUALIFICACAO;
            query = query.Where(p => p.QUAL_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<QUALIFICACAO> GetAllItens()
        {
            IQueryable<QUALIFICACAO> query = Db.QUALIFICACAO.Where(p => p.QUAL_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<QUALIFICACAO> GetAllItensAdm()
        {
            IQueryable<QUALIFICACAO> query = Db.QUALIFICACAO;
            return query.ToList();
        }
    }
}
