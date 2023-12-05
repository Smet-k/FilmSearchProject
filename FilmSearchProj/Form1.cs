using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FilmSearchProj
{
    public partial class Form1 : Form
    {

        HttpClient client = new HttpClient();
        string requestUri = "";
        string APIKey = "da223b1f";

        int multiplier = 0;
        int page = 1;
        int totalMatches = 0;
        int startPos = 0;
        int endPos = 10;
        JObject obj;
        public string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            return $"{char.ToUpper(input[0])}{input[1..]}";
        }

        #region buttons
        List<Button> btns;

        Button Last = new Button()
        {
            Text = @">>",
            FlatStyle = FlatStyle.Popup,
            BackColor = Color.White,
            Cursor = Cursors.Hand,
        };

        Button Next = new Button()
        {
            Text = ">",
            FlatStyle = FlatStyle.Popup,
            BackColor = Color.White,
            Cursor = Cursors.Hand,
        };


        Button First = new Button()
        {
            Text = @"<<",
            FlatStyle = FlatStyle.Popup,
            BackColor = Color.White,
            Cursor = Cursors.Hand,
        };



        Button Prev = new Button()
        {
            Text = "<",
            FlatStyle = FlatStyle.Popup,
            BackColor = Color.White,
            Cursor = Cursors.Hand,
        };


        
        // http://www.omdbapi.com/?i=tt3896198&apikey=da223b1f API Key
        // http://www.omdbapi.com/?apikey=[yourkey]& DATA REQUEST https://www.omdbapi.com/?s=hello&apikey=da223b1f
        // http://img.omdbapi.com/?apikey=[yourkey]& IMAGE REQUEST 

        //Button funcs
        private void last(object sender, EventArgs e)
        {
            //page = totalMatches / 10;
            multiplier = totalMatches / 10 / 9;
            page = 1 + 9 * multiplier;
            getData();
            addButtons();
        }
        private void next(object sender, EventArgs e)
        {
            if ((multiplier + 1) * 9 - totalMatches / 10 <= 9)
            {
                //page += 9;
                multiplier += 1;
                page = 1 + 9 * multiplier;
            }

            getData();
            addButtons();

        }
        private void first(object sender, EventArgs e)
        {

            multiplier = 0;
            page = 1 + 9 * multiplier;

            getData();
            addButtons();
        }
        private void prev(object sender, EventArgs e)
        {
            if (multiplier != 0)
            {
                multiplier -= 1;
                page = 1 + 9 * multiplier;
            }
            getData();
            addButtons();
        }

        private void goToPage(object sender, EventArgs e)
        {
            Button send = sender as Button;
            page = Convert.ToInt32(send.Text);
            getData();

        }

        private void GetDetails(object sender, EventArgs e)
        {
            Button send = sender as Button;
            Detail detail = new Detail(send.Name);
            detail.Show();
            
        }
        
        private void addButtons()
        {
            btns = new List<Button>() { First, Prev };
            int maxMatches = 9;
            if (totalMatches < 9)
            {
                maxMatches = totalMatches;
            }
            for (int i = 1 + 9 * multiplier; i < 1 + 9 * multiplier + maxMatches; i++)
            {
                if (i <= totalMatches / 10)
                {
                    Button newbtn = new Button()
                    {
                        Text = i.ToString(),
                        FlatStyle = FlatStyle.Popup,
                        BackColor = Color.White,
                        Cursor = Cursors.Hand,
                    };
                    newbtn.Click += goToPage;
                    btns.Add(newbtn);
                }

            }
            btns.AddRange(new List<Button> { Next, Last });

            panel2.Controls.Clear();
            int x = panel2.Location.X, y = panel2.Location.Y - panel2.Height;
            for (int i = 0; i < btns.Count; i++)
            {

                panel2.Controls.Add(btns[i]);
                btns[i].Location = new Point(x, y);
                btns[i].Width = 40;
                panel2.Width += btns[i].Width;
                x += btns[i].Width;
            }


        }
        #endregion
        public Form1()
        {
            InitializeComponent();

            //Button setup
            Prev.Click += prev;
            First.Click += first;
            Next.Click += next;
            Last.Click += last;
            this.Text = "Netflix from Aliexpress dot com";
        }



        private async void loadPage()
        {
            label3.Text = $"Page:{page}";
            panel1.Controls.Clear();
            if (endPos > totalMatches)
            {
                endPos = totalMatches;
            }
            else if (endPos + 10 * page > totalMatches)
            {
                endPos = totalMatches - 10 * page + 1;
            }
            else { endPos = 10; }
            int x = panel1.Location.X, y = 30;

            for (int i = startPos; i < endPos; i++)
            {
                GroupBox group = new GroupBox();

                #region getting poster
                string pictureURI = obj["Search"][i]["Poster"].ToString();

                PictureBox picture = new PictureBox();
                if (pictureURI != "N/A")
                {
                    try
                    {
                        byte[] data = await client.GetByteArrayAsync(pictureURI);


                        using (var ms = new MemoryStream(data))
                        {
                            picture.Image = new Bitmap(ms);
                        }
                        picture.SizeMode = PictureBoxSizeMode.StretchImage;
                        picture.Size = new Size(250, 225);
                        picture.Location = new Point(group.Location.X + 20, group.Location.Y + 20);
                    }
                    catch (Exception ex)
                    {
                        picture.ImageLocation = @"..\Images\Placeholder.png";
                        picture.SizeMode = PictureBoxSizeMode.StretchImage;
                        picture.Size = new Size(250, 225);
                        picture.Location = new Point(group.Location.X + 20, group.Location.Y + 20);

                    }
                }
                else
                {
                    picture.ImageLocation = @"..\Images\Placeholder.png";
                    picture.SizeMode = PictureBoxSizeMode.StretchImage;
                    picture.Size = new Size(250, 225);
                    picture.Location = new Point(group.Location.X + 20, group.Location.Y + 20);
                }
                #endregion

                #region creating members
                Label title = new Label();
                title.Text = obj["Search"][i]["Title"].ToString();
                title.Location = new Point(group.Location.X + 20, group.Location.Y + 20 + picture.Height);

                Label year = new Label();
                year.Text = obj["Search"][i]["Year"].ToString();
                year.Location = new Point(group.Location.X + 20, group.Location.Y + picture.Height + 40);

                Label type = new Label();
                type.Text = FirstCharToUpper(obj["Search"][i]["Type"].ToString());
                type.Location = new Point(group.Location.X + 20, group.Location.Y + picture.Height + 60);

                Button details = new Button();
                details.Text = "Details";
                details.Location = new Point(group.Location.X + 20, group.Location.Y + picture.Height + 80);
                details.Name = obj["Search"][i]["imdbID"].ToString();
                details.FlatStyle = FlatStyle.Popup;
                details.BackColor = Color.White;
                details.Cursor = Cursors.Hand;
                details.Click += GetDetails;
                //imdbID

                group.Controls.Add(picture);
                group.Controls.Add(title);
                group.Controls.Add(year);
                group.Controls.Add(type);
                group.Controls.Add(details);
                group.Size = new Size(350, 400);
                #endregion
                panel1.Controls.Add(group);

                group.Location = new Point(x, y);

                x += group.Width;
                if (x >= this.Width - group.Width)
                {
                    y += group.Height;
                    x = panel1.Location.X;
                }


            }
        }

        private async void getDataAsync()
        {
            string result = await client.GetStringAsync(requestUri);

            obj = JObject.Parse(result);

            totalMatches = Convert.ToInt32(obj["totalResults"].ToString());
            if (panel2.Controls.Count == 0)
            {
                addButtons();
            }

            loadPage();
        }


        private void getData()
        {
            requestUri = $"https://www.omdbapi.com/?s={textBox1.Text}&type={comboBox1.SelectedItem}&page={page}&apikey={APIKey}";
            getDataAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            page = 1;
            endPos = 10;
            startPos = 0;
            label3.Visible = true;
            getData();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && textBox1.Text != "") { button1.Enabled = true; }
            else { button1.Enabled = false; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && textBox1.Text != "") { button1.Enabled = true; }
            else { button1.Enabled = false; }
        }

    }
}