using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AgendaWinForms
{
    public class Form1 : Form
    {
        private readonly Contatos agenda = new Contatos();
        private readonly BindingList<Contato> listaContatos = new BindingList<Contato>();
        private readonly BindingSource bs = new BindingSource();
        private readonly BindingList<Telefone> telefonesDoFormulario = new BindingList<Telefone>();

        private TextBox txtNome, txtEmail, txtTipo, txtNumero, txtPesquisa;
        private DateTimePicker dtpNascimento;
        private CheckBox chkPrincipal;
        private ListBox lstTelefones;
        private DataGridView grid;
        private Button btnAdicionar, btnPesquisar, btnAlterar, btnRemover, btnListar, btnAdicionarTelefone;

        public Form1()
        {
            Text = "Agenda de Contatos";
            Width = 1050;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            MontarInterface();
            bs.DataSource = listaContatos;
            grid.DataSource = bs;
        }

        private void MontarInterface()
        {
            Label titulo = new Label { Text = "AGENDA DE CONTATOS", Font = new Font("Arial", 16, FontStyle.Bold), AutoSize = true, Location = new Point(20, 15) };
            Controls.Add(titulo);

            Controls.Add(new Label { Text = "Nome", Location = new Point(20, 65), AutoSize = true });
            txtNome = new TextBox { Location = new Point(20, 85), Width = 250 };
            Controls.Add(txtNome);

            Controls.Add(new Label { Text = "E-mail (identificador)", Location = new Point(290, 65), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(290, 85), Width = 250 };
            Controls.Add(txtEmail);

            Controls.Add(new Label { Text = "Data de nascimento", Location = new Point(560, 65), AutoSize = true });
            dtpNascimento = new DateTimePicker { Location = new Point(560, 85), Width = 170, Format = DateTimePickerFormat.Short, MaxDate = DateTime.Today };
            Controls.Add(dtpNascimento);

            GroupBox grupoTelefone = new GroupBox { Text = "Telefone", Location = new Point(20, 130), Width = 710, Height = 125 };
            grupoTelefone.Controls.Add(new Label { Text = "Tipo", Location = new Point(15, 28), AutoSize = true });
            txtTipo = new TextBox { Location = new Point(15, 50), Width = 120 };
            grupoTelefone.Controls.Add(txtTipo);
            grupoTelefone.Controls.Add(new Label { Text = "Número", Location = new Point(150, 28), AutoSize = true });
            txtNumero = new TextBox { Location = new Point(150, 50), Width = 180 };
            grupoTelefone.Controls.Add(txtNumero);
            chkPrincipal = new CheckBox { Text = "Principal", Location = new Point(345, 50), AutoSize = true };
            grupoTelefone.Controls.Add(chkPrincipal);
            btnAdicionarTelefone = new Button { Text = "Adicionar telefone", Location = new Point(465, 47), Width = 145 };
            btnAdicionarTelefone.Click += btnAdicionarTelefone_Click;
            grupoTelefone.Controls.Add(btnAdicionarTelefone);
            lstTelefones = new ListBox { Location = new Point(15, 78), Width = 430, Height = 38 };
            grupoTelefone.Controls.Add(lstTelefones);
            Controls.Add(grupoTelefone);

            btnAdicionar = Botao("Adicionar contato", 20, 275, btnAdicionar_Click);
            btnPesquisar = Botao("Pesquisar", 150, 275, btnPesquisar_Click);
            btnAlterar = Botao("Alterar", 250, 275, btnAlterar_Click);
            btnRemover = Botao("Remover", 330, 275, btnRemover_Click);
            btnListar = Botao("Listar contatos", 410, 275, btnListar_Click);

            Controls.Add(new Label { Text = "Pesquisar por e-mail:", Location = new Point(590, 280), AutoSize = true });
            txtPesquisa = new TextBox { Location = new Point(730, 275), Width = 240 };
            Controls.Add(txtPesquisa);

            grid = new DataGridView { Location = new Point(20, 325), Width = 950, Height = 290, ReadOnly = true, AutoGenerateColumns = false, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nome", DataPropertyName = "Nome", Width = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "E-mail", DataPropertyName = "Email", Width = 250 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nascimento", DataPropertyName = "DtNasc", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Telefone principal", DataPropertyName = "TelefonePrincipal", Width = 180 });
            grid.CellDoubleClick += grid_CellDoubleClick;
            Controls.Add(grid);
        }

        private Button Botao(string texto, int x, int y, EventHandler evento)
        {
            Button botao = new Button { Text = texto, Location = new Point(x, y), Width = 115, Height = 30 };
            botao.Click += evento;
            Controls.Add(botao);
            return botao;
        }

        private void btnAdicionarTelefone_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTipo.Text) || string.IsNullOrWhiteSpace(txtNumero.Text))
            {
                MessageBox.Show("Informe o tipo e o número.");
                return;
            }
            Telefone telefone = new Telefone(txtTipo.Text.Trim(), txtNumero.Text.Trim(), chkPrincipal.Checked);
            if (telefone.Principal)
                foreach (Telefone t in telefonesDoFormulario) t.Principal = false;
            else if (telefonesDoFormulario.Count == 0) telefone.Principal = true;
            telefonesDoFormulario.Add(telefone);
            AtualizarListaTelefones();
            txtTipo.Clear(); txtNumero.Clear(); chkPrincipal.Checked = false;
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {
                Contato contato = LerContatoDoFormulario();
                if (!agenda.adicionar(contato)) { MessageBox.Show("Já existe um contato com esse e-mail."); return; }
                SincronizarLista(); LimparFormulario(); MessageBox.Show("Contato adicionado.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            string email = txtPesquisa.Text.Trim();
            if (email == "") email = txtEmail.Text.Trim();
            Contato contato = agenda.pesquisar(new Contato(email, "", new Data()));
            if (contato == null) { MessageBox.Show("Contato não encontrado."); return; }
            PreencherFormulario(contato);
            MessageBox.Show(contato.ToString(), "Contato encontrado");
        }

        private void btnAlterar_Click(object sender, EventArgs e)
        {
            try
            {
                string emailAtual = txtPesquisa.Text.Trim();
                if (emailAtual == "") emailAtual = txtEmail.Text.Trim();
                Contato existente = agenda.pesquisar(new Contato(emailAtual, "", new Data()));
                if (existente == null) { MessageBox.Show("Contato não encontrado."); return; }
                if (!agenda.alterar(emailAtual, LerContatoDoFormulario())) { MessageBox.Show("O novo e-mail já pertence a outro contato."); return; }
                SincronizarLista(); LimparFormulario(); MessageBox.Show("Contato alterado.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            string email = txtPesquisa.Text.Trim();
            if (email == "") email = txtEmail.Text.Trim();
            Contato contato = agenda.pesquisar(new Contato(email, "", new Data()));
            if (contato == null) { MessageBox.Show("Contato não encontrado."); return; }
            if (MessageBox.Show("Deseja remover este contato?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { agenda.remover(contato); SincronizarLista(); LimparFormulario(); }
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            SincronizarLista();
            if (listaContatos.Count == 0) MessageBox.Show("Nenhum contato cadastrado.");
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            PreencherFormulario((Contato)bs[e.RowIndex]);
        }

        private Contato LerContatoDoFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtEmail.Text)) throw new Exception("Nome e e-mail são obrigatórios.");
            Data data = new Data(dtpNascimento.Value.Day, dtpNascimento.Value.Month, dtpNascimento.Value.Year);
            Contato contato = new Contato(txtEmail.Text.Trim(), txtNome.Text.Trim(), data);
            foreach (Telefone telefone in telefonesDoFormulario) contato.adicionarTelefone(new Telefone(telefone.Tipo, telefone.Numero, telefone.Principal));
            return contato;
        }

        private void PreencherFormulario(Contato contato)
        {
            txtNome.Text = contato.Nome; txtEmail.Text = contato.Email; txtPesquisa.Text = contato.Email;
            dtpNascimento.Value = contato.DtNasc.ParaDateTime();
            telefonesDoFormulario.Clear();
            foreach (Telefone t in contato.Telefones) telefonesDoFormulario.Add(new Telefone(t.Tipo, t.Numero, t.Principal));
            AtualizarListaTelefones();
        }

        private void LimparFormulario()
        {
            txtNome.Clear(); txtEmail.Clear(); txtPesquisa.Clear(); telefonesDoFormulario.Clear(); AtualizarListaTelefones();
        }

        private void AtualizarListaTelefones()
        {
            lstTelefones.DataSource = null; lstTelefones.DataSource = telefonesDoFormulario;
        }

        private void SincronizarLista()
        {
            listaContatos.RaiseListChangedEvents = false; listaContatos.Clear();
            foreach (Contato contato in agenda.Agenda) listaContatos.Add(contato);
            listaContatos.RaiseListChangedEvents = true; bs.ResetBindings(false);
        }
    }
}