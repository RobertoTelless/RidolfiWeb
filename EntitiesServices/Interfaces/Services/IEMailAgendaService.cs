using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IEMailAgendaService : IServiceBase<EMAIL_AGENDAMENTO>
    {
        Int32 Create(EMAIL_AGENDAMENTO item);
        Int32 Edit(EMAIL_AGENDAMENTO item);
        List<EMAIL_AGENDAMENTO> GetAllItens(Int32 idAss);
    }
}
