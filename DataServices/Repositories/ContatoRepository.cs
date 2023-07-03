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
    public class ContatoRepository : RepositoryBase<CONTATO>, IContatoRepository
    {
        public CONTATO CheckExistProt(String conta)
        {
            IQueryable<CONTATO> query = Db.CONTATO;
            if (conta != null)
            {
                query = query.Where(p => p.CONT_NR_PROTOCOLO == conta);
            }
            return query.FirstOrDefault();
        }

        public List<CONTATO> GetAllItens()
        {
            IQueryable<CONTATO> query = Db.CONTATO.Where(p => p.CONT_IN_ATIVO == 1);
            query = query.OrderByDescending(a => a.CONT_DT_CONTATO);
            return query.ToList();
        }

        public CONTATO GetItemById(Int32 id)
        {
            IQueryable<CONTATO> query = Db.CONTATO.Where(p => p.CONT_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONTATO> GetAllItensAdm()
        {
            IQueryable<CONTATO> query = Db.CONTATO;
            query = query.OrderByDescending(a => a.CONT_DT_CONTATO);
            return query.ToList();
        }

        public List<CONTATO> ExecuteFilter(Int32? precatorio, Int32? beneficiario, Int32? quali, Int32? quem, String protocolo, DateTime? inicio, DateTime? final, String telefone,  String agente, String campanha)
        {
            List<CONTATO> lista = new List<CONTATO>();
            IQueryable<CONTATO> query = Db.CONTATO;
            if (precatorio > 0)
            {
                query = query.Where(p => p.PREC_CD_ID == precatorio);
            }
            if (beneficiario > 0)
            {
                query = query.Where(p => p.BENE_CD_ID == beneficiario);
            }
            if (quali > 0)
            {
                query = query.Where(p => p.QUAL_CD_ID == quali);
            }
            if (quem > 0)
            {
                query = query.Where(p => p.QUDE_CD_ID == quem);
            }
            if (!String.IsNullOrEmpty(protocolo))
            {
                query = query.Where(p => p.CONT_NR_PROTOCOLO == protocolo);
            }
            if (!String.IsNullOrEmpty(telefone))
            {
                query = query.Where(p => p.CONT_NR_TELEFONE == telefone);
            }
            if (!String.IsNullOrEmpty(agente))
            {
                query = query.Where(p => p.CONT_NM_AGENTE == agente);
            }
            if (!String.IsNullOrEmpty(campanha))
            {
                query = query.Where(p => p.CONT_NM_CAMPANHA == campanha);
            }
            if (inicio != null & final == null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CONT_DT_CONTATO) >= DbFunctions.TruncateTime(inicio));
            }
            if (inicio == null & final != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CONT_DT_CONTATO) <= DbFunctions.TruncateTime(final));
            }
            if (inicio != null & final != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CONT_DT_CONTATO) >= DbFunctions.TruncateTime(inicio) & DbFunctions.TruncateTime(p.CONT_DT_CONTATO) <= DbFunctions.TruncateTime(final));
            }
            if (query != null)
            {
                query = query.OrderByDescending(a => a.CONT_DT_CONTATO);
                lista = query.ToList<CONTATO>();
            }
            return lista;
        }
    }
}
 