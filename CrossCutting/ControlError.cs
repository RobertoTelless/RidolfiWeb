using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public class ControlError
    {
        public List<string> Mensagens { get; set; }
        public bool IsOK { get; set; }

        public ControlError()
        {
            Mensagens = new List<string>();
            IsOK = true;
        }

        public void HandleExeption(Exception ex)
        {
            Mensagens.Add(ex.Message);
            Mensagens.Add(ex.StackTrace);

            this.IsOK = false;
        }

        

        public void HandleExeption(string menssage)
        {
            Mensagens.Add(menssage);
        }

        public void HandleExeption(List<string> menssagens)
        {
            Mensagens.AddRange(menssagens);
        }

        public string GetMenssage()
        {
            string mensagem = "";

            foreach(string m in Mensagens)
            {
                mensagem = string.Concat(mensagem, " ", m);
            }

            return mensagem;
        }
       
    }
}
