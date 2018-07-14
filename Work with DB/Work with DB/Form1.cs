using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Work_with_DB
{
    public partial class Form1 : Form
    {
        SqlConnection sqlConnection;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Refrash()
        {
            listView1.Items.Clear();
            SqlDataReader sqlReader = null;

            SqlCommand command = new SqlCommand("SELECT * FROM [People]", sqlConnection);

            try
            {
                sqlReader = await command.ExecuteReaderAsync(); //зчитує таблицю

                while (await sqlReader.ReadAsync())
                {
                    ListViewItem item = new ListViewItem(new string[] 
                    {
                        Convert.ToString(sqlReader["Id"]),
                        Convert.ToString(sqlReader["Name"]),
                        Convert.ToString(sqlReader["Surname"])
                    });

                    listView1.Items.Add(item);
         
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!sqlReader.IsClosed)
                    sqlReader.Close();
            }
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            string conectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=d:\load\Work with DB\Work with DB\Database1.mdf;Integrated Security=True";//ссилка на локальну базу даних
            try
            {
                sqlConnection = new SqlConnection(conectionString);
                await sqlConnection.OpenAsync(); // відкриття в асинхронному вигляді, щоб не тормозило вікно
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            listView1.Columns.Add("ID",50); // перша змінна це назва колонки, друга - початкова ширина
            listView1.Columns.Add("Ім'я",100);
            listView1.Columns.Add("Прізвище",100);

            Refrash();// перезагружає список в SELECT з DataBase1.mdf
        }

        private void вихідToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
                Application.Exit();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) && (string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))) 
            //надана можливість задавати не всі значення(крім ID), для того щоб можна було змінити лиш один елемент без повторного запису
            {
                MessageBox.Show("Поле \"ID\" і поля \"Ім'я\" або \"Прізвище\" не можуть бути пустими!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    string changedValues = null;
                    if (!string.IsNullOrWhiteSpace(textBox2.Text))
                    {
                        changedValues = "[Name] = @Name";
                        if (!string.IsNullOrWhiteSpace(textBox3.Text))
                            changedValues += ", [Surname] = @Surname";
                    }
                     else
                        changedValues = "[Surname] = @Surname";

                    SqlCommand command = new SqlCommand("UPDATE [People] SET " + changedValues + " WHERE [Id] = @Id", sqlConnection); 
                    command.Parameters.AddWithValue("Id", textBox1.Text);
                    if (!string.IsNullOrWhiteSpace(textBox2.Text))
                        command.Parameters.AddWithValue("Name", textBox2.Text);
                    if (!string.IsNullOrWhiteSpace(textBox3.Text))
                        command.Parameters.AddWithValue("Surname", textBox3.Text);

                    int check = await command.ExecuteNonQueryAsync();

                    if (check != 0)
                    {
                        label8.Visible = true;
                        label8.ForeColor = Color.Lime;
                        label8.Text = "Заміна елементів виконана успішно!";
                    }
                    else
                    {
                        label8.Visible = true;
                        label8.ForeColor = Color.Red;
                        label8.Text = "Error: " + "Даний параметр ID не знайдено!";
                    }
                }

                catch (Exception ex)
                {
                    label8.Visible = true;
                    label8.ForeColor = Color.Red;
                    label8.Text = "Error: " + ex.Message.ToString();
                }

                finally
                {
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Полe \"ID\" не може бути пустим!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    SqlCommand command = new SqlCommand("DELETE FROM [People] WHERE [Id] = @Id", sqlConnection);
                    command.Parameters.AddWithValue("Id", textBox4.Text);
                    
                    int check = await command.ExecuteNonQueryAsync();

                    if (check != 0)
                    {
                        label9.Visible = true;
                        label9.ForeColor = Color.Lime;
                        label9.Text = "Видалення виконано успішно!";
                    }
                    else
                    {
                        label9.Visible = true;
                        label9.ForeColor = Color.Red;
                        label9.Text = "Error: " + "Даний параметр ID не знайдено!";
                    }

                }

                catch (Exception ex)
                {
                    label9.Visible = true;
                    label9.ForeColor = Color.Red;
                    label9.Text = "Error: " + ex.Message.ToString();
                }

                finally
                {
                    textBox4.Clear();
                }
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox6.Text) || string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Поля \"Ім'я\" і \"Прізвище\" не можуть бути пустими!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    SqlCommand command = new SqlCommand("INSERT INTO [People] (Name, Surname) VALUES(@Name, @Surname)", sqlConnection);
                    command.Parameters.AddWithValue("Name", textBox6.Text);
                    command.Parameters.AddWithValue("Surname", textBox5.Text);
                    await command.ExecuteNonQueryAsync();

                    label7.Visible = true;
                    label7.ForeColor = Color.Lime;
                    label7.Text = "Додавання виконано успішно!";
                }

                catch (Exception ex)
                {
                    label7.Visible = true;
                    label7.ForeColor = Color.Red;
                    label7.Text = "Error: " + ex.Message.ToString();
                }

                finally
                {
                    textBox6.Clear();
                    textBox5.Clear();
                }
            }
            
        }

        private void обновитиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refrash();// перезагружає список в SELECT
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            Refrash();// перезагружає список в SELECT
        }

        private void проПрограмуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Working with DB,\n2017", "Про програму", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
