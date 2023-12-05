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
    public class UsuarioService : ServiceBase<USUARIO>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPerfilRepository _perfRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        private readonly ITemplateRepository _tempRepository;
        private readonly IUsuarioAnexoRepository _anexoRepository;
        private readonly INotificacaoRepository _notRepository;
        private readonly INoticiaRepository _ntcRepository;
        private readonly ICargoRepository _carRepository;
        private readonly IUsuarioAnotacaoRepository _anoRepository;
        private readonly ILogExcecaoRepository _excRepository;

        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogRepository logRepository, IConfiguracaoRepository configuracaoRepository, IPerfilRepository perfRepository, ITemplateRepository tempRepository, IUsuarioAnexoRepository anexoRepository, INotificacaoRepository notRepository, INoticiaRepository ntcRepository, ICargoRepository carRepository, IUsuarioAnotacaoRepository anoRepository, ILogExcecaoRepository excRepository) : base(usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _logRepository = logRepository;
            _configuracaoRepository = configuracaoRepository;
            _perfRepository = perfRepository;
            _tempRepository = tempRepository;
            _anexoRepository = anexoRepository;
            _notRepository = notRepository;
            _ntcRepository = ntcRepository;
            _carRepository = carRepository;
            _anoRepository = anoRepository;
            _excRepository = excRepository;
        }

        public USUARIO RetriveUserByEmail(String email)
        {
            USUARIO usuario = _usuarioRepository.GetByEmailOnly(email);
            return usuario;
        }

        public TEMPLATE GetTemplate(String code)
        {
            return _tempRepository.GetByCode(code);
        }

        public Boolean VerificarCredenciais (String senha, USUARIO usuario)
        {
            // Criptografa senha informada
            //String senhaCrip = Cryptography.Encrypt(senha);
            ////string senhaCrip = senha;

            //// verifica senha
            //if (usuario.USUA_NM_SENHA.Trim() != senhaCrip.Trim())
            //{
            //    return false;
            //}
            //return true;

            Byte[] salt = usuario.USUA_NM_SALT;
            String hashedPassword = CrossCutting.Cryptography.HashPassword(senha, salt);
            if (hashedPassword != usuario.USUA_NM_SENHA)
            {
                return false;
            }
            return true;
        }

        public USUARIO GetByEmail(String email, Int32 idAss)
        {
            return _usuarioRepository.GetByEmail(email, idAss);
        }

        public USUARIO CheckExist(USUARIO usuario, Int32 idAss)
        {
            return _usuarioRepository.CheckExist(usuario, idAss);
        }

        public USUARIO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public USUARIO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public USUARIO GetByLogin(String login, Int32 idAss)
        {
            return _usuarioRepository.GetByLogin(login, idAss);
        }

        public TEMPLATE GetTemplateByCode(String codigo)
        {
            return _tempRepository.GetByCode(codigo);
        }

        public USUARIO GetItemById(Int32 id)
        {
            return _usuarioRepository.GetItemById(id);
        }

        public List<USUARIO> GetAllUsuariosAdm(Int32 idAss)
        {
            return _usuarioRepository.GetAllUsuariosAdm(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuarioRepository.GetAllUsuarios(idAss);
        }

        public List<USUARIO> GetAllItens(Int32 idAss)
        {
            return _usuarioRepository.GetAllItens(idAss);
        }

        public List<CARGO> GetAllCargos(Int32 idAss)
        {
            return _carRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllItensBloqueados(Int32 idAss)
        {
            return _usuarioRepository.GetAllItensBloqueados(idAss);
        }

        public USUARIO GetAdministrador(Int32 idAss)
        {
            return _usuarioRepository.GetAdministrador(idAss);
        }

        public List<USUARIO> GetAllItensAcessoHoje(Int32 idAss)
        {
            return _usuarioRepository.GetAllItensAcessoHoje(idAss);
        }

        public Int32 CreateUser(USUARIO usuario, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _usuarioRepository.Add(usuario);
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

        public Int32 CreateUser(USUARIO usuario)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _usuarioRepository.Add(usuario);
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

        public Int32 EditUser(USUARIO usuario, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    usuario.EMPRESA = null;
                    USUARIO obj = _usuarioRepository.GetById(usuario.USUA_CD_ID);
                    _usuarioRepository.Detach(obj);
                    _logRepository.Add(log);
                    _usuarioRepository.Update(usuario);
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

        public Int32 EditUser(USUARIO usuario)
        {
            try
            {
                USUARIO obj = _usuarioRepository.GetById(usuario.USUA_CD_ID);
                _usuarioRepository.Detach(obj);
                _usuarioRepository.Update(usuario);
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Int32 VerifyUserSubscription(USUARIO usuario)
        {
            return 0;
        }

        public Endereco GetAdressCEP(string CEP)
        {
            Endereco endereco = null;
            return endereco;
        }

        public CONFIGURACAO CarregaConfiguracao(Int32 id)
        {
            CONFIGURACAO conf = _configuracaoRepository.GetById(id);
            return conf;
        }

        public List<USUARIO> ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email, Int32 idAss)
        {
            List<USUARIO> lista = _usuarioRepository.ExecuteFilter(perfilId, cargoId, nome, login, email, idAss);
            return lista;
        }

        public List<LOG_EXCECAO_NOVO> ExecuteFilterExcecao(Int32? usuaId, DateTime? data, String gerador, Int32 idAss)
        {
            List<LOG_EXCECAO_NOVO> lista = _excRepository.ExecuteFilter(usuaId, data, gerador, idAss);
            return lista;
        }

        public List<PERFIL> GetAllPerfis()
        {
            List<PERFIL> lista = _perfRepository.GetAll().ToList();
            return lista;
        }

        public List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss)
        {
            return _notRepository.GetAllItensUser(id, idAss);
        }

        public List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss)
        {
            return _notRepository.GetNotificacaoNovas(id, idAss);
        }

        public List<NOTICIA> GetAllNoticias(Int32 idAss)
        {
            return _ntcRepository.GetAllItens(idAss);
        }

        public LOG_EXCECAO_NOVO GetLogExcecaoById(Int32 id)
        {
            return _excRepository.GetItemById(id);
        }

        public List<LOG_EXCECAO_NOVO> GetAllLogExcecao(Int32 idAss)
        {
            return _excRepository.GetAllItens(idAss);
        }

        public Int32 CreateLogExcecao(LOG_EXCECAO_NOVO log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _excRepository.Add(log);
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

        public Int32 EditAnexo(USUARIO_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    USUARIO_ANEXO obj = _anexoRepository.GetById(item.USAN_CD_ID);
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
    }
}
