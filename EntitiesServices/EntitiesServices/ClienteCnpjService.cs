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
    public class ClienteCnpjService : ServiceBase<CLIENTE_QUADRO_SOCIETARIO>, IClienteCnpjService
    {
        private readonly IClienteCnpjRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public ClienteCnpjService(IClienteCnpjRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
        }

        public CLIENTE_QUADRO_SOCIETARIO CheckExist(CLIENTE_QUADRO_SOCIETARIO cqs)
        {
            CLIENTE_QUADRO_SOCIETARIO item = _baseRepository.CheckExist(cqs);
            return item;
        }

        public List<CLIENTE_QUADRO_SOCIETARIO> GetAllItens()
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lista = _baseRepository.GetAllItens();
            return lista;
        }

        public List<CLIENTE_QUADRO_SOCIETARIO> GetByCliente(CLIENTE cliente)
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lista = _baseRepository.GetByCliente(cliente);
            return lista;
        }

        public Int32 Create(CLIENTE_QUADRO_SOCIETARIO cqs, LOG log)
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

        public Int32 Create(CLIENTE_QUADRO_SOCIETARIO cqs)
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