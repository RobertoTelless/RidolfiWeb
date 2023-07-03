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
    public class GrupoService : ServiceBase<GRUPO>, IGrupoService
    {
        private readonly IGrupoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IGrupoContatoRepository _contRepository;
        private readonly IClienteRepository _cliRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public GrupoService(IGrupoRepository baseRepository, ILogRepository logRepository, IGrupoContatoRepository contRepository, IClienteRepository cliRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _contRepository = contRepository;
            _cliRepository = cliRepository;
        }

        public GRUPO CheckExist(GRUPO conta, Int32 idAss)
        {
            GRUPO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public GRUPO_CLIENTE CheckExistContato(GRUPO_CLIENTE conta)
        {
            GRUPO_CLIENTE item = _contRepository.CheckExist(conta);
            return item;
        }

        public GRUPO GetItemById(Int32 id)
        {
            GRUPO item = _baseRepository.GetItemById(id);
            return item;
        }

        public GRUPO_CLIENTE GetContatoById(Int32 id)
        {
            return _contRepository.GetItemById(id);
        }

        public List<GRUPO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CLIENTE> FiltrarContatos(GRUPO grupo, Int32 idAss)
        {
            return _cliRepository.FiltrarContatos(grupo, idAss);
        }

        public List<GRUPO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public Int32 Create(GRUPO item, LOG log)
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

        public Int32 Create(GRUPO item)
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


        public Int32 Edit(GRUPO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    GRUPO obj = _baseRepository.GetById(item.GRUP_CD_ID);
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

        public Int32 Edit(GRUPO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    GRUPO obj = _baseRepository.GetById(item.GRUP_CD_ID);
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

        public Int32 Delete(GRUPO item, LOG log)
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

        public Int32 CreateContato(GRUPO_CLIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _contRepository.Add(item);
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

        public Int32 EditContato(GRUPO_CLIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    GRUPO_CLIENTE obj = _contRepository.GetById(item.GRCL_CD_ID);
                    _contRepository.Detach(obj);
                    _contRepository.Update(item);
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
