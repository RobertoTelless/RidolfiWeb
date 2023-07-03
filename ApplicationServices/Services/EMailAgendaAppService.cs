using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class EMailAgendaAppService : AppServiceBase<EMAIL_AGENDAMENTO>, IEMailAgendaAppService
    {
        private readonly IEMailAgendaService _baseService;

        public EMailAgendaAppService(IEMailAgendaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<EMAIL_AGENDAMENTO> GetAllItens(Int32 idAss)
        {
            List<EMAIL_AGENDAMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public Int32 ValidateCreate(EMAIL_AGENDAMENTO item)
        {
            try
            {
                // Completa objeto

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(EMAIL_AGENDAMENTO item)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
