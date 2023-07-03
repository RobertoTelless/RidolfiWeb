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
    public class AssinanteCnpjService : ServiceBase<ASSINANTE_QUADRO_SOCIETARIO>, IAssinanteCnpjService
    {
        private readonly IAssinanteCnpjRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public AssinanteCnpjService(IAssinanteCnpjRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
        }

        public ASSINANTE_QUADRO_SOCIETARIO CheckExist(ASSINANTE_QUADRO_SOCIETARIO cqs)
        {
            ASSINANTE_QUADRO_SOCIETARIO item = _baseRepository.CheckExist(cqs);
            return item;
        }

        public List<ASSINANTE_QUADRO_SOCIETARIO> GetAllItens()
        {
            List<ASSINANTE_QUADRO_SOCIETARIO> lista = _baseRepository.GetAllItens();
            return lista;
        }

        public List<ASSINANTE_QUADRO_SOCIETARIO> GetByCliente(ASSINANTE cliente)
        {
            List<ASSINANTE_QUADRO_SOCIETARIO> lista = _baseRepository.GetByCliente(cliente);
            return lista;
        }

        public Int32 Create(ASSINANTE_QUADRO_SOCIETARIO cqs, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(cqs);
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

        public Int32 Create(ASSINANTE_QUADRO_SOCIETARIO cqs)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(cqs);
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
