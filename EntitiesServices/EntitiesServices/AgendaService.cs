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
    public class AgendaService : ServiceBase<AGENDA>, IAgendaService
    {
        private readonly IAgendaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICategoriaAgendaRepository _tipoRepository;
        private readonly IAgendaAnexoRepository _anexoRepository;
        private readonly IAgendaContatoRepository _contRepository;

        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public AgendaService(IAgendaRepository baseRepository, ILogRepository logRepository, ICategoriaAgendaRepository tipoRepository, IAgendaAnexoRepository anexoRepository, IAgendaContatoRepository contRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _contRepository = contRepository;
        }

        public List<AGENDA> GetByDate(DateTime data, Int32 idAss)
        {
            List<AGENDA> item = _baseRepository.GetByDate(data, idAss);
            return item;
        }

        public List<AGENDA> GetByUser(Int32 id, Int32 idAss)
        {
            List<AGENDA> item = _baseRepository.GetByUser(id, idAss);
            return item;
        }

        public AGENDA GetItemById(Int32 id)
        {
            AGENDA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<AGENDA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<AGENDA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<AGENDA> ExecuteFilter(DateTime? data, Int32? cat, String titulo, String descricao, Int32 idAss, Int32 idUser, Int32 corp)
        {
            List<AGENDA> lista = _baseRepository.ExecuteFilter(data, cat, titulo, descricao, idAss, idUser, corp);
            return lista;
        }

        public AGENDA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<CATEGORIA_AGENDA> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public async Task<IEnumerable<CATEGORIA_AGENDA>> GetAllItensAsync(Int32 idAss)
        {
            return await _tipoRepository.GetAllItensAsync(idAss);
        }

        public Int32 Create(AGENDA item, LOG log)
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

        public Int32 Create(AGENDA item)
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


        public Int32 Edit(AGENDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.CATEGORIA_AGENDA = null;
                    item.USUARIO = null;
                    item.USUARIO1 = null;

                    Int32? cat = item.CAAG_CD_ID;
                    Int32? usu = item.USUA_CD_ID;
                    Int32? usu1 = item.AGEN_CD_USUARIO;

                    AGENDA obj = _baseRepository.GetItemById(item.AGEN_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    item.CAAG_CD_ID = cat;
                    item.USUA_CD_ID = usu1.Value;
                    item.AGEN_CD_USUARIO = usu.Value;
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

        public Int32 Edit(AGENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AGENDA obj = _baseRepository.GetById(item.AGEN_CD_ID);
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

        public Int32 Delete(AGENDA item, LOG log)
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

        public Int32 EditAnexo(AGENDA_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AGENDA_ANEXO obj = _anexoRepository.GetById(item.AGAN_CD_ID);
                    _anexoRepository.Detach(obj);
                    _anexoRepository.Update(item);
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

        public Int32 CreateContato(AGENDA_CONTATO item)
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

        public AGENDA_CONTATO GetContatoById(Int32 id)
        {
            return _contRepository.GetItemById(id);
        }

        public Int32 EditContato(AGENDA_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AGENDA_CONTATO obj = _contRepository.GetById(item.AGCO_CD_ID);
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
