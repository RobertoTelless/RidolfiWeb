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
    public class VendaMensalService : ServiceBase<VENDA_MENSAL>, IVendaMensalService
    {
        private readonly IVendaMensalRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IVendaMensalAnexoRepository _aneRepository;
        private readonly IVendaMensalAnotacaoRepository _anoRepository;
        private readonly IVendaMensalComissaoRepository _comRepository;

        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public VendaMensalService(IVendaMensalRepository baseRepository, ILogRepository logRepository, IVendaMensalAnexoRepository aneRepository, IVendaMensalAnotacaoRepository anoRepository, IVendaMensalComissaoRepository comRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _aneRepository = aneRepository;
            _anoRepository = anoRepository;
            _comRepository = comRepository;
        }

        public Int32 EditAnotacao(VENDA_MENSAL_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    VENDA_MENSAL_ANOTACAO obj = _anoRepository.GetById(item.VMAT_CD_ID);
                    _anoRepository.Detach(obj);
                    _anoRepository.Update(item);
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

        public VENDA_MENSAL_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public VENDA_MENSAL_ANEXO GetAnexoById(Int32 id)
        {
            return _aneRepository.GetItemById(id);
        }

        public VENDA_MENSAL CheckExist(VENDA_MENSAL conta, Int32 idAss)
        {
            VENDA_MENSAL item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public VENDA_MENSAL GetItemById(Int32 id)
        {
            VENDA_MENSAL item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<VENDA_MENSAL> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<VENDA_MENSAL> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<VENDA_MENSAL> ExecuteFilter(DateTime? dataRef, Int32? tipo, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(dataRef, tipo, idAss);

        }

        public Int32 Create(VENDA_MENSAL item)
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


        public Int32 Edit(VENDA_MENSAL item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VENDA_MENSAL obj = _baseRepository.GetById(item.VEMA_CD_ID);
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

        public Int32 Edit(VENDA_MENSAL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VENDA_MENSAL obj = _baseRepository.GetById(item.VEMA_CD_ID);
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

        public Int32 Delete(VENDA_MENSAL item, LOG log)
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

        public VENDA_MENSAL_COMISSAO GetComissaoById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public Int32 EditComissao(VENDA_MENSAL_COMISSAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VENDA_MENSAL_COMISSAO obj = _comRepository.GetById(item.VECO_CD_ID);
                    _comRepository.Detach(obj);
                    _comRepository.Update(item);
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

        public Int32 CreateComissao(VENDA_MENSAL_COMISSAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _comRepository.Add(item);
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
