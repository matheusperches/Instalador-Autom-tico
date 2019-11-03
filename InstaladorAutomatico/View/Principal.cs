﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace InstaladorAutomatico
{
    public partial class Principal : Form
    {
        //declarando lista local
        List<Model.Programa> ListaLocal = new List<Model.Programa>();
        List<int> linhasSelecionadas = new List<int>();
        Queue<String> filaDeInstalacao = new Queue<String>();
        Model.Programa ProgramaFuncoes = new Model.Programa();

        public Principal()
        {
            //inicializando componente e Lista
            InitializeComponent();
            //Image image = Image.FromFile(@"C:\TI\Ammyy\Ammyy_v3.5.exe");
            //caminhoIconeDataGridViewTextBoxColumn.Image = image;
            //caminhoIconeDataGridViewTextBoxColumn.HeaderText = "Image";
            //caminhoIconeDataGridViewTextBoxColumn.Name = "img";
        }

        private void Principal_Load(object sender, EventArgs e)
        {
            GradeDeDados.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            GradeDeDados.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            nomeProgramaDataGridViewTextBoxColumn.ReadOnly = true;
            arquiteturaProgramaDataGridViewTextBoxColumn.ReadOnly = true;
            Selecionar.ReadOnly = false;
            ProgramaFuncoes.VerificaPorXMLInicializacao();
            ObterLista();
            DefineValorCheckBox(true);
        }

        private int PercorreCheckBoxes()
        {
            Boolean valorCheckBox;
            int contaMarcacoes = 0;

            for (int i = 0; i <= GradeDeDados.RowCount - 1; i++)
            {
                valorCheckBox = Convert.ToBoolean(GradeDeDados[2, i].Value);
                if (valorCheckBox == true)
                {
                    contaMarcacoes++;
                }
            }

            return contaMarcacoes;
        }

        private void DefineValorCheckBox(Boolean valorBooleano)
        {
            for (int i = 0; i <= GradeDeDados.RowCount - 1; i++)
            {
                GradeDeDados[2, i].Value = valorBooleano;
            }
        }
        private void BtnMarcaDesmarca_Click(object sender, EventArgs e)
        {
            if (PercorreCheckBoxes() == 0)
            {
                DefineValorCheckBox(true);
            }
            else if (PercorreCheckBoxes() == GradeDeDados.RowCount)
            {
                DefineValorCheckBox(false);
            }
            else if (PercorreCheckBoxes() < GradeDeDados.RowCount)
            {
                DefineValorCheckBox(true);
            }
            /*
             * depreciado
             * foreach (Control c in tblPnlOrganiza.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox cb = (CheckBox)c;
                    if (cb.Checked == false)
                    {
                        if (executouSegundoIf == false)
                        {
                            cb.Checked = true;
                            executouPrimeiroIf = true;
                            executouSegundoIf = false;
                            BtnMarcaDesmarca.Text = "Desmarcar Tudo";
                        }
                    }
                    if (cb.Checked == true)
                    {
                        if (executouPrimeiroIf == false)
                        {
                            cb.Checked = false;
                            executouSegundoIf = true;
                            executouPrimeiroIf = false;
                            BtnMarcaDesmarca.Text = "Marcar Tudo";
                        }
                    }
                }
            }*/
        }

        private void ObterLinhasSelecionadas()
        {
            linhasSelecionadas.Clear();
            Boolean valorCheckBox;
            for (int i = 0; i <= GradeDeDados.RowCount - 1; i++)
            {
                valorCheckBox = Convert.ToBoolean(GradeDeDados[2, i].Value);
                if (valorCheckBox == true)
                {
                    GradeDeDados.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    linhasSelecionadas.Add(i);
                }
            }
        }

        private void MarcaComoPendente()
        {
            Boolean valorCheckBox;
            for (int i = 0; i <= GradeDeDados.RowCount - 1; i++)
            {
                valorCheckBox = Convert.ToBoolean(GradeDeDados[2, i].Value);
                if (valorCheckBox == true)
                {
                    GradeDeDados.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
        }

        private void GeraFilaInstalacao()
        {
            for (int i = 0; i <= linhasSelecionadas.Count- 1; i++)
            {
                filaDeInstalacao.Enqueue(ListaLocal[linhasSelecionadas[i]].diretorioPrograma);
            }
        }

        private void ExecutaFilaDeInstalacao()
        {
            String ProgramaSendoInstalado;
            ObterLinhasSelecionadas();
            GeraFilaInstalacao();
            for (int i = 0; i <= GradeDeDados.Rows.Count - 1; i++)
            {
                if (filaDeInstalacao.Count == 0)
                {
                    MessageBox.Show("Instalação concluída!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ProgramaSendoInstalado = filaDeInstalacao.Dequeue();
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(ProgramaSendoInstalado);
                System.Diagnostics.Process rfp = new System.Diagnostics.Process();
                rfp = System.Diagnostics.Process.Start(psi);
                rfp.WaitForExit(300000);
                GradeDeDados.Rows[linhasSelecionadas[i]].DefaultCellStyle.BackColor = Color.LightGreen;
            }
            linhasSelecionadas.Clear();
            
        }
         
        private void btnInstlr_Click(object sender, EventArgs e)
        {
            GradeDeDados.Enabled = false;

            if (PercorreCheckBoxes() == 0)
            {
                MessageBox.Show("Nenhum programa está selecionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GradeDeDados.ClearSelection();
            ExecutaFilaDeInstalacao();
            linhasSelecionadas.Clear();

            //chechando arquitetura do sistema
            bool ArchSys = System.Environment.Is64BitOperatingSystem;
            if (ArchSys == true)
            {
                //fazer uma filtragem de programas.
            }
            else
            {
                //fazer outra filtragem de programas.
            }
            GradeDeDados.Enabled = true;
        }
        private void CopiarArquivos()
        {
            Boolean valorCheckBox;
            System.IO.Directory.CreateDirectory(Properties.Settings.Default.DestinoCopia);
            String caminhoCopia;
            String extensaoArquivo;
            if (PercorreCheckBoxes() == 0)
            {
                MessageBox.Show("Nenhum programa está selecionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MarcaComoPendente();
            ObterLinhasSelecionadas();
            for (int i = 0; i <= GradeDeDados.RowCount - 1; i++)
            {
                valorCheckBox = Convert.ToBoolean(GradeDeDados[2, i].Value);
                if (valorCheckBox == true)
                {
                    try
                    {
                        extensaoArquivo = Path.GetExtension(ListaLocal[i].diretorioPrograma);
                        caminhoCopia = Properties.Settings.Default.DestinoCopia + "\\" + ListaLocal[i].nomePrograma;
                        System.IO.Directory.CreateDirectory(caminhoCopia);
                        File.Copy(ListaLocal[i].diretorioPrograma, caminhoCopia + "\\" + ListaLocal[i].nomePrograma + extensaoArquivo, true);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Erro ao copiar o programa {ListaLocal[i].nomePrograma}.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    GradeDeDados.Rows[linhasSelecionadas[i]].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
            MessageBox.Show($"Cópia concluída!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCopiarArquivos_Click(object sender, EventArgs e)
        {
            CopiarArquivos();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ListaDeProgramasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //abrindo o formulário para a adição de um novo programa
            View.Gerenciar_Programas novoPrograma = new View.Gerenciar_Programas();
            novoPrograma.ShowDialog();
            novoPrograma.Dispose();
        }

        private void BtnVrfcInstlc_Click(object sender, EventArgs e)
        {

        }

        public void ObterLista()
        {
            ProgramaFuncoes.VerificaSelecionarLocalSalvamentoXML();
            try
            {
                ListaLocal.Clear();
                ListaLocal.AddRange(ProgramaFuncoes.DeserializaPrograma());
                GradeDeDados.DataSource = null;
                GradeDeDados.DataSource = ListaLocal;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Arquivo não encontrado. A tabela está vazia.", "Falha no carregamento", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnAtualizarDataGrid_Click(object sender, EventArgs e)
        {
            ObterLista();
            GradeDeDados.FirstDisplayedScrollingRowIndex = GradeDeDados.RowCount - 1;
        }

        private void SairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnSair_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void VerificarWindowsUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cplPath = System.IO.Path.Combine(Environment.SystemDirectory, "control.exe");
            System.Diagnostics.Process.Start(cplPath, "/name Microsoft.WindowsUpdate");
        }
    }
}
