using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEmailAgendaRepository : IRepositoryBase<EMAIL_AGENDAMENTO>
    {
        List<EMAIL_AGENDAMENTO> GetAllItens(Int32 idAss);
    }
}
