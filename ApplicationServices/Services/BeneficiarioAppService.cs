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
    public class BeneficiarioAppService : AppServiceBase<BENEFICIARIO>, IBeneficiarioAppService
    {
        private readonly IBeneficiarioService _baseService;
        private readonly IConfiguracaoService _confService;

        public BeneficiarioAppService(IBeneficiarioService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<BENEFICIARIO> GetAllItens()
        {
            List<BENEFICIARIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public List<BENEFICIARIO> GetAllItensAdm()
        {
            List<BENEFICIARIO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public List<SEXO> GetAllSexo()
        {
            List<SEXO> lista = _baseService.GetAllSexo();
            return lista;
        }

        public BENEFICIARIO GetItemById(Int32 id)
        {
            BENEFICIARIO item = _baseService.GetItemById(id);
            return item;
        }

        public BENEFICIARIO CheckExist(BENEFICIARIO conta)
        {
            BENEFICIARIO item = _baseService.CheckExist(conta);
            return item;
        }

        public List<ESTADO_CIVIL> GetAllEstadoCivil()
        {
            List<ESTADO_CIVIL> lista = _baseService.GetAllEstadoCivil();
            return lista;
        }

        public BENEFICIARIO CheckExist(String nome, String cpf)
        {
            BENEFICIARIO item = _baseService.CheckExist(nome, cpf);
            return item;
        }

        public List<TIPO_TELEFONE_BASE> GetAllTipoTelefone()
        {
            List<TIPO_TELEFONE_BASE> lista = _baseService.GetAllTipoTelefone();
            return lista;
        }

        public List<ESCOLARIDADE> GetAllEscolaridade()
        {
            List<ESCOLARIDADE> lista = _baseService.GetAllEscolaridade();
            return lista;
        }

        public List<PARENTESCO> GetAllParentesco()
        {
            List<PARENTESCO> lista = _baseService.GetAllParentesco();
            return lista;
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllTiposPessoa();
            return lista;
        }

        public BENEFICIARIO_ANEXO GetAnexoById(Int32 id)
        {
            BENEFICIARIO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public BENEFICIARIO_ANOTACOES GetComentarioById(Int32 id)
        {
            BENEFICIARIO_ANOTACOES lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? tipo, Int32? sexo, Int32? estado, Int32? escolaridade, Int32? parentesco, String razao, String nome, DateTime? dataNasc, String cpf, String cnpj, String parente, out List<BENEFICIARIO> objeto)
        {
            try
            {
                objeto = new List<BENEFICIARIO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, sexo, estado, escolaridade, parentesco, razao, nome, dataNasc, cpf, cnpj, parente);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(BENEFICIARIO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.BENE_IN_ATIVO = 1;


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddBENE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<BENEFICIARIO>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(BENEFICIARIO item, BENEFICIARIO itemAntes, USUARIO usuario)
        {
            try
            {
                if (itemAntes.ESCOLARIDADE != null)
                {
                    itemAntes.ESCOLARIDADE = null;
                }
                if (itemAntes.ESTADO_CIVIL != null)
                {
                    itemAntes.ESTADO_CIVIL = null;
                }
                if (itemAntes.TIPO_PESSOA != null)
                {
                    itemAntes.TIPO_PESSOA = null;
                }
                if (itemAntes.SEXO != null)
                {
                    itemAntes.SEXO = null;
                }
                if (itemAntes.PARENTESCO != null)
                {
                    itemAntes.PARENTESCO = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditBENE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<BENEFICIARIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<BENEFICIARIO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(BENEFICIARIO item, BENEFICIARIO itemAntes)
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

        public Int32 ValidateDelete(BENEFICIARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PRECATORIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.BENE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelBENE",
                    LOG_TX_REGISTRO = item.BENE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(BENEFICIARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.BENE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatBENE",
                    LOG_TX_REGISTRO = item.BENE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CONTATO GetContatoById(Int32 id)
        {
            CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public Int32 ValidateEditContato(CONTATO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(CONTATO item)
        {
            try
            {
                item.CONT_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ENDERECO GetEnderecoById(Int32 id)
        {
            ENDERECO lista = _baseService.GetEnderecoById(id);
            return lista;
        }

        public Int32 ValidateEditEndereco(ENDERECO item)
        {
            try
            {
                // Persiste
                return _baseService.EditEndereco(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateEndereco(ENDERECO item)
        {
            try
            {
                item.ENDE_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateEndereco(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
