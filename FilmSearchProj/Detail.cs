using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FilmSearchProj
{
    public partial class Detail : Form
    {
        string id;
        JObject obj;
        HttpClient client = new HttpClient();

        //string result = await client.GetStringAsync(requestUri);

        //obj = JObject.Parse(result);
        public Detail()
        {
            InitializeComponent();
        }

        public Detail(string id)
        {
            InitializeComponent();
            this.id = id;
            this.Width = 800;
            this.Height = 500;
            loadDetails();
        }

        public string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            return $"{char.ToUpper(input[0])}{input[1..]}";
        }

        private async void loadDetails() 
        {

            string result = await client.GetStringAsync($"https://www.omdbapi.com/?i={id}&plot=full&apikey=da223b1f");

            obj = JObject.Parse(result);

            #region getting poster
            string pictureURI = obj["Poster"].ToString();

            if (pictureURI != "N/A")
            {
                try
                {
                    byte[] data = await client.GetByteArrayAsync(pictureURI);


                    using (var ms = new MemoryStream(data))
                    {
                        pictureBox1.Image = new Bitmap(ms);
                    }
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    pictureBox1.ImageLocation = @"..\Images\Placeholder.png";
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else
            {
                pictureBox1.ImageLocation = @"..\Images\Placeholder.png";
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            }
            #endregion

            this.Text = obj["Title"].ToString();

            label1.Text = $"{obj["Title"]}";
            
            label2.Text = $"{obj["Year"]}";

            //label4.Text = FirstCharToUpper(obj["Type"].ToString());
            label4.Text = $"{FirstCharToUpper(obj["Type"].ToString())}";

            label5.Text = $"{obj["Actors"]}";

            label11.Text = $"{obj["Awards"]}";

            int i = 0;
            label13.Text = "";
            while (true)
            {
                try
                {
                    label13.Text += $"{obj["Ratings"][i]["Source"]}:{obj["Ratings"][i]["Value"]}\n";
                    i++;
                }
                catch 
                {
                    break;
                }
            }
            

            string plot = obj["Plot"].ToString();
            int lengthCounter = 0;
            Label tmp = new Label() { Text = "", Width = 0, AutoSize = true, Visible = false };
            this.Controls.Add(tmp);
            foreach (var letter in plot) 
            {
                tmp.Text += letter;

                label3.Text += letter;

                lengthCounter += tmp.Width;

                tmp.Text = "";
                if(lengthCounter > this.Width) 
                {
                    label3.Text += "\n";
                    lengthCounter = 0;
                }
            }
        }

        private void Detail_Load(object sender, EventArgs e)
        {

        }
    }
}
