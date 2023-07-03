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
    public class EmailAgendaService : ServiceBase<EMAIL_AGENDAMENTO>, IEMailAgendaService
    {
        private readonly IEmailAgendaRepository _baseRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public EmailAgendaService(IEmailAgendaRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public List<EMAIL_AGENDAMENTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public Int32 Create(EMAIL_AGENDAMENTO item)
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

        public Int32 Edit(EMAIL_AGENDAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMAIL_AGENDAMENTO obj = _baseRepository.GetById(item.EMAG_CD_ID);
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

    }
}
