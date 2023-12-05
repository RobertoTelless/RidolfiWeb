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
using System.Web;
using System.Net;
using Azure.Communication.Email;

namespace ApplicationServices.Services
{
    public class UsuarioAppService : AppServiceBase<USUARIO>, IUsuarioAppService
    {
        private readonly IUsuarioService _usuarioService;
        private readonly INotificacaoService _notiService;
        private readonly IAgendaService _agService;
        private readonly IClienteAppService _cliService;
        private readonly ICRMAppService _crmService;
        private readonly ITarefaService _tarService;
        private readonly ILogService _logService;

        public UsuarioAppService(IUsuarioService usuarioService, INotificacaoService notiService, IAgendaService agService, ITarefaService tarService, IClienteAppService cliService, ICRMAppService crmService, ILogService logService) : base(usuarioService)
        {
            _usuarioService = usuarioService;
            _notiService = notiService;
            _agService = agService;
            _tarService = tarService;
            _cliService = cliService;
            _crmService = crmService;
            _logService = logService;
        }

        public USUARIO GetByEmail(String email, Int32 idAss)
        {
            return _usuarioService.GetByEmail(email, idAss);
        }

        public USUARIO GetByLogin(String login, Int32 idAss)
        {
            return _usuarioService.GetByLogin(login, idAss);
        }

        public List<USUARIO> GetAllUsuariosAdm(Int32 idAss)
        {
            return _usuarioService.GetAllUsuariosAdm(idAss);
        }

        public USUARIO GetItemById(Int32 id)
        {
            return _usuarioService.GetItemById(id);
        }

        public USUARIO CheckExist(USUARIO usuario, Int32 idAss)
        {
            return _usuarioService.CheckExist(usuario, idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuarioService.GetAllUsuarios(idAss);
        }

        public List<USUARIO> GetAllItens(Int32 idAss)
        {
            return _usuarioService.GetAllItens(idAss);
        }

        public List<CARGO> GetAllCargos(Int32 idAss)
        {
            return _usuarioService.GetAllCargos(idAss);
        }

        public USUARIO GetAdministrador(Int32 idAss)
        {
            return _usuarioService.GetAdministrador(idAss);
        }

        public List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss)
        {
            return _usuarioService.GetAllItensUser(id, idAss);
        }

        public List<NOTICIA> GetAllNoticias(Int32 idAss)
        {
            return _usuarioService.GetAllNoticias(idAss);
        }

        public List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss)
        {
            return _usuarioService.GetNotificacaoNovas(id, idAss);
        }

        public List<USUARIO> GetAllItensBloqueados(Int32 idAss)
        {
            return _usuarioService.GetAllItensBloqueados(idAss);
        }

        public List<USUARIO> GetAllItensAcessoHoje(Int32 idAss)
        {
            return _usuarioService.GetAllItensAcessoHoje(idAss);
        }

        public USUARIO_ANEXO GetAnexoById(Int32 id)
        {
            return _usuarioService.GetAnexoById(id);
        }

