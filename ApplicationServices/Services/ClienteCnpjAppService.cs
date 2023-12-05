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
    public class ClienteCnpjAppService : AppServiceBase<CLIENTE_QUADRO_SOCIETARIO>, IClienteCnpjAppService
    {
        private readonly IClienteCnpjService _baseService;

        public ClienteCnpjAppService(IClienteCnpjService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public CLIENTE_QUADRO_SOCIETARIO CheckExist(CLIENTE_QUADRO_SOCIETARIO cqs)
        {
            CLIENTE_QUADRO_SOCIETARIO item = _baseService.CheckExist(cqs);
            return item;
        }

        public List<CLIENTE_QUADRO_SOCIETARIO> GetAllItens()
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<CLIENTE_QUADRO_SOCIETARIO> GetByCliente(CLIENTE cliente)
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lista = _baseService.GetByCliente(cliente);
            return lista;
        }

        public Int32 ValidateCreate(CLIENTE_QUADRO_SOCIETARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.CLQS_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}