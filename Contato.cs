using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgendaWinForms
{
    public class Contato
    {
        public string Email { get; set; }
        public string Nome { get; set; }
        public Data DtNasc { get; set; }
        public List<Telefone> Telefones { get; private set; }

        public string TelefonePrincipal
        {
            get { return getTelefonePrincipal(); }
        }

        public Contato(string email, string nome, Data dtNasc)
        {
            Email = email;
            Nome = nome;
            DtNasc = dtNasc;
            Telefones = new List<Telefone>();
        }

        public int getIdade()
        {
            DateTime nascimento = DtNasc.ParaDateTime();
            int idade = DateTime.Today.Year - nascimento.Year;
            if (nascimento.Date > DateTime.Today.AddYears(-idade).Date)
                idade--;
            return idade;
        }

        public void adicionarTelefone(Telefone t)
        {
            if (t == null) return;
            if (t.Principal)
                foreach (Telefone telefone in Telefones)
                    telefone.Principal = false;
            else if (Telefones.Count == 0)
                t.Principal = true;
            Telefones.Add(t);
        }

        public string getTelefonePrincipal()
        {
            Telefone telefone = Telefones.FirstOrDefault(t => t.Principal);
            return telefone == null ? "Não informado" : telefone.Numero;
        }

        public override string ToString()
        {
            StringBuilder texto = new StringBuilder();
            texto.AppendLine("Nome: " + Nome);
            texto.AppendLine("E-mail: " + Email);
            texto.AppendLine("Nascimento: " + DtNasc);
            texto.AppendLine("Idade: " + getIdade() + " anos");
            texto.AppendLine("Telefone principal: " + getTelefonePrincipal());
            texto.Append("Telefones: ");
            texto.Append(Telefones.Count == 0 ? "Nenhum" : string.Join(", ", Telefones));
            return texto.ToString();
        }

        public override bool Equals(object obj)
        {
            Contato outro = obj as Contato;
            return outro != null && string.Equals(Email, outro.Email, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Email ?? "");
        }
    }
}