        public USUARIO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _usuarioService.GetAnotacaoById(id);
        }

        public Int32 ValidateCreate(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Verifica senhas iguais
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuarioLogado.ASSI_CD_ID);
                if (usuario.USUA_NM_SENHA != usuario.USUA_NM_SENHA_CONFIRMA)
                {
                    return 1;
                }

                // Verifica força da senha
                String senha = usuario.USUA_NM_SENHA;
                if (senha.Length < conf.CONF_NR_TAMANHO_SENHA)
                {
                    return 9;
                }
                if (!senha.Any(char.IsUpper) || !senha.Any(char.IsLower) && !senha.Any(char.IsDigit))
                {
                    return 10;
                }
                if (!senha.Any(p => ! char.IsLetterOrDigit(p)))
                {
                    return 11;
                }
                if (senha.Contains(usuario.USUA_NM_LOGIN) || senha.Contains(usuario.USUA_NM_NOME) || senha.Contains(usuario.USUA_NM_EMAIL))
                {
                    return 12;
                }
                if (usuario.USUA_NM_SENHA == usuarioLogado.USUA_NM_SENHA)
                {
                    return 13;
                }

                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 2;
                }

                // Verifica existencia prévia
                if (_usuarioService.GetByLogin(usuario.USUA_NM_LOGIN, usuarioLogado.ASSI_CD_ID) != null)
                {
                    return 4;
                }

                // Verifica existencia CPF
                if (_usuarioService.CheckExist(usuario, usuarioLogado.ASSI_CD_ID) != null)
                {
                    return 6;
                }

                //Completa campos de usuários
                // Grava senha
                byte[] salt = CrossCutting.Cryptography.GenerateSalt();
                String hashedPassword = CrossCutting.Cryptography.HashPassword(usuario.USUA_NM_SENHA, salt);
                usuario.USUA_NM_SENHA = hashedPassword;
                usuario.USUA_NM_SALT = salt;
                usuario.USUA_IN_BLOQUEADO = 0;
                usuario.USUA_IN_PROVISORIO = 0;
                usuario.USUA_IN_LOGIN_PROVISORIO = 0;
                usuario.USUA_NR_ACESSOS = 0;
                usuario.USUA_NR_FALHAS = 0;
                usuario.USUA_DT_ALTERACAO = null;
                usuario.USUA_DT_BLOQUEADO = null;
                usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
                usuario.USUA_DT_ACESSO = DateTime.Now;
                usuario.USUA_DT_CADASTRO = DateTime.Today.Date;
                usuario.USUA_IN_ATIVO = 1;
                usuario.CAUS_CD_ID = 1;
                usuario.USUA_DT_ULTIMA_FALHA = DateTime.Now;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddUSUA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<USUARIO>(usuario),
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 1

                };


                // Persiste
                Int32 volta = _usuarioService.CreateUser(usuario, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAssinante(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Verifica senhas
                if (usuario.USUA_NM_SENHA != usuario.USUA_NM_SENHA_CONFIRMA)
                {
                    return 1;
                }

                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 2;
                }

                // Verifica existencia prévia
                if (_usuarioService.GetByLogin(usuario.USUA_NM_LOGIN, usuarioLogado.ASSI_CD_ID) != null)
                {
                    return 4;
                }

                //Completa campos de usuários
                //usuario.USUA_NM_SENHA = Cryptography.Encode(usuario.USUA_NM_SENHA);
                usuario.USUA_IN_BLOQUEADO = 0;
                usuario.USUA_IN_PROVISORIO = 0;
                usuario.USUA_IN_LOGIN_PROVISORIO = 0;
                usuario.USUA_NR_ACESSOS = 0;
                usuario.USUA_NR_FALHAS = 0;
                usuario.USUA_DT_ALTERACAO = null;
                usuario.USUA_DT_BLOQUEADO = null;
                usuario.USUA_DT_TROCA_SENHA = null;
                usuario.USUA_DT_ACESSO = DateTime.Now;
                usuario.USUA_DT_CADASTRO = DateTime.Today.Date;
                usuario.USUA_IN_ATIVO = 1;
                usuario.CAUS_CD_ID = 1;
                usuario.USUA_DT_ULTIMA_FALHA = DateTime.Now;
                usuario.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddUSUA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<USUARIO>(usuario),
                    LOG_IN_ATIVO = 1
                };


                // Persiste
                Int32 volta = _usuarioService.CreateUser(usuario, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioAntes, USUARIO usuarioLogado)
        {
            try
            {
                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 1;
                }

                USUARIO usu = _usuarioService.GetByLogin(usuario.USUA_NM_LOGIN, usuarioLogado.ASSI_CD_ID);
                if (usu != null)
                {
                    if (usu.USUA_CD_ID != usuario.USUA_CD_ID)
                    {
                        return 3;
                    }
                }

                //Acerta campos de usuários
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 1;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EdtUSUA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<USUARIO>(usuario),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<USUARIO>(usuarioAntes),
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 1

                };


                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 1;
                }

                USUARIO usu = _usuarioService.GetByLogin(usuario.USUA_NM_LOGIN, usuarioLogado.ASSI_CD_ID);
                if (usu != null)
                {
                    if (usu.USUA_CD_ID != usuario.USUA_CD_ID)
                    {
                        return 3;
                    }
                }

                // Verifica existencia prévia

                //Acerta campos de usuários
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 1;
                usuario.CAUS_CD_ID = 1;

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateDelete(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Verifica integridade
                if (usuario.NOTICIA_COMENTARIO.Count > 0)
                {
                    return 1;
                }
                if (usuario.NOTIFICACAO.Count > 0)
                {
                    return 1;
                }
                if (usuario.TAREFA.Count > 0)
                {
                    return 1;
                }
                if (usuario.TAREFA_ACOMPANHAMENTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos de usuários
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 0;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "DelUSUA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<USUARIO>(usuario),
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Verifica integridade

                // Acerta campos de usuários
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 1;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "ReaUSUA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<USUARIO>(usuario),
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateBloqueio(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                //Acerta campos de usuários
                usuario.USUA_DT_BLOQUEADO = DateTime.Today;
                usuario.USUA_IN_BLOQUEADO = 1;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "BlqUSUA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<USUARIO>(usuario),
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDesbloqueio(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                //Acerta campos de usuários
                usuario.USUA_DT_BLOQUEADO = DateTime.Now;
                usuario.USUA_IN_BLOQUEADO = 0;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "DbqUSUA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<USUARIO>(usuario),
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateLogin(String login, String senha, out USUARIO usuario)
        {
            try
            {
                usuario = new USUARIO();
                Int32 idade = 0;

                // Checa preenchimento do login
                if (String.IsNullOrEmpty(login))
                {
                    return 10;
                }

                // Checa senha
                if (String.IsNullOrEmpty(senha))
                {
                    return 9;
                }

                // Checa login
                usuario = _usuarioService.GetByLogin(login, 1);
                if (usuario == null)
                {
                    usuario = new USUARIO();
                    return 2;
                }

                // Verifica se está ativo
                if (usuario.USUA_IN_ATIVO != 1)
                {
                    return 3;
                }

                // Verifica se está bloqueado
                if (usuario.USUA_IN_BLOQUEADO == 1)
                {
                    return 4;
                }

                // Verifica idade da senha
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                TimeSpan diff = DateTime.Today.Date - usuario.USUA_DT_TROCA_SENHA.Value;
                if (diff.TotalDays > conf.CONF_NR_VALIDADE_SENHA)
                {
                    idade = 1;
                }

                // verifica senha proviória
                if (usuario.USUA_IN_PROVISORIO == 1)
                {
                    if (usuario.USUA_IN_LOGIN_PROVISORIO == 0)
                    {
                        usuario.USUA_IN_LOGIN_PROVISORIO = 1;
                    }
                    else
                    {
                        return 5;
                    }
                }

                // Verifica credenciais
                Boolean retorno = _usuarioService.VerificarCredenciais(senha, usuario);
                if (!retorno)
                {
                    if (usuario.USUA_NR_FALHAS <= _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID).CONF_NR_FALHAS_DIA)
                    {
                        if (usuario.USUA_DT_ULTIMA_FALHA != null)
                        {
                            if (usuario.USUA_DT_ULTIMA_FALHA.Value.Date != DateTime.Now.Date)
                            {
                                usuario.USUA_DT_ULTIMA_FALHA = DateTime.Now.Date;
                                usuario.USUA_NR_FALHAS = 1;
                            }
                            else
                            {
                                usuario.USUA_NR_FALHAS++;
                            }
                        }
                        else
                        {
                            usuario.USUA_DT_ULTIMA_FALHA = DateTime.Today.Date;
                            usuario.USUA_NR_FALHAS = 1;
                        }

                    }
                    else if (usuario.USUA_NR_FALHAS > _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID).CONF_NR_FALHAS_DIA)
                    {
                        usuario.USUA_DT_BLOQUEADO = DateTime.Today.Date;
                        usuario.USUA_IN_BLOQUEADO = 1;
                        usuario.USUA_NR_FALHAS = 0;
                        usuario.USUA_DT_ULTIMA_FALHA = DateTime.Today.Date;
                        Int32 voltaBloqueio = _usuarioService.EditUser(usuario);
                        return 6;
                    }
                    Int32 volta = _usuarioService.EditUser(usuario);
                    return 7;
                }

                // Checa se está pendente de validação
                if (usuario.USUA_IN_PENDENTE_CODIGO == 1)
                {
                    return 30;
                }

                // Atualiza acessos e data do acesso
                usuario.USUA_NR_ACESSOS = ++usuario.USUA_NR_ACESSOS;
                usuario.USUA_DT_ACESSO = DateTime.Now;
                if (idade == 1)
                {
                    usuario.USUA_IN_LOGIN_PROVISORIO = 1;
                    usuario.USUA_IN_PROVISORIO = 1;
                }
                Int32 voltaAcesso = _usuarioService.EditUser(usuario);
                if (idade == 1)
                {
                    return 22;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AccSIST",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = _logService.Create(log);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateChangePassword(USUARIO usuario)
        {
            try
            {
                // Sanitização
                usuario.USUA_NM_NOVA_SENHA = CrossCutting.UtilitariosGeral.CleanStringSenha(usuario.USUA_NM_NOVA_SENHA);
                usuario.USUA_NM_SENHA_CONFIRMA = CrossCutting.UtilitariosGeral.CleanStringSenha(usuario.USUA_NM_SENHA_CONFIRMA);

                // Checa preenchimento
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                if (String.IsNullOrEmpty(usuario.USUA_NM_NOVA_SENHA))
                {
                    return 3;
                }
                if (String.IsNullOrEmpty(usuario.USUA_NM_SENHA_CONFIRMA))
                {
                    return 4;
                }

                // Verifica se senha igual a anterior
                if (usuario.USUA_NM_SENHA == usuario.USUA_NM_NOVA_SENHA)
                {
                    return 1;
                }

                // Verifica se senha foi confirmada
                if (usuario.USUA_NM_NOVA_SENHA != usuario.USUA_NM_SENHA_CONFIRMA)
                {
                    return 2;
                }

                // Verifica força da senha
                String senha = usuario.USUA_NM_NOVA_SENHA;
                if (senha.Length < conf.CONF_NR_TAMANHO_SENHA)
                {
                    return 9;
                }
                if (!senha.Any(char.IsUpper) || !senha.Any(char.IsLower) && !senha.Any(char.IsDigit))
                {
                    return 10;
                }
                if (!senha.Any(char.IsLetterOrDigit))
                {
                    return 11;
                }
                if (senha.Contains(usuario.USUA_NM_LOGIN) || senha.Contains(usuario.USUA_NM_NOME) || senha.Contains(usuario.USUA_NM_EMAIL))
                {
                    return 12;
                }

                // Gera codigo
                String codigo = Cryptography.GenerateRandomPasswordNumero(6);

                // Grava senha
                byte[] salt = CrossCutting.Cryptography.GenerateSalt();
                String hashedPassword = CrossCutting.Cryptography.HashPassword(usuario.USUA_NM_NOVA_SENHA, salt);
                usuario.USUA_NM_SENHA = hashedPassword;
                usuario.USUA_NM_SALT = salt;
                //usuario.USUA_NM_SENHA = Cryptography.Encrypt(usuario.USUA_NM_NOVA_SENHA);
                usuario.USUA_IN_PENDENTE_CODIGO = 1;
                usuario.USUA_NM_NOVA_SENHA = null;
                usuario.USUA_NM_SENHA_CONFIRMA = null;
                usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
                usuario.USUA_SG_CODIGO = codigo;
                usuario.USUA_DT_CODIGO = DateTime.Now;
                Int32 volta = _usuarioService.EditUser(usuario);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "CodPWDS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = _logService.Create(log);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateChangePasswordFinal(USUARIO usuario)
        {
            try
            {
                // Checa preenchimento
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);

                //Completa e acerta campos 
                usuario.USUA_DT_TROCA_SENHA = DateTime.Now.Date;
                usuario.USUA_IN_BLOQUEADO = 0;
                usuario.USUA_IN_PROVISORIO = 0;
                usuario.USUA_IN_LOGIN_PROVISORIO = 0;
                usuario.USUA_NR_ACESSOS = 0;
                usuario.USUA_NR_FALHAS = 0;
                usuario.USUA_DT_ALTERACAO = DateTime.Now.Date;
                usuario.USUA_DT_BLOQUEADO = null;
                usuario.USUA_DT_ACESSO = DateTime.Now;
                usuario.USUA_DT_CADASTRO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 1;
                usuario.USUA_DT_ULTIMA_FALHA = null;
                usuario.USUA_IN_PENDENTE_CODIGO = 0;
                usuario.USUA_NM_NOVA_SENHA = null;
                usuario.USUA_NM_SENHA_CONFIRMA = null;
                usuario.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Alteração de Senha";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A sua senha foi alterada em " + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = usuario.USUA_CD_ID;
                Int32 volta1 = _notiService.Create(noti);


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "ChgPWDS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = _logService.Create(log);

                // Persiste
                return _usuarioService.EditUser(usuario);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Int32> GenerateNewPassword(String email)
        {
            // Checa email
            if (!ValidarItensDiversos.IsValidEmail(email))
            {
                return 1;
            }
            USUARIO usuario = _usuarioService.RetriveUserByEmail(email);
            if (usuario == null)
            {
                return 2;
            }

            // Verifica se usuário está ativo
            if (usuario.USUA_IN_ATIVO == 0)
            {
                return 3;
            }

            // Verifica se usuário não está bloqueado
            if (usuario.USUA_IN_BLOQUEADO == 1)
            {
                return 4;
            }

            // Gera nova senha
            String senha = Cryptography.GenerateRandomPassword(6);
            byte[] salt = CrossCutting.Cryptography.GenerateSalt();
            String hashedPassword = CrossCutting.Cryptography.HashPassword(senha, salt);
            usuario.USUA_NM_SENHA = hashedPassword;
            usuario.USUA_NM_SALT = salt;

            // Atauliza objeto
            usuario.USUA_IN_PROVISORIO = 1;
            usuario.USUA_DT_ALTERACAO = DateTime.Now;
            usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
            usuario.USUA_IN_LOGIN_PROVISORIO = 0;

            // Monta log
            LOG log = new LOG();
            log.LOG_DT_DATA = DateTime.Now;
            log.LOG_NM_OPERACAO = "NewPWD";
            log.ASSI_CD_ID = usuario.ASSI_CD_ID;
            log.LOG_TX_REGISTRO = "Geração de nova senha";
            log.LOG_IN_ATIVO = 1;
            log.LOG_IN_SISTEMA = 1;

            // Atualiza usuario
            Int32 volta = _usuarioService.EditUser(usuario);

            // Gera Notificação
            NOTIFICACAO noti = new NOTIFICACAO();
            noti.CANO_CD_ID = 1;
            noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
            noti.NOTI_DT_EMISSAO = DateTime.Today;
            noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            noti.NOTI_IN_VISTA = 0;
            noti.NOTI_NM_TITULO = "Geração de Nova Senha";
            noti.NOTI_IN_ATIVO = 1;
            noti.NOTI_TX_TEXTO = "ATENÇÃO: Sua solicitação de nova senha foi atendida em " + DateTime.Today.Date.ToLongDateString() + ". Verifique no seu e-mail cadastrado no sistema.";
            noti.USUA_CD_ID = usuario.USUA_CD_ID;
            Int32 volta1 = _notiService.Create(noti);

            // Recupera template e-mail
            String header = _usuarioService.GetTemplate("NEWPWD").TEMP_TX_CABECALHO;
            String body = _usuarioService.GetTemplate("NEWPWD").TEMP_TX_CORPO;
            String data = _usuarioService.GetTemplate("NEWPWD").TEMP_TX_DADOS;

            // Prepara dados do e-mail  
            header = header.Replace("{Nome}", usuario.USUA_NM_NOME);
            data = data.Replace("{Data}", usuario.USUA_DT_TROCA_SENHA.Value.ToLongDateString());
            data = data.Replace("{Senha}", senha);

            // Concatena
            String emailBody = header + body + data;

            // Prepara e-mail e enviar
            CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Geração de Nova Senha";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = "SysPrec";
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
            String status = "Succeeded";
            String iD = "xyz";

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Monta Log
            log = new LOG
            {
                LOG_DT_DATA = DateTime.Now,
                ASSI_CD_ID = usuario.ASSI_CD_ID,
                USUA_CD_ID = usuario.USUA_CD_ID,
                LOG_NM_OPERACAO = "SolPWDS",
                LOG_IN_ATIVO = 1,
                LOG_TX_REGISTRO = null,
                LOG_IN_SISTEMA = 1
            };
            Int32 volta2 = _logService.Create(log);
            return 0;
        }

        public Tuple<Int32, List<USUARIO>, Boolean> ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email, Int32 idAss)
        {
            try
            {
                List<USUARIO> objeto = new List<USUARIO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _usuarioService.ExecuteFilter(perfilId, cargoId, nome, login, email, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<LOG_EXCECAO_NOVO>, Boolean> ExecuteFilterExcecao(Int32? usuaId, DateTime? data, String gerador, Int32 idAss)
        {
            try
            {
                List<LOG_EXCECAO_NOVO> objeto = new List<LOG_EXCECAO_NOVO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _usuarioService.ExecuteFilterExcecao(usuaId, data, gerador, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PERFIL> GetAllPerfis()
        {
            List<PERFIL> lista = _usuarioService.GetAllPerfis();
            return lista;
        }

        public LOG_EXCECAO_NOVO GetLogExcecaoById(Int32 id)
        {
            return _usuarioService.GetLogExcecaoById(id);
        }

        public List<LOG_EXCECAO_NOVO> GetAllLogExcecao(Int32 idAss)
        {
            return _usuarioService.GetAllLogExcecao(idAss);
        }

        public Int32 ValidateCreateLogExcecao(LOG_EXCECAO_NOVO log)
        {
            // Persiste
            Int32 volta = _usuarioService.CreateLogExcecao(log);
            return volta;
        }

        public List<VOLTA_PESQUISA> PesquisarTudo(String parm, Int32 idUsu, Int32 idAss)
        {
            // Critica
            if (parm == null)
            {
                return null;
            }

            // Busca em Produtos
            List<CLIENTE> listaClientes= new List<CLIENTE>();
            listaClientes = _cliService.GetAllItens(idAss).Where(p => p.CLIE_NM_NOME.Contains(parm) || (p.CLIE_NM_RAZAO ?? "").Contains(parm) || (p.CLIE_NR_CPF ?? "").Contains(parm)|| (p.CLIE_NR_CNPJ ?? "").Contains(parm) || (p.CLIE_NR_TELEFONE ?? "").Contains(parm)  || (p.CLIE_NR_CELULAR ?? "").Contains(parm)   || (p.CLIE_NM_EMAIL ?? "").Contains(parm)).ToList();

            // Busca em Processos
            List<CRM> listaCRM = new List<CRM>();
            listaCRM = _crmService.GetAllItens(idAss).Where(p => p.CRM1_NM_NOME.Contains(parm) || (p.CLIENTE.CLIE_NM_NOME ?? "").Contains(parm) || (p.CLIENTE.CLIE_NM_RAZAO ?? "").Contains(parm) || (p.CRM1_DS_DESCRICAO ?? "").Contains(parm)).ToList();

            // Busca em Acoes
            List<CRM_ACAO> listaAcoes = new List<CRM_ACAO>();
            listaAcoes = _crmService.GetAllAcoes(idAss).Where(p => p.CRAC_NM_TITULO.Contains(parm) || (p.CRAC_DS_DESCRICAO?? "").Contains(parm)).ToList();

            // Busca em Propostas
            List<CRM_PEDIDO_VENDA> listaPropostas = new List<CRM_PEDIDO_VENDA>();
            listaPropostas = _crmService.GetAllPedidos(idAss).Where(p => p.CRPV_NM_NOME.Contains(parm) || (p.CLIENTE.CLIE_NM_NOME ?? "").Contains(parm)  || (p.CLIENTE.CLIE_NM_RAZAO ?? "").Contains(parm)).ToList();

            // Busca em Agenda
            List<AGENDA> listaAgenda = new List<AGENDA>();
            listaAgenda = _agService.GetByUser(idUsu, idAss).Where(p => p.AGEN_NM_TITULO.Contains(parm) || (p.AGEN_DS_DESCRICAO ?? "").Contains(parm)).ToList();

            // Busca em Tarefas
            List<TAREFA> listaTarefa= new List<TAREFA>();
            listaTarefa = _tarService.GetByUser(idUsu).Where(p => p.TARE_NM_TITULO.Contains(parm) || (p.TARE_DS_DESCRICAO ?? "").Contains(parm)).ToList();

            // Prepara lista de retorno
            List<VOLTA_PESQUISA> listaVolta = new List<VOLTA_PESQUISA>();
            if (listaClientes.Count > 0)
            {
                foreach (CLIENTE item in listaClientes)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 1;
                    volta.PEGR_CD_ITEM = item.CLIE_CD_ID;
                    volta.PEGR_NM_NOME1 = item.CLIE_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.CLIE_NR_CPF;
                    volta.PEGR_NM_NOME3 = item.CLIE_NR_CNPJ;
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaCRM.Count > 0)
            {
                foreach (CRM item in listaCRM)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 2;
                    volta.PEGR_CD_ITEM = item.CRM1_CD_ID;
                    volta.PEGR_NM_NOME1 = item.CRM1_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.CLIENTE.CLIE_NM_NOME;
                    volta.PEGR_NM_NOME3 = item.CRM1_IN_STATUS.ToString();
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaAcoes.Count > 0)
            {
                foreach (CRM_ACAO item in listaAcoes)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 3;
                    volta.PEGR_CD_ITEM = item.CRAC_CD_ID;
                    volta.PEGR_NM_NOME1 = item.CRM.CRM1_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.CRAC_NM_TITULO;
                    volta.PEGR_NM_NOME3 = item.CRAC_IN_STATUS.ToString();
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaPropostas.Count > 0)
            {
                foreach (CRM_PEDIDO_VENDA item in listaPropostas)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 4;
                    volta.PEGR_CD_ITEM = item.CRPV_CD_ID;
                    volta.PEGR_NM_NOME1 = item.CRM.CRM1_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.CRPV_NM_NOME;
                    volta.PEGR_NM_NOME3 = item.CRPV_IN_STATUS.ToString();
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaAgenda.Count > 0)
            {
                foreach (AGENDA item in listaAgenda)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 5;
                    volta.PEGR_CD_ITEM = item.AGEN_CD_ID;
                    volta.PEGR_NM_NOME1 = item.AGEN_NM_TITULO;
                    volta.PEGR_NM_NOME2 = item.AGEN_DS_DESCRICAO;
                    volta.PEGR_NM_NOME3 = item.AGEN_DT_DATA.ToShortDateString();
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaTarefa.Count > 0)
            {
                foreach (TAREFA item in listaTarefa)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 6;
                    volta.PEGR_CD_ITEM = item.TARE_CD_ID;
                    volta.PEGR_NM_NOME1 = item.TARE_NM_TITULO;
                    volta.PEGR_NM_NOME2 = item.TARE_DS_DESCRICAO;
                    volta.PEGR_NM_NOME3 = item.TARE_DT_CADASTRO.ToShortDateString();
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }
            return listaVolta;
        }

        public Int32 ValidateEditAnexo(USUARIO_ANEXO item)
        {
            try
            {
                // Persiste
                return _usuarioService.EditAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
