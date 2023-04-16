using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CinemaReservation
{
    public partial class Form1 : Form
    {
        // variabel untuk menyimpan data movie
        public string[] dbmovie = new string[500];
        public string[] dbimg = new string[500];
        // variabel untuk menyimpan data reservasi
        public bool[,,,,] reserve = new bool[500,3,10,10,2];
        // variabel untuk menghitung jumlah movie dalam movie list
        int dbmovie_count = 0;
        public string select = "";
        public Form1()
        {
            InitializeComponent();
            // membaca movie list dari file list_movie.txt
            using (StreamReader file = new StreamReader("../../../list_movie.txt"))
            {
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    Console.WriteLine(ln);
                    dbmovie[dbmovie_count] = ln;
                    int ix = ln.IndexOf("|");

                    if (ix != -1)
                    {
                        dbmovie[dbmovie_count] = ln.Substring(0,ix-1);
                        int fn_length = ln.Length - ix - 2;
                        dbimg[dbmovie_count] = ln.Substring(ix+2, fn_length);
                    }
                    dbmovie_count++;
                }
                file.Close();
            }
            // memanggil fungsi untuk inisialisasi random reservation
            random_reserve();
            // menampilkan menu movie list
            menu(true);
        }

        // fungsi untuk menampilkan movie_list & sekaligus clear all control dalam window (untuk ganti form)
        public void menu(bool show)
        {
            if (show)
            {
                Label title = new Label();
                title.Text = "Choose Movie and Playing Time";
                title.Location = new Point(5, 5);
                title.Width = 600;
                title.Height = 30;
                title.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.Controls.Add(title);

                Panel pan = new Panel();
                pan.Location = new Point(10, 30);
                pan.Width = 500;
                pan.Height = 500;
                pan.BackColor = System.Drawing.Color.White;
                pan.AutoScroll = false;
                pan.HorizontalScroll.Enabled = false;
                pan.HorizontalScroll.Visible = false;
                pan.HorizontalScroll.Maximum = 0;
                pan.AutoScroll = true;
                this.Controls.Add(pan);


                Button[,] pt = new Button[500, 3];
                for (int i = 0; i < dbmovie_count; i++)
                {
                    PictureBox pics = new PictureBox();
                    pics.Location = new Point(10, i * 180 + 10);
                    pics.Name = dbimg[i];
                    pics.Size = new Size(90, 160);
                    pics.ImageLocation = "../../../image/" + dbimg[i];
                    pan.Controls.Add(pics);

                    Label title_movie = new Label();
                    title_movie.Text = dbmovie[i];
                    title_movie.Location = new Point(120, i * 180 + 70);
                    title_movie.Width = 150;
                    title_movie.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    pan.Controls.Add(title_movie);

                    pt[i, 0] = new Button();
                    pt[i, 0].Location = new Point(300, i * 180 + 20);
                    pt[i, 0].Width = 100;
                    pt[i, 0].Height = 30;
                    pt[i, 0].BackColor = Color.Red;
                    pt[i, 0].ForeColor = Color.White;
                    pt[i, 0].Name = dbmovie[i] + "|1";
                    pt[i, 0].Text = "11:00 WIB";
                    pt[i, 0].Click += (s, e) => {
                        Button btn = (Button)s;
                        if (btn != null)
                        {
                            select = (btn.Name);
                            seat(select);
                        }
                    };
                    pan.Controls.Add(pt[i, 0]);

                    pt[i, 1] = new Button();
                    pt[i, 1].Location = new Point(300, i * 180 + 70);
                    pt[i, 1].Width = 100;
                    pt[i, 1].Height = 30;
                    pt[i, 1].BackColor = Color.Red;
                    pt[i, 1].ForeColor = Color.White;
                    pt[i, 1].Name = dbmovie[i] + "|2";
                    pt[i, 1].Text = "15:00 WIB";
                    pt[i, 1].Click += (s, e) => {
                        Button btn = (Button)s;
                        if (btn != null)
                        {
                            select = (btn.Name);
                            seat(select);
                        }
                    };
                    pan.Controls.Add(pt[i, 1]);

                    pt[i, 2] = new Button();
                    pt[i, 2].Location = new Point(300, i * 180 + 120);
                    pt[i, 2].Width = 100;
                    pt[i, 2].Height = 30;
                    pt[i, 2].BackColor = Color.Red;
                    pt[i, 2].ForeColor = Color.White;
                    pt[i, 2].Name = dbmovie[i] + "|3";
                    pt[i, 2].Text = "20:00 WIB";
                    pt[i, 2].Click += (s, e) => {
                        Button btn = (Button)s;
                        if (btn != null)
                        {
                            select = (btn.Name);
                            seat(select);
                        }
                    };
                    pan.Controls.Add(pt[i, 2]);
                }
            }
            else
            {
                this.Controls.Clear();
            }
            
        }
        // fungsi untuk inisialisasi random reservation dengan random reservasi paling banyak 60% (kurang dari 70%)
        public void random_reserve()
        {
            var r1 = new Random();
            for (int i = 0; i < dbmovie_count; i ++)
            {
                for(int j = 0; j < 3; j++)
                {
                    for (int s = 0; s < 60; s++)
                    {
                        int x = r1.Next(0, 9);
                        int y = r1.Next(0, 9);
                        reserve[i, j, x, y, 0] = true;
                    }
                }
            }
        }
        // fungsi untuk menu pemilihan seat oleh pengunjung
        public void seat(string mov)
        {
            menu(false);
            string movie = "";
            int movie_i = 0;
            string time = "";
            int time_i = 0;
            int ix = mov.IndexOf("|");
            string yr_str = "";

            if (ix != -1)
            {
                movie = mov.Substring(0, ix);
                int s_length = mov.Length - ix - 1;
                time = mov.Substring(ix + 1, s_length);
            }

            if(time == "1")
            {
                time_i = 0;
                time = "11:00";
            }
            else if (time == "2")
            {
                time_i = 1;
                time = "15:00";
            }
            else if (time == "3")
            {
                time_i = 2;
                time = "20:00";
            }

            for(int x = 0; x < dbmovie_count; x++)
            {
                if(movie == dbmovie[x])
                {
                    movie_i = x;
                }
            }

            Label title = new Label();
            title.Text = "Reserve A Seat For Movie : " + movie + " at " + time + " WIB";
            title.Location = new Point(5, 5);
            title.Width = 600;
            title.Height = 20;
            title.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Controls.Add(title);

            Label st = new Label();
            st.Text = "click the seat to select";
            st.Location = new Point(8, 30);
            st.Width = 600;
            st.Height = 15;
            st.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Controls.Add(st);

            Panel pan = new Panel();
            pan.Location = new Point(10, 50);
            pan.Width = 500;
            pan.Height = 500;
            pan.BackColor = System.Drawing.Color.White;
            pan.AutoScroll = false;
            pan.HorizontalScroll.Enabled = false;
            pan.HorizontalScroll.Visible = false;
            pan.HorizontalScroll.Maximum = 0;
            pan.AutoScroll = true;
            this.Controls.Add(pan);

            Label lb = new Label();
            lb.Location = new Point(10, 5);
            lb.Width = 480;
            lb.Height = 20;
            lb.BackColor = Color.Black;
            lb.ForeColor = Color.White;
            lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lb.Text = "Screen";
            lb.TextAlign = ContentAlignment.MiddleCenter;
            pan.Controls.Add(lb);

            int y = 0;
            for (char c = 'A'; c < 'K'; c++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button bt = new Button();
                    bt.Location = new Point(x*45 + 25, y * 40 + 30);
                    bt.Width = 40;
                    bt.Height = 40;
                    if(reserve[movie_i, time_i, x, y,0])
                    {
                        bt.BackColor = Color.Red;
                        bt.Enabled = false;
                    }
                    else
                    {
                        bt.BackColor = Color.White;
                    }
                    if (reserve[movie_i, time_i, x, y, 1])
                    {
                        yr_str += c + (x+1).ToString() + ", ";
                        bt.BackColor = Color.GreenYellow;
                    }
                    bt.ForeColor = Color.Black;
                    bt.Text = Char.ToString(c)+' '+(x+1).ToString();
                    bt.Name = Char.ToString(c) + x.ToString();
                    bt.Click += (s, e) => {
                        Button btn = (Button)s;
                        if (btn != null)
                        {
                            bt.BackColor = Color.GreenYellow;
                            book(movie_i, time_i, btn.Name, true);
                        }
                    };
                    pan.Controls.Add(bt);
                }
                y++;
            }

            Label yr = new Label();
            yr.Location = new Point(180, 450);
            yr.Width = 200;
            yr.Height = 30;
            yr.BackColor = Color.GreenYellow;
            yr.ForeColor = Color.Black;
            yr.Text = "Your Seat : " + yr_str;
            yr.TextAlign = ContentAlignment.MiddleLeft;
            pan.Controls.Add(yr);

            Button back = new Button();
            back.Location = new Point(20,450);
            back.Width = 70;
            back.Height = 30;
            back.BackColor = Color.White;
            back.ForeColor = Color.Black;
            back.Text = "Back";
            back.Click += (s, e) => {
                Button btn = (Button)s;
                if (btn != null)
                {
                    menu(false);
                    menu(true);
                }
            };
            pan.Controls.Add(back);

            Button res = new Button();
            res.Location = new Point(100, 450);
            res.Width = 70;
            res.Height = 30;
            res.BackColor = Color.White;
            res.ForeColor = Color.Black;
            res.Text = "Reset";
            res.Click += (s, e) => {
                Button btn = (Button)s;
                if (btn != null)
                {
                    reset_reservation(movie_i, time_i);
                    menu(false);
                    seat(mov);
                }
            };
            pan.Controls.Add(res);

            Button sv = new Button();
            sv.Location = new Point(400, 450);
            sv.Width = 70;
            sv.Height = 30;
            sv.BackColor = Color.White;
            sv.ForeColor = Color.Black;
            sv.Text = "Reserve";
            sv.Click += (s, e) => {
                Button btn = (Button)s;
                if (btn != null)
                {
                    menu(false);
                    seat(mov);
                }
            };
            pan.Controls.Add(sv);
        }

        // fungsi untuk mereset reservasi user terhadap pilihan movie & playing time tertentu
        public void reset_reservation(int movie_i, int time_i)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (reserve[movie_i, time_i, x, y, 1] == true)
                    {
                        reserve[movie_i, time_i, x, y, 0] = false;
                        reserve[movie_i, time_i, x, y, 1] = false;
                    }
                }
            }
        }

        // fungsi untuk menyimpan data user yang membooking seat pada pilihan movie & playing time tertentu
        public void book(int movie_i, int time_i, string n, bool vl)
        {
            int y = n[0]-65;
            int x = n[1]-48;
            reserve[movie_i, time_i, x, y, 0] = vl;
            reserve[movie_i, time_i, x, y, 1] = vl;
        }
    }
}
