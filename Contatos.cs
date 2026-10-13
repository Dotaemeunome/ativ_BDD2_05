using System;
using System.Collections.Generic;

namespace AgendaWinForms
{
    public class Contatos
    {
        private readonly List<Contato> agenda = new List<Contato>();
        public IReadOnlyList<Contato> Agenda { get { return agenda.AsReadOnly(); } }

        public bool adicionar(Contato c)
        {
            if (c == null || agenda.Contains(c)) return false;
            agenda.Add(c);
            return true;
        }

        public Contato pesquisar(Contato c)
        {
            if (c == null) return null;
            int posicao = agenda.IndexOf(c);
            return posicao < 0 ? null : agenda[posicao];
        }

        public bool alterar(Contato c)
        {
            if (c == null) return false;
            int posicao = agenda.IndexOf(c);
            if (posicao < 0) return false;
            agenda[posicao] = c;
            return true;
        }

        public bool alterar(string emailAtual, Contato novoContato)
        {
            Contato atual = pesquisar(new Contato(emailAtual, "", new Data()));
            if (atual == null || novoContato == null) return false;
            Contato possivelDuplicado = pesquisar(novoContato);
            if (possivelDuplicado != null && !ReferenceEquals(atual, possivelDuplicado)) return false;
            agenda[agenda.IndexOf(atual)] = novoContato;
            return true;
        }

        public bool remover(Contato c)
        {
            return c != null && agenda.Remove(c);
        }
    }
}