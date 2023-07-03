using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class TemplatePropostaService : ServiceBase<TEMPLATE_PROPOSTA>, ITemplatePropostaService
    {
        private readonly ITemplatePropostaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public TemplatePropostaService(ITemplatePropostaRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;

        }

        public TEMPLATE_PROPOSTA GetItemById(Int32 id)
        {
            TEMPLATE_PROPOSTA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<TEMPLATE_PROPOSTA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<TEMPLATE_PROPOSTA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public TEMPLATE_PROPOSTA CheckExist(TEMPLATE_PROPOSTA item, Int32 idAss)
        {
            TEMPLATE_PROPOSTA volta = _baseRepository.CheckExist(item, idAss);
            return volta;
        }

        public TEMPLATE_PROPOSTA GetByCode(String sigla, Int32 idAss)
        {
            TEMPLATE_PROPOSTA item = _baseRepository.GetByCode(sigla, idAss);
            return item;
        }

        public List<TEMPLATE_PROPOSTA> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(sigla, nome, conteudo, idAss);

        }

        public Int32 Create(TEMPLATE_PROPOSTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(TEMPLATE_PROPOSTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(TEMPLATE_PROPOSTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    TEMPLATE_PROPOSTA obj = _baseRepository.GetById(item.TEPR_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(TEMPLATE_PROPOSTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    TEMPLATE_PROPOSTA obj = _baseRepository.GetById(item.TEPR_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(TEMPLATE_PROPOSTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
