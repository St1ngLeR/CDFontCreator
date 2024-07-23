namespace CDFontCreator
{
    public partial class Form1 : Form
    {
        List<byte> wid = new List<byte>();
        char[] characters;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 1;
            foreach (FontFamily font in FontFamily.Families)
            {
                listBox1.Items.Add(font.Name);
            }
            int index = listBox1.FindString("Arial");
            if (index != -1)
            {
                listBox1.SetSelected(index, true);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                label1.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
            }
            else
            {
                label1.Font = new Font(label1.Font, label1.Font.Style & ~FontStyle.Bold);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                label1.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Italic);
            }
            else
            {
                label1.Font = new Font(label1.Font, label1.Font.Style & ~FontStyle.Italic);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Font = new Font(listBox1.SelectedItem.ToString(), label1.Font.Size, label1.Font.Style);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.AutoScroll = true;
            button2.Enabled = true;
            wid.Clear();

            if (comboBox1.SelectedItem == "512")
            {
                pictureBox1.Width = 512;
                pictureBox1.Height = 512;
            }
            else if (comboBox1.SelectedItem == "1024")
            {
                pictureBox1.Width = 1024;
                pictureBox1.Height = 1024;
            }
            else if (comboBox1.SelectedItem == "2048")
            {
                pictureBox1.Width = 2048;
                pictureBox1.Height = 2048;
            }
            else if (comboBox1.SelectedItem == "4096")
            {
                pictureBox1.Width = 4096;
                pictureBox1.Height = 4096;
            }

            // Размеры для сетки символов
            int cols = 16;
            int rows = 16;

            // Получаем ширину и высоту для изображения из comboBox1
            int imgWidth = int.Parse((string)comboBox1.SelectedItem);
            int imgHeight = int.Parse((string)comboBox1.SelectedItem);

            // Размер ячейки для каждого символа
            int cellWidth = imgWidth / cols;
            int cellHeight = imgHeight / rows;

            // Инициализируем новое изображение
            Bitmap bitmap = new Bitmap(imgWidth, imgHeight);

            // Используем Graphics для рендеринга текста
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Заливаем фон
                graphics.Clear(Color.Black);

                // Получаем шрифт из label1
                Font font = new Font(label1.Font.FontFamily, 19 * (imgWidth / 512), label1.Font.Style);
                Font font_wid = new Font(label1.Font.FontFamily, 72, label1.Font.Style);

                // Настройки для выравнивания текста

                StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Near, // Выравнивание текста слева
                    LineAlignment = StringAlignment.Center // Выравнивание текста по середине вертикали
                };

                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        // Индекс символа в массиве
                        int index = (y * cols + x) % characters.Length;

                        // Текущий символ для отображения
                        char charToDraw = characters[index];

                        Rectangle cellRect = new Rectangle((x * cellWidth) - 6, (y * cellHeight) + 3, cellWidth, cellHeight);

                        // Рисуем символ в ячейке
                        graphics.DrawString(charToDraw.ToString(), font, Brushes.White, cellRect);

                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                        SizeF len = graphics.MeasureString(charToDraw.ToString(), font_wid);
                        //MessageBox.Show($"{charToDraw}\n{len.Width}");

                        int offset;

                        // Вся эта дичь с "offset" - это фикс расстояния между конкретными буквами. Я без понятия как это автоматизировать.
                        if ((charToDraw == 'i') || (charToDraw == 'I') || (charToDraw == 'l') || (charToDraw == '!') || (charToDraw == ';') || (charToDraw == ':') || (charToDraw == ',') || (charToDraw == '.') || (charToDraw == '`') || (charToDraw == '\'') || (charToDraw == '"'))
                        {
                            offset = -8;
                        }
                        else if ((charToDraw == 'w') || (charToDraw == 'W') || (charToDraw == 'm') || (charToDraw == 'M') || (charToDraw == 'ш') || (charToDraw == 'Ш') || (charToDraw == 'щ') || (charToDraw == 'Щ') || (charToDraw == 'м') || (charToDraw == 'М') || (charToDraw == 'ж') || (charToDraw == 'Ж'))
                        {
                            offset = 8;
                        }
                        else
                        {
                            offset = 0;
                        }
                        wid.Add((byte)((len.Width) + offset));
                    }
                }
            }

            // Показываем изображение в pictureBox1
            pictureBox1.Image = bitmap;
        }

        private void SaveToTga(Image image, string filename)
        {
            Bitmap originalImage = new Bitmap(image);

            int width = int.Parse((string)comboBox1.SelectedItem);
            int height = int.Parse((string)comboBox1.SelectedItem);

            // Шаг 2: Создать новое белое полотно размера, выбранного в comboBox1
            Bitmap newCanvas = new Bitmap(width, height);

            // Шаг 3: Использовать инвертированные цвета исходного изображения в качестве альфа-канала
            for (int y = 0; y < newCanvas.Height; y++)
            {
                for (int x = 0; x < newCanvas.Width; x++)
                {
                    Color alphaColor = originalImage.GetPixel(x, y);
                    int alpha = (alphaColor.R + alphaColor.G + alphaColor.B) / 3;
                    Color newColor = Color.FromArgb(alpha, 255, 255, 255); // Создание цвета с полученным альфа-каналом
                    newCanvas.SetPixel(x, y, newColor);
                }
            }

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                // Заголовок TGA
                byte[] header = new byte[18] { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                Array.Copy(BitConverter.GetBytes((short)width), 0, header, 12, 2);
                Array.Copy(BitConverter.GetBytes((short)height), 0, header, 14, 2);
                header[16] = 32; // Количество бит на пиксель, 24 для RGB, 32 для RGBA
                header[17] = 0;   // Описание изображения

                fs.Write(header, 0, header.Length);

                byte[] tga_bytes = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54, 0x52, 0x55, 0x45, 0x56, 0x49, 0x53, 0x49, 0x4F, 0x4E, 0x2D, 0x58, 0x46, 0x49, 0x4C, 0x45, 0x2E, 0x00 };

                // Данные изображения
                for (int y = height - 1; y >= 0; y--) // TGA файлы хранятся с низу вверх
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color = newCanvas.GetPixel(x, y);
                        fs.WriteByte(color.B); // Blue
                        fs.WriteByte(color.G); // Green
                        fs.WriteByte(color.R); // Red
                        fs.WriteByte(color.A); // Alpha
                    }
                }
                fs.Write(tga_bytes, 0, tga_bytes.Length);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "";
            sfd.DefaultExt = ".tga";
            sfd.Filter = "Crashday Font Files (*.tga, *.tex, *.wid)|*.tga";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveToTga(pictureBox1.Image, sfd.FileName);
                File.WriteAllText(Path.GetDirectoryName(sfd.FileName) + "\\" + Path.GetFileNameWithoutExtension(sfd.FileName) + ".tex", "has_alpha\r\ndisable_mipmapping\r\nSTANDARD");
                File.WriteAllBytes(Path.GetDirectoryName(sfd.FileName) + "\\" + Path.GetFileNameWithoutExtension(sfd.FileName) + ".wid", wid.ToArray());
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            characters = new char[] {
                ' ', '\u250c', '\u2510', '\u2514', '\u2518', '\u2502', '\u2500', '\u2022', '\u25d8', ' ', ' ', '\u2642', '\u2640', ' ', '\u266c', '\u2609',
                '\u253c', '\u25c0', '\u2195' ,'\u203c' ,'\u00b6', '\u2534', '\u252c', '\u2524', '\u2191', '\u251c', '\u2192', '\u2190', ' ', ' ', ' ', ' ',
                ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')' ,'*' ,'+', ',', '-', '.', '/',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?',
                '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
                'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_',
                '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
                'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', ' ',
                '\u0402', '\u0403', '\u0317', '\u0453', '\u02f6', '\u2026', '\u2020', '\u2021', '\u20ac', '\u2030','\u0409', '\u2039', '\u040a', '\u040c', '\u040b', '\u040f',
                '\u0452', '\u2035', '\u00b4', '\u030f', '\u030b', '\u26ab', '\u2013', '\u2014', ' ', '\u2122','\u0459', '\u203a', '\u045a', '\u045c', '\u045b', '\u045f',
                ' ', '\u040e', '\u045e', '\u00a3', '\u00a4', '\u0490', '\u254e', '\u00a7', '\u0401', '\u00a9','\u0404', '\u00ab', '\u00ac', '\u00ad', '\u00ae', '\u00cf',
                '\u00b0', '\u00b1', '\u00b2', '\u00b3', '\u00b4', '\u00b5', '\u00b6', '\u00b7', '\u0451', '\u2116','\u2218', '\u00bb', '\u00bc', '\u00bd', '\u00be', '\u00bf',
                'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П',
                'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я',
                'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п',
                'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я'};
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            characters = new char[] {
                ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')' ,'*' ,'+', ',', '-', '.', '/',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?',
                '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
                'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_',
                '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
                'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', '\u00a9',
                '\u00c0', '\u00c1', '\u00c2', '\u00c3', '\u00c4', '\u00c5', '\u00c6', '\u00c7', '\u00c8', '\u00c9','\u00ca', '\u00cb', '\u00cc', '\u00cd', '\u00ce', '\u00cf',
                '\u00d0', '\u00d1', '\u00d2', '\u00d3', '\u00d4', '\u00d5', '\u00d6', '\u00d7', '\u00d8', '\u00d9','\u00da', '\u00db', '\u00dc', '\u00dd', '\u00de', '\u00df',
                '\u00e0', '\u00e1', '\u00e2', '\u00e3', '\u00e4', '\u00e5', '\u00e6', '\u00e7', '\u00e8', '\u00e9','\u00ea', '\u00eb', '\u00ec', '\u00ed', '\u00ee', '\u00ef',
                '\u00f0', '\u00f1', '\u00f2', '\u00f3', '\u00f4', '\u00f5', '\u00f6', '\u00f7', '\u00f8', '\u00f9','\u00fa', '\u00fb', '\u00fc', '\u00fd', '\u00fe', '\u00ff',
                ' ', 'Б', 'В', 'Г', 'Д', 'Е', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П',
                'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я',
                'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р',
                'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я', '\u00b0',
                '\u00a1', '\u00bf', '\u00a3', '\u2032', '\u0401', '\u0451', '\u011f', '\u011e', '\u015f', '\u015e','\u0131', '\u0130', '\u0151', '\u0150', '\u0171', '\u0170',
                '\u015a', '\u0104', '\u0107', '\u0118', '\u0179', '\u0141', '\u017b', '\u0143', '\u015b', '\u0105','\u0107', '\u0119', '\u017a', '\u0142', '\u017c', '\u0144'};
        }
    }
}
