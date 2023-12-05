using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using EntitiesServices.WorkClasses;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MensagemRepository : RepositoryBase<MENSAGENS>, IMensagemRepository
    {
        public MENSAGENS CheckExist(MENSAGENS conta, Int32 idAss)
        {
            IQueryable<MENSAGENS> query = Db.MENSAGENS;
            query = query.Where(p => p.MENS_DT_CRIACAO == conta.MENS_DT_CRIACAO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public MENSAGENS GetItemById(Int32 id)
        {
            IQueryable<MENSAGENS> query = Db.MENSAGENS;
            query = query.Where(p => p.MENS_CD_ID == id);
            query = query.Include(p => p.MENSAGENS_DESTINOS);
            query = query.Include(p => p.RECURSIVIDADE);
            return query.FirstOrDefault();
        }

        public List<MENSAGENS> GetAllItens(Int32 idAss)
        {
            IQueryable<MENSAGENS> query = Db.MENSAGENS.Where(p => p.MENS_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.MENS_DT_CRIACAO);
            return query.ToList();
        }

        public List<MENSAGENS> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MENSAGENS> query = Db.MENSAGENS;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.MENS_DT_CRIACAO);
            return query.ToList();
        }


        public List<MENSAGENS> ExecuteFilterSMS(DateTime? envio,  DateTime? faixa, Int32 cliente, String texto, Int32 idAss)
        {
            List<MENSAGENS> lista = new List<MENSAGENS>();
            IQueryable<MENSAGENS> query = Db.MENSAGENS;
            if (!String.IsNullOrEmpty(texto))
            {
                query = query.Where(p => p.MENS_TX_TEXTO.Contains(texto));
            }
            if ((envio != DateTime.MinValue & envio != null) & (faixa == DateTime.MinValue || faixa == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MENS_DT_CRIACAO) >= DbFunctions.TruncateTime(envio));
            }
            if ((envio == DateTime.MinValue || envio == null) & (faixa != DateTime.MinValue & faixa != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MENS_DT_CRIACAO) <= DbFunctions.TruncateTime(faixa));
            }
            if ((envio != DateTime.MinValue & envio != null) & (faixa != DateTime.MinValue & faixa != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MENS_DT_CRIACAO) >= DbFunctions.TruncateTime(envio) & DbFunctions.TruncateTime(p.MENS_DT_CRIACAO) <= DbFunctions.TruncateTime(faixa));
            }
            if (cliente > 0)
            {
                query = query.Where(p => p.CLIE_CD_ID == cliente);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.MENS_IN_TIPO == 2);
                query = query.OrderBy(a => a.MENS_DT_ENVIO);
                lista = query.ToList<MENSAGENS>();
            }
            return lista;
        }

        public List<MENSAGENS> ExecuteFilterEMail(DateTime? envio,  DateTime? faixa, Int32 cliente, String texto, Int32 idAss)
        {
            List<MENSAGENS> lista = new List<MENSAGENS>();
            IQueryable<MENSAGENS> query = Db.MENSAGENS;
            if (!String.IsNullOrEmpty(texto))
            {
                query = query.Where(p => p.MENS_TX_TEXTO.Contains(texto));
            }
            if ((envio != DateTime.MinValue & envio != null) & (faixa == DateTime.MinValue || faixa == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MENS_DT_CRIACAO) >= DbFunctions.TruncateTime(envio));
            }
            if ((envio == DateTime.MinValue || envio == null) & (faixa != DateTime.MinValue & faixa != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MENS_DT_CRIACAO) <= DbFunctions.TruncateTime(faixa));
            }
            if ((envio != DateTime.MinValue & envio != null) & (faixa != DateTime.MinValue & faixa != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MENS_DT_CRIACAO) >= DbFunctions.TruncateTime(envio) & DbFunctions.TruncateTime(p.MENS_DT_CRIACAO) <= DbFunctions.TruncateTime(faixa));
            }
            if (cliente > 0)
            {
                query = query.Where(p => p.CLIE_CD_ID == cliente);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.MENS_IN_TIPO == 1);
                query = query.OrderBy(a => a.MENS_DT_ENVIO);
                lista = query.ToList<MENSAGENS>();
            }
            return lista;
        }

        public List<MENSAGENS> FilterMensagensNotSend(Int32? mensID, Enumerador.MensagemTipo mensagemTipo)
        {
            IQueryable<MENSAGENS> query = null;

            if (mensID == null)
            {
                query = Db.MENSAGENS.Where(m => (Enumerador.StatusEnvioEmail)m.MENS_IN_STATUS == Enumerador.StatusEnvioEmail.AGUARDANDO_ENVIO 
                && m.MENS_IN_TIPO == (int)mensagemTipo
                && m.MENS_IN_ATIVO == 1);
            }
            else
            {
                query = Db.MENSAGENS.Where(m => m.MENS_CD_ID == mensID);
            }

            
            query = query.Include(m => m.USUARIO);
            query = query.Include(m => m.CLIENTE);
            query = query.Include(m => m.GRUPO);
            query = query.Include(m => m.GRUPO.GRUPO_CLIENTE);
            query = query.Include(m => m.MENSAGEM_ANEXO);          
            

            return query.ToList<MENSAGENS>().Where(x=> x.MENS_DT_AGENDAMENTO <= DateTime.Now).ToList();
        }
    }
}
 