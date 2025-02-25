using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
//ProcCores = Numero de processadores lógicos no sistema



namespace FixAudioDG
{
    public partial class Form1: Form
    {
        //Numero de cores no pc em dec
        public int ProcCores = Environment.ProcessorCount;
        //Numero de cores no pc em bin
        public string TotalCoresBin;
        //Core selecionado
        public string StringDecSelectedCore;
        //core selecionado
        public int SelectedCore;
        //zeros a direita do core selecionado
        public string SelectedCoresPadBin;
        //bit a ser alterado
        public string StringBinSelectedCore;
        //prioridade selecionada
        public string SelectedPriority;
        //código da prioridade selecionada
        public string PrioriCode;
        //Valida config Salva
        public int validconf;

        public Form1()
        {
            InitializeComponent();

            label5.Text = "FixAudioDG GUI v.1.0 by Simian";

            //label1.Text = cores.ToString();
            for (int i = 0; i < ProcCores; i++)
            {
                comboBox1.Items.Add(i);
            }
            comboBox1.SelectedIndex = (int)Math.Round((double)(ProcCores / 4 * 3) +1);
            comboBox2.SelectedIndex = 2;
            label6.Visible = true;
            label7.Visible = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label6.Visible = false;
            if (comboBox1.SelectedIndex < (int)Math.Round((double)(ProcCores / 4)))
            {
                MessageBox.Show("Evite usar núcleos iniciais, estes tendem a ser mais sobrecarregados", "Recomendação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            testeconf();
            if (validconf == 2)
            {
                MessageBox.Show("Configuração salva inválida\nSua configuração salva foi redefinida, configure-a novamente", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (validconf == 1)
            {
                //nada a fazer
            } else if (validconf == 0)
            {
                button2.Enabled = true;
            }
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label2.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void testeconf()
        {
            try
            {
                RegistryKey GetReg = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SimianSoftware\AudioDG");

                string SelCore = GetReg.GetValue("Afinidade").ToString();
                if (int.Parse(SelCore) < 0 || int.Parse(SelCore) > ProcCores)
                {
                    validconf = 2;
                    GetReg.DeleteValue("Afinidade");
                    GetReg.DeleteValue("Prioridade");
                    return;
                }
                string SelPrioridade = GetReg.GetValue("Prioridade").ToString();
                if (int.Parse(SelPrioridade) < 0 || int.Parse(SelPrioridade) > 5)
                    {
                    validconf = 2;
                    GetReg.DeleteValue("Afinidade");
                    GetReg.DeleteValue("Prioridade");
                    return;
                }

                string SelPrio = (GetReg.GetValue("Prioridade").ToString());

                validconf = 0;

            }
            catch
            {
                //MessageBox.Show("Registro não existe");
                validconf = 1;
            }
        }

        private void Afinidade()
        {
            //Um zero para cada core do processador do usuario
            TotalCoresBin = "".PadLeft(ProcCores, '0') + ".";
            //Core escolhido
            SelectedCore = comboBox1.SelectedIndex;
            //um zero a direita de cada core até o core escolhido
            SelectedCoresPadBin = "".PadLeft(SelectedCore, '0');
            //altera o zero do core escolhido para 1 e adiciona os zeros a direita
            StringBinSelectedCore = TotalCoresBin.Replace(SelectedCoresPadBin + ".", "1" + SelectedCoresPadBin).Substring(1);
            //transforma o binário acima em int decimal e por fim em string
            StringDecSelectedCore = Convert.ToInt32(StringBinSelectedCore, 2).ToString();
        }

        private void Prioridade()
        {
            /*
             index  id      Nome
             0      256      Tempo Real
             1      128      Alta
             2      32768    Acima da Normal
             3      32       Normal
             4      16384    Abaixo da Normal
             5      64       Ocioso
            */
            SelectedPriority = comboBox2.SelectedIndex.ToString();
            if (SelectedPriority == "0") { PrioriCode = "256"; }
            else if (SelectedPriority == "1") { PrioriCode = "128"; }
            else if (SelectedPriority == "2") { PrioriCode = "32768"; }
            else if (SelectedPriority == "3") { PrioriCode = "32"; }
            else if (SelectedPriority == "4") { PrioriCode = "16384"; }
            else if (SelectedPriority == "5") { PrioriCode = "64"; }
            else { PrioriCode = "32"; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Afinidade();
            Prioridade();

           /* //Descomente para debug
            MessageBox.Show(  "Cores disponiveis em dec: " + ProcCores.ToString() 
                            + "\nCores disponiveis em Bin: " + TotalCoresBin.ToString() 
                            + "\nCore Selecionado: " + SelectedCore.ToString() 
                            + "\nPadding do Core Selecionado em Bin: " + SelectedCoresPadBin
                            + "\nString Final em Bin: " + StringBinSelectedCore
                            + "\nString Final em Dec: " + StringDecSelectedCore
                            + "\n"
                            + "\nPrioridade index: " + SelectedPriority
                            + "\nPrioridade code: " + PrioriCode.ToString()
                            
            );
            */

                Go();

        }

        public void Go()
        {
            label5.Text = "Definindo afinidade";
            Process cmda = new Process();
            cmda.StartInfo.FileName = "powershell.exe";
            cmda.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmda.StartInfo.Arguments = "-Command \"$process = Get-Process audiodg ; $process.ProcessorAffinity = " + StringDecSelectedCore + "\"";
            cmda.Start();
            cmda.WaitForExit();
            int exitca = cmda.ExitCode;
            //if (exitca >= 1) { MessageBox.Show("Falhou ao definir Afinidade! \nCodigo de erro: " + exitca, "Falha", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            label5.Text = "Definindo Prioridade";
            Process cmdb = new Process();
            cmdb.StartInfo.FileName = "powershell.exe";
            cmdb.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmdb.StartInfo.Arguments = "-Command \"Get-WmiObject Win32_Process -Filter \\\"Name='audiodg.exe'\\\" | ForEach-Object { $_.SetPriority(" + PrioriCode + ") }\"";
            cmdb.Start();
            cmdb.WaitForExit();
            int exitcb = cmdb.ExitCode;
            //if (exitcb >= 1) { MessageBox.Show("Falhou ao definir Prioridade! \nCodigo de erro: " + exitca, "AudioDG - Falha", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            int exitcsum = (exitca + exitcb);
            if (exitcsum == 0) { 
                MessageBox.Show(" Prioridade e afinidade configuradas com Sucesso!", "AudioDG - Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                label5.Text = "Sucesso!";
                //Application.Exit();
            } else
            {
                string error;
                if (exitca > 0) { error = "Definir Afinidade: Erro " + exitca.ToString(); }else { error = "Definir Afinidade: Sucesso."; }
                if (exitcb > 0) { error = error + "\nDefinir Prioridade: Erro " + exitcb.ToString(); } else { error = error + "\nDefinir Prioridade: Sucesso."; }
                MessageBox.Show(error, "Falha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label5.Text = "Falhou!";
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegistryKey GetReg = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SimianSoftware\AudioDG");

            string SelCore = GetReg.GetValue("Afinidade").ToString();
            comboBox1.SelectedIndex = int.Parse(SelCore);

            string SelPrio = (GetReg.GetValue("Prioridade").ToString());
            comboBox2.SelectedIndex = int.Parse(SelPrio);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Afinidade();
            Prioridade();
            RegistryKey newreg = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SimianSoftware\AudioDG");
            newreg.SetValue("Prioridade", SelectedPriority, RegistryValueKind.String);
            newreg.SetValue("Afinidade", SelectedCore, RegistryValueKind.String);
            testeconf();
            if (validconf == 0)
            {
                button2.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label7.Visible = false;
            if (comboBox2.SelectedIndex == 3)
            {
                MessageBox.Show("Esta configuração não irá surtir efeito na performance do AudioDG", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (comboBox2.SelectedIndex > 3)
            {
                MessageBox.Show("Esta configuração irá piorar a performance do AudioDG", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
