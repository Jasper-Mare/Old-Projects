namespace TerrainMaker {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        bool[] data;

        private void Form1_Load(object sender, EventArgs e) {
            
        }

        private void btn_setup_Click(object sender, EventArgs e) {
            panel1.Controls.Clear();
            int chunkSize = (int)numericUpDown1.Value;
            data = new bool[5*chunkSize*chunkSize];
            int spaceX = 20, spaceY = 20, index = 0;

            for (int t = 0; t < 5; t++) {
                for (int x = 0; x < chunkSize; x++) {
                    for (int y = 0; y < chunkSize; y++) {
                        Button button = new Button();
                        button.Location = new Point(x*spaceX+spaceX*2, y*spaceY+t*spaceY*(chunkSize+1)+spaceY );
                        button.Size = new Size(spaceX-2, spaceY-2);
                        button.Click += TerrainButton_Click;
                        button.Tag = index;

                        panel1.Controls.Add(button);
                        index++;
                    }
                }
            }
            
        }

        private void TerrainButton_Click(object? sender, EventArgs e) {
            if (sender is null) { return; }
            Button buttonPressed = (Button)sender;
            int index = (int)buttonPressed.Tag;

            data[index] = !data[index];

            if (data[index]) {
                buttonPressed.BackColor = SystemColors.ControlDark;
            } else { 
                buttonPressed.BackColor = SystemColors.ControlLight;
            }

        }

        private void btn_save_Click(object sender, EventArgs e) {
            int x,y;
            string input = Microsoft.VisualBasic.Interaction.InputBox("Input x and y pos of chunk e.g. 1,3");
            x = int.Parse(input.Split(',')[0]);
            y = int.Parse(input.Split(',')[1]);

            string path = Application.UserAppDataPath + "/Terrain/(" + x.ToString().PadLeft(4) + ", " + y.ToString().PadLeft(4) + ").chunk";

            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

            using (BinaryWriter writer = new BinaryWriter(stream)) {
                writer.Write(data.Length);
                for (int i = 0; i < data.Length; i++) { 
                    writer.Write(data[i]);
                }
            }

            stream.Close();
            MessageBox.Show("Saved to "+path);
        }
    }
}