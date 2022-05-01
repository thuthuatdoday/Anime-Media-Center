using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Anime_Media_Center
{
    public partial class frmMain : Form
    {
        public frmMain() // khoi tao frmMain
        {
            InitializeComponent();
        }

        // bien toan cuc
        private string appDir = AppDomain.CurrentDomain.BaseDirectory; // thu muc root
        private List<string> listItemAnime = new List<string>(); // danh sach anime
        private List<string> listItemEpisode = new List<string>(); // danh sach episode
        private string recentScan = ""; // folder goc anime mo gan day
        private string recentAnime = ""; // anime lua chon gan day
        private string recentImage = ""; // folder anh mo gan day
        private string recentFolder = ""; // folder phim mo gan day

        private string AnimeDataName(string anime) // chuan hoa ten data anime
        {
            // quy tac chuan hoa
            anime = Regex.Replace(anime, @"[\?]", "_"); // thay the ? => _
            anime = Regex.Replace(anime, @"[^a-zA-Z0-9 \-_+\[\]\(\)!.,]{1,}", "-"); // [a-z], [A-Z], [0-9], "[]()+-_!., "
            anime = Regex.Replace(anime, @"[\-]{1,}", "-"); // xoa "-[-]"
            anime = Regex.Replace(anime, @"[ ]{1,}", " "); // xoa " [ ]"
            anime = Regex.Replace(anime, @"\- ( |\-){1,}", "- "); // xoa "-[ |-]"

            // tra lai ten data anime chuan hoa
            return anime;
        }

        private string AnimeShortName(string anime) // rut gon ten anime
        {
            // quy tac rut gon
            anime = Regex.Replace(anime, @" \[(.*)\]$", ""); // xoa chu thich

            // tra lai ten anime rut gon
            return anime;
        }

        private Image ResizeImage(Image image, int width, int height) // giu ti le khi thay doi kich thuoc hinh anh
        {
            // khoi tao hinh anh dich
            Bitmap newImage = new Bitmap(width, height); // thiet lap kich thuoc
            Graphics graphics = Graphics.FromImage((Image)newImage); // bat dau chinh sua

            // thong so nguon
            int posX = 0; // toa do X
            int posY = 0; // toa do Y
            int srcWidth = image.Width; // chieu rong
            int srcHeight = image.Height; // chieu cao

            // ti le hien thi
            double srcRatio = (double)(srcWidth / srcHeight); // ti le nguon
            double descRatio = (double)(width / height); // ti le dich

            // tinh toan lai thong so nguon cho phu hop

            if (srcRatio < descRatio) // ti le nguon < dich, chieu cao anh xa be hon
            {
                int newHeight = (int)(srcWidth * height / width); // tinh chieu cao anh xa
                posY = (int)((srcHeight - newHeight) / 2); // tinh toa do Y moi
                srcHeight = newHeight; // thiet lap chieu cao moi
            }

            if (srcRatio > descRatio) // ti le nguon > dich, chieu rong anh xa be hon
            {
                int newWidth = (int)(srcHeight * width / height); // tinh chieu rong anh xa
                posX = (int)((srcWidth - newWidth) / 2); // tinh toa do X moi
                srcWidth = newWidth; // thiet lap chieu rong moi
            }

            // tien hanh chinh sua
            Rectangle srcRectangle = new Rectangle(posX, posY, srcWidth, srcHeight); // thong so nguon
            Rectangle descRectangle = new Rectangle(0, 0, width, height); // thong so dich
            graphics.DrawImage(image, descRectangle, srcRectangle, GraphicsUnit.Pixel); // ve lai hinh anh
            graphics.Dispose(); // ket thuc chinh sua

            // tra lai hinh anh moi
            return (Image)newImage;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format) // lay thong tin bo giai ma hinh anh
        {
            // thong tin bo giai ma
            ImageCodecInfo imageCodecInfo = null;

            // danh sach bo giai ma
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            // tim kiem thong tin bo giai ma
            foreach (ImageCodecInfo codec in codecs) // duyet danh sach
            {
                if (format.Guid == codec.FormatID) // thong tin trung khop
                {
                    imageCodecInfo = codec; // lay thong tin bo giai ma tim duoc
                }
            }

            // tra lai thong tin bo giai ma tim duoc
            return imageCodecInfo;
        }

        private class MyComparer : IComparer<string> // so sanh ten file su dung Windows API
        {
            // goi lop thu vien Windows API
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]

            // goi ham Windows API
            private static extern int StrCmpLogicalW(string x, string y);

            // su dung ham Windows API
            public int Compare(string x, string y)
            {
                return StrCmpLogicalW(x, y);
            }
        }

        private Stream StreamFrom(string url) // doc du lieu tu url
        {
            // khoi tao tien trinh
            Process process = new Process();

            // thiet lap thong so tien trinh
            process.StartInfo.FileName = appDir + "curl.exe"; // duong dan curl
            process.StartInfo.Arguments = url; // tham so
            process.StartInfo.UseShellExecute = false; // tat giao dien
            process.StartInfo.CreateNoWindow = true; // tat tao cua so moi
            process.StartInfo.RedirectStandardOutput = true; // xuat du lieu tieu chuan
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; // an cua so

            // chay tien trinh
            process.Start();

            // doc du lieu
            Stream data = process.StandardOutput.BaseStream;

            // tra lai du lieu doc duoc
            return data;
        }

        private string SeasonFrom(string startTime) // chuyen doi thoi gian thanh mua anime
        {
            string season = ""; // mua anime

            // kiem tra dinh dang chuoi: month day, year
            if (Regex.IsMatch(startTime, @"^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[\ ][0-9]{1,2}\,[\ ][0-9]{4}")) // dinh dang ho tro
            {
                season = Regex.Match(startTime, @"^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[\ ][0-9]{1,2}\,[\ ][0-9]{4}").Value; // tach thong tin thoi gian
                string month = Regex.Match(season, @"^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)").Value; // tach thang
                string year = Regex.Match(season, @"[0-9]{4}$").Value; // tach nam

                season = ""; // thiet lap lai mmua anime

                // chuyen doi thoi gian thanh mua anime

                if (month == "Jan" || month == "Feb" || month == "Mar") // thang 1-3: mua dong
                {
                    season = "Winter " + year; // dinh dang mua dong: "Winter {year}"
                }

                if (month == "Apr" || month == "May" || month == "Jun") // thang 4-6: mua xuan
                {
                    season = "Spring " + year; // dinh dang mua xuan: "Spring {year}"
                }

                if (month == "Jul" || month == "Aug" || month == "Sep") // thang 7-9: mua he
                {
                    season = "Summer " + year; // dinh dang mua he: "Summer {year}"
                }

                if (month == "Oct" || month == "Nov" || month == "Dec") // thang 10-12: mua thu
                {
                    season = "Fall " + year; // dinh dang mua thu: "Fall {year}"
                }
            }

            // tra lai dinh dang mua anime
            return season;
        }

        private string StringFrom(string input, string start, string end) // tach chuoi trong doan chi dinh
        {
            // chuoi tach duoc
            string output = "";

            // tinh toan thong so start
            start = Regex.Match(input, start).Value; // tao chuoi start
            int startIndex = (start != "") ? input.IndexOf(start) : -1; // tim vi tri start

            // kiem tra ket qua tim kiem start
            if (startIndex != -1) // tim thay start
            {
                // xoa chuoi den het vi tri start
                startIndex = startIndex + start.Length; // xac dinh vi tri bat dau lay
                int length = input.Length - startIndex; // xac dinh chieu dai chuoi lay
                output = input.Substring(startIndex, length); // cat chuoi

                // tinh toan thong so end
                end = Regex.Match(output, end).Value; // tao chuoi end
                startIndex = (end != "") ? output.IndexOf(end) : -1; // tim vi tri end

                // kiem tra ket qua tim kiem end
                if (startIndex != -1) // tim thay end
                {
                    // xoa chuoi tu vi tri end den het
                    length = startIndex; // xac dinh chieu dai chuoi lay
                    startIndex = 0; // xac dinh vi tri bat dau lay
                    output = output.Substring(startIndex, length); // cat chuoi
                }
                else // khong tim thay end
                {
                    output = ""; // dat lai chuoi
                }
            }

            // chuan hoa chuoi lay duoc
            if (output != "") // chuoi khac rong
            {
                // quy tac chuan hoa
                output = Regex.Replace(output, @"<span.*?span>", ""); // loai bo tag span
                output = Regex.Replace(output, @"<.*?>", ""); // loai bo tag html
                output = Regex.Replace(output, @"[ ]{1,}", " "); // xoa " [ ]"
                output = Regex.Replace(output, @"^[ ]{1,}", ""); // xoa " " dau chuoi
                output = Regex.Replace(output, @"[ ]{1,}$", ""); // xoa " " cuoi chuoi
            }

            // tra lai chuoi tim duoc
            return output;
        }

        private void loadAppExec() // khoi tao chuong trinh
        {
            // kiem tra file Data.xml
            if (!File.Exists(appDir + "Data.xml")) // khong tim thay file Data.xml
            {
                // tao file Data.xml <= null

                XmlTextWriter dataXml = new XmlTextWriter(appDir + "Data.xml", Encoding.UTF8); // mo file Data.xml
                dataXml.Formatting = Formatting.Indented; // thiet lap dinh dang
                dataXml.WriteStartDocument(); // bat dau noi dung

                // ghi the <list> <= null
                dataXml.WriteStartElement("list");
                dataXml.WriteString("");
                dataXml.WriteEndElement();

                dataXml.WriteEndDocument(); // ket thuc noi dung
                dataXml.Flush(); // giai phong bo nho
                dataXml.Close(); // dong file Data.xml
            }

            // kiem tra folder Data
            if (!Directory.Exists(appDir + "Data")) // khong tim thay folder Data
            {
                Directory.CreateDirectory(appDir + "Data"); // tao folder Data
            }
        }

        private void loadListAnime() // hien thi danh sach anime: Data.xml => listItemAnime => lstAnime
        {
            // xoa danh sach anime
            lstAnime.Items.Clear(); // xoa lstAnime
            listItemAnime.Clear(); // xoa listItemAnime

            // doc danh sach anime: Data.xml => listItemAnime

            XmlTextReader dataXml = new XmlTextReader(appDir + "Data.xml"); // mo file Data.xml

            while (dataXml.Read()) // doc noi dung file Data.xml
            {
                if (dataXml.NodeType == XmlNodeType.Text) // doc noi dung title the <anime>
                {
                    if (dataXml.Value != "\r\n  ") // title khac rong
                    {
                        listItemAnime.Add(dataXml.Value); // them title vao listItemAnime
                    }
                }
            }

            dataXml.Close(); // dong file Data.xml

            // hien thi danh sach anime: listItemAnime => lstAnime

            foreach (string anime in listItemAnime) // duyet listItemAnime
            {
                lstAnime.Items.Add(anime); // them anime vao lstAnime
            }

            // chon anime
            if (lstAnime.Items.Count != 0) // co anime
            {
                // kiem tra co anime ghi nho hay khong
                if (recentAnime != "") // co anime duoc ghi nho
                {
                    int index = lstAnime.FindString(recentAnime); // xac dinh vi tri trong danh sach

                    if (index != -1) // tim thay anime
                    {
                        lstAnime.SelectedIndex = index; // chon anime
                    }
                }

                // kiem tra da chon anime hay chua
                if (lstAnime.SelectedIndex == -1) // chua chon anime
                {
                    lstAnime.SelectedIndex = 0; // chon anime dau tien
                }

                // cuon thanh cuon len vi tri duoc chon
                if (lstAnime.SelectedIndex < 19) // anime duoc chon: item < 19
                {
                    lstAnime.TopIndex = 0; // cuon thanh cuon len dau tien
                }
                else // anime duoc chon: 19 =< item < end
                {
                    if (lstAnime.SelectedIndex < lstAnime.Items.Count - 19) // anime duoc chon: 19 =< item < end - 19
                    {
                        lstAnime.TopIndex = lstAnime.SelectedIndex - 18; // cuon thanh cuon chinh giua
                    }
                    else // anime duoc chon: item > end - 19
                    {
                        lstAnime.TopIndex = lstAnime.Items.Count - 19; // cuon thanh cuon xuong cuoi cung
                    }
                }
            }
        }

        private void saveListAnime() // luu danh sach anime: listItemAnime => Data.xml
        {
            // sap xep listItemAnime
            listItemAnime.Sort(new MyComparer());

            // luu danh sach anime: listItemAnime => Data.xml

            XmlTextWriter dataXml = new XmlTextWriter(appDir + "Data.xml", Encoding.UTF8); // mo file Data.xml
            dataXml.Formatting = Formatting.Indented; // thiet lap dinh dang
            dataXml.WriteStartDocument(); // bat dau noi dung
            dataXml.WriteStartElement("list"); // mo the <list>

            // ghi the <anime> <= listItemAnime
            foreach (string anime in listItemAnime)
            {
                dataXml.WriteStartElement("anime");
                dataXml.WriteString(anime);
                dataXml.WriteEndElement();
            }

            dataXml.WriteEndElement(); // dong the <list>
            dataXml.WriteEndDocument(); // ket thuc noi dung
            dataXml.Flush(); // giai phong bo nho
            dataXml.Close(); // dong file Data.xml
        }

        private void loadListEpisode() // hien thi danh sach tap phim: txtLink => listItemEpisode => lstEpisode
        {
            // xoa danh sach tap phim
            lstEpisode.Items.Clear(); // xoa lstEpisode
            listItemEpisode.Clear(); // xoa listItemEpisode

            // hien thi so tap phim
            if (txtLink.Text == "") // txtLink chua nhap
            {
                txtEpisode.Text = Properties.Resources.Episode; // txtEpisode => null
            }
            else // txtLink da nhap
            {
                txtEpisode.Text = "0"; // txtEpisode => "0"
            }

            // kiem tra duong dan txtLink
            if (Directory.Exists(txtLink.Text)) // tim thay duong dan txtLink
            {
                string[] listFile = Directory.GetFiles(txtLink.Text, "*.*", SearchOption.TopDirectoryOnly); // lay danh sach tap phim
                listItemEpisode = new List<string>(listFile); // khoi tao danh sach tap phim
                listItemEpisode.Sort(new MyComparer()); // sap xep danh sach tap phim

                // hien thi danh sach tap phim: listItemEpisode => lstEpisode

                foreach (string fileName in listItemEpisode) // duyet listItemEpisode
                {
                    lstEpisode.Items.Add(Path.GetFileName(fileName)); // them ten file vao lstEpisode
                }

                // hien thi so tap phim
                txtEpisode.Text = lstEpisode.Items.Count.ToString();
            }
        }

        private void loadAnimeInfo() // doc thong tin anime: lstAnime => Anime.xml => frmMain
        {
            // form danh sach anime

            // btAdd
            btAdd.Enabled = true; // hien nut

            // form thong tin anime

            // txtUrl
            txtUrl.ReadOnly = true; // khong cho nhap
            txtUrl.Text = Properties.Resources.Url; // chua nhap

            // btGet
            btGet.Enabled = false; // an nut

            // txtTitle
            txtTitle.ReadOnly = true; // khong cho nhap
            txtTitle.Text = Properties.Resources.Title; // chua nhap

            // cbType
            cbType.Enabled = false; // khong cho chon
            cbType.SelectedIndex = -1; // chua chon

            // txtType
            txtType.ReadOnly = true; // khong cho nhap
            txtType.Text = Properties.Resources.Type; // chua nhap

            // txtGenre
            txtGenre.ReadOnly = true; // khong cho nhap
            txtGenre.Text = Properties.Resources.Genre; // chua nhap

            // txtStatus
            txtStatus.ReadOnly = true; // khong cho nhap
            txtStatus.Text = Properties.Resources.Status; // chua nhap

            // cbStatus
            cbStatus.Enabled = false; // khong cho chon
            cbStatus.SelectedIndex = -1; // chua chon

            // txtRelease
            txtRelease.ReadOnly = true; // khong cho nhap
            txtRelease.Text = Properties.Resources.Release; // chua nhap

            // txtSeason
            txtSeason.ReadOnly = true; // khong cho nhap
            txtSeason.Text = Properties.Resources.Season; // chua nhap

            // txtInfo
            txtInfo.ReadOnly = true; // khong cho nhap
            txtInfo.Text = Properties.Resources.Info; // chua nhap

            // btOpenImg
            btOpenImg.Enabled = false; // an nut

            // txtCover
            txtCover.Text = Properties.Resources.Cover; // dat lai duong dan

            // imgCover
            imgCover.Image = Properties.Resources.Empty; // dat lai anh bia

            // btOpenDir
            btOpenDir.Enabled = false; // an nut

            // txtLink
            txtLink.Text = Properties.Resources.Link; // dat lai duong dan

            // txtEpisode
            txtEpisode.Text = Properties.Resources.Episode; // xoa so luong tap tin

            // lstEpisode
            lstEpisode.Items.Clear(); // xoa danh sach tap phim

            // btSave
            btSave.Enabled = false; // an nut

            // doc thong tin anime
            if (lstAnime.Items.Count == 0) // khong co anime
            {
                // form danh sach anime

                // btEdit
                btEdit.Enabled = false; // an nut

                // btRemove
                btRemove.Enabled = false; // an nut

                // hien thi thong bao
                txtMessage.Text = "Show data: no anime";
            }
            else // co anime
            {
                // chon anime dau tien neu chua chon
                if (lstAnime.SelectedIndex == -1) // chua chon anime
                {
                    lstAnime.SelectedIndex = 0; // chon anime dau tien
                }

                // cuon thanh cuon len vi tri duoc chon
                if (lstAnime.SelectedIndex < 19) // anime duoc chon: item < 19
                {
                    lstAnime.TopIndex = 0; // cuon thanh cuon len dau tien
                }
                else // anime duoc chon: 19 =< item < end
                {
                    if (lstAnime.SelectedIndex < lstAnime.Items.Count - 19) // anime duoc chon: 19 =< item < end - 19
                    {
                        lstAnime.TopIndex = lstAnime.SelectedIndex - 18; // cuon thanh cuon chinh giua
                    }
                    else // anime duoc chon: item > end - 19
                    {
                        lstAnime.TopIndex = lstAnime.Items.Count - 19; // cuon thanh cuon xuong cuoi cung
                    }
                }

                // form danh sach anime

                // btEdit
                btEdit.Enabled = true; // hien nut

                // btRemove
                btRemove.Enabled = true; // hien nut

                string anime = lstAnime.SelectedItem.ToString(); // ten anime
                string dataFile = AnimeDataName(anime); // ten data anime

                // kiem tra data anime
                if (!File.Exists(appDir + "Data\\" + dataFile + ".xml")) // khong tim thay file Anime.xml
                {
                    txtMessage.Text = "Data not found: " + AnimeShortName(anime); // hien thi thong bao
                }
                else // tim thay file Anime.xml
                {
                    // hien thi thong bao
                    txtMessage.Text = "Show data: " + AnimeShortName(anime);

                    // doc thong tin anime: Anime.xml => frmMain

                    XmlTextReader dataXml = new XmlTextReader(appDir + "Data\\" + dataFile + ".xml"); // mo file Anime.xml

                    while (dataXml.Read()) // doc noi dung file Anime.xml
                    {
                        if (dataXml.NodeType == XmlNodeType.Element) // doc the
                        {
                            switch (dataXml.Name) // ten the
                            {
                                case "title": // doc the <title> => txtTitle
                                    dataXml.Read();
                                    txtTitle.Text = dataXml.Value;
                                    break;

                                case "type": // doc the <type> => cbType
                                    dataXml.Read();
                                    cbType.SelectedIndex = (dataXml.Value == "\r\n  ") ? Int32.Parse("-1") : Int32.Parse(dataXml.Value);
                                    break;

                                case "index": // doc the <index> => txtType
                                    dataXml.Read();
                                    txtType.Text = (dataXml.Value == "\r\n  ") ? Properties.Resources.Type : dataXml.Value;
                                    break;

                                case "genre": // doc the <genre> => txtGenre
                                    dataXml.Read();
                                    txtGenre.Text = (dataXml.Value == "\r\n  ") ? Properties.Resources.Genre : dataXml.Value;
                                    break;

                                case "episode": // doc the <episode> => txtStatus
                                    dataXml.Read();
                                    txtStatus.Text = (dataXml.Value == "\r\n  ") ? Properties.Resources.Status : dataXml.Value;
                                    break;

                                case "status": // doc the <status> => cbStatus
                                    dataXml.Read();
                                    cbStatus.SelectedIndex = (dataXml.Value == "\r\n  ") ? Int32.Parse("-1") : Int32.Parse(dataXml.Value);
                                    break;

                                case "release": // doc the <release> => txtRelease
                                    dataXml.Read();
                                    txtRelease.Text = (dataXml.Value == "\r\n  ") ? Properties.Resources.Release : dataXml.Value;
                                    break;

                                case "season": // doc the <season> => txtSeason
                                    dataXml.Read();
                                    txtSeason.Text = (dataXml.Value == "\r\n  ") ? Properties.Resources.Season : dataXml.Value;
                                    break;

                                case "info": // doc the <info> => txtInfo
                                    dataXml.Read();
                                    txtInfo.Text = (dataXml.Value == "\r\n  ") ? Properties.Resources.Info : dataXml.Value;
                                    break;

                                case "cover": // doc the <cover> => txtCover => imgCover

                                    // <cover> => txtCover
                                    dataXml.Read();
                                    txtCover.Text = (dataXml.Value == "\r\n  ") ? Properties.Resources.Cover : dataXml.Value;

                                    // txtCover => imgCover
                                    if (File.Exists(appDir + "Data\\" + txtCover.Text + ".jpg")) // tim thay file Anime.jpg
                                    {
                                        FileStream fileStream = new FileStream(appDir + "Data\\" + txtCover.Text + ".jpg", FileMode.Open); // mo file Anime.jpg
                                        imgCover.Image = Image.FromStream(fileStream); // doc du lieu Anime.jpg => imgCover
                                        fileStream.Close(); // dong file Anime.jpg
                                    }

                                    break;

                                case "link": // doc the <link> => txtLink => lstEpisode

                                    // <link> => txtLink
                                    dataXml.Read();
                                    txtLink.Text = (dataXml.Value == "\r\n  ") ? Properties.Resources.Link : dataXml.Value;

                                    // txtLink => lstEpisode
                                    loadListEpisode();

                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    dataXml.Close(); // dong file Anime.xml
                }
            }
        }

        private void saveAnimeInfo() // luu thong tin anime: frmMain => Data.xml => Anime.xml, Anime.jpg
        {
            bool isVal = true; // da nhap

            if (txtTitle.Text == "") // txtTitle chua nhap
            {
                txtMessage.Text = "Title: invalid"; // hien thi thong bao
                isVal = false; // chua nhap
            }

            if (cbType.SelectedIndex != -1) // cbType da chon
            {
                if (cbType.SelectedItem.ToString() == "Season" && txtType.Text == "") // cbType chon "Season", txtType chua nhap
                {
                    txtMessage.Text = "Season <index>: invalid"; // hien thi thong bao
                    isVal = false; // chua nhap
                }
            }

            if (isVal) // da nhap
            {
                // ten anime: txtTitle cbType txtType [txtStatus] [txtSeason] [cbStatus]

                string anime = txtTitle.Text; // ten anime

                if (cbType.SelectedIndex != -1) // da chon cbType
                {
                    if (cbType.SelectedItem.ToString() != "TV Series") // cbType chon khac "TV Series"
                    {
                        anime = anime + " - " + cbType.SelectedItem.ToString(); // ten anime: txtTitle cbType

                        if (txtType.Text != "") // txtType khac rong
                        {
                            anime = anime + " " + txtType.Text; // ten anime: txtTitle cbType txtType
                        }
                    }
                }

                if (txtStatus.Text != "") // txtStatus khac rong
                {
                    anime = anime + " [" + txtStatus.Text + "]"; // ten anime: txtTitle cbType txtType [txtStatus]
                }

                if (txtSeason.Text != "") // txtSeason khac rong
                {
                    anime = anime + " [" + txtSeason.Text + "]"; // ten anime: txtTitle cbType txtType [txtStatus] [txtSeason]
                }

                if (cbStatus.SelectedIndex != -1) // da chon cbStatus
                {
                    anime = anime + " [" + cbStatus.SelectedItem.ToString() + "]"; // ten anime: txtTitle cbType txtType [txtStatus] [txtSeason] [cbStatus]
                }

                recentAnime = anime; // ghi nho anime

                string dataFile = AnimeDataName(anime); // ten data anime

                bool isSave = true; // cho phep luu anime

                if (btAdd.Enabled == false) // them moi anime
                {
                    // tim anime trung txtTitle cbType txtType

                    List<string> listItemResult = listItemAnime.FindAll(x => x.Contains(AnimeShortName(anime))); // lay danh sach anime chua shortname
                    List<string> listItemRemove = new List<string>(); // danh sach anime trung shortname se xoa

                    foreach (string item in listItemResult) // duyet ket qua tim kiem
                    {
                        if (AnimeShortName(item) == AnimeShortName(anime)) // ket qua giong voi shortname
                        {
                            listItemRemove.Add(item); // them anime vao danh sach se xoa
                        }
                    }

                    if (listItemRemove.Count > 0) // co anime trong danh sach se xoa
                    {
                        DialogResult dialogResult = MessageBox.Show("Data anime: " + AnimeShortName(anime) + " already exists. Do you want to overwrite it?", "Add anime", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.Yes) // dong y ghi de thong tin anime
                        {
                            foreach (string item in listItemRemove) // duyet danh sach se xoa
                            {
                                // xoa anime khoi danh sach
                                listItemAnime.Remove(item);

                                // xoa data anime

                                if (File.Exists(appDir + "Data\\" + AnimeDataName(item) + ".xml")) // tim thay file Anime.xml
                                {
                                    File.Delete(appDir + "Data\\" + AnimeDataName(item) + ".xml"); // xoa file Anime.xml
                                }

                                if (File.Exists(appDir + "Data\\" + AnimeDataName(item) + ".jpg")) // tim thay file Anime.jpg
                                {
                                    File.Delete(appDir + "Data\\" + AnimeDataName(item) + ".jpg"); // xoa file Anime.jpg
                                }
                            }
                        }

                        if (dialogResult == DialogResult.No) // khong dong y ghi de thong tin anime
                        {
                            isSave = false; // khong luu anime
                        }
                    }
                }

                if (isSave) // cho phep luu anime
                {
                    if (btEdit.Enabled == false) // sua thong tin anime
                    {
                        removeAnimeInfo(); // xoa thonng tin anime dang lua chon
                    }

                    if (!File.Exists(appDir + "Data\\" + dataFile + ".xml")) // khong tim thay file Anime.xml
                    {
                        listItemAnime.Add(anime); // them anime vao danh sach: frmMain => lstItemAnime
                    }

                    saveListAnime(); // luu danh sach anime: listItemAnime => Data.xml

                    // luu thong tin anime: frmMain => Anime.xml

                    XmlTextWriter dataXml = new XmlTextWriter(appDir + "Data\\" + dataFile + ".xml", Encoding.UTF8); // mo file Anime.xml
                    dataXml.Formatting = Formatting.Indented; // thiet lap dinh dang
                    dataXml.WriteStartDocument(); // bat dau noi dung
                    dataXml.WriteStartElement("anime"); // mo the <anime>

                    // ghi the <title> <= txtTitle
                    dataXml.WriteStartElement("title");
                    dataXml.WriteString(txtTitle.Text);
                    dataXml.WriteEndElement();

                    // ghi the <type> <= cbType
                    dataXml.WriteStartElement("type");
                    dataXml.WriteString(cbType.SelectedIndex.ToString());
                    dataXml.WriteEndElement();

                    // ghi the <index> <= txtType
                    dataXml.WriteStartElement("index");
                    dataXml.WriteString(txtType.Text);
                    dataXml.WriteEndElement();

                    // ghi the <genre> <= txtGenre
                    dataXml.WriteStartElement("genre");
                    dataXml.WriteString(txtGenre.Text);
                    dataXml.WriteEndElement();

                    // ghi the <episode> <= txtStatus
                    dataXml.WriteStartElement("episode");
                    dataXml.WriteString(txtStatus.Text);
                    dataXml.WriteEndElement();

                    // ghi the <status> <= cbStatus
                    dataXml.WriteStartElement("status");
                    dataXml.WriteString(cbStatus.SelectedIndex.ToString());
                    dataXml.WriteEndElement();

                    // ghi the <release> <= txtRelease
                    dataXml.WriteStartElement("release");
                    dataXml.WriteString(txtRelease.Text);
                    dataXml.WriteEndElement();

                    // ghi the <season> <= txtSeason
                    dataXml.WriteStartElement("season");
                    dataXml.WriteString(txtSeason.Text);
                    dataXml.WriteEndElement();

                    // ghi the <info> <= txtInfo
                    dataXml.WriteStartElement("info");
                    dataXml.WriteString(txtInfo.Text);
                    dataXml.WriteEndElement();

                    // ghi the <cover> <= dataFile
                    dataXml.WriteStartElement("cover");
                    dataXml.WriteString(dataFile);
                    dataXml.WriteEndElement();

                    // ghi the <link> <= txtLink
                    dataXml.WriteStartElement("link");
                    dataXml.WriteString(txtLink.Text);
                    dataXml.WriteEndElement();

                    dataXml.WriteEndElement(); // dong the <anime>
                    dataXml.WriteEndDocument(); // ket thuc noi dung
                    dataXml.Flush(); // giai phong bo nho
                    dataXml.Close(); // dong ket file Anime.xml

                    // luu anh bia anime: imgCover => Anime.jpg

                    EncoderParameters encoderParameters = new EncoderParameters(1); // khoi tao tham so
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L); // thiet lap chat luong anh

                    Bitmap image = new Bitmap(imgCover.Image); // khoi tao anh bia
                    image.Save(appDir + "Data\\" + dataFile + ".jpg", GetEncoder(ImageFormat.Jpeg), encoderParameters); // luu anh bia
                    image.Dispose(); // giai phong bo nho

                    loadListAnime(); // hien thi danh sach anime
                    loadAnimeInfo(); // hien thi thong tin anime
                    txtMessage.Text = "Save data: " + AnimeShortName(recentAnime); // hien thi thong bao
                }
            }
        }

        private void removeAnimeInfo() // xoa thong tin anime: lstAnime => Data.xml => Anime.xml, Anime.jpg
        {
            if (lstAnime.SelectedIndex != -1) // da chon anime
            {
                string anime = lstAnime.SelectedItem.ToString(); // ten anime

                // xoa anime trong Data.xml
                listItemAnime.Remove(anime); // xoa anime khoi listItemAnime
                saveListAnime(); // luu danh sach anime: listItemAnime => Data.xml

                string dataFile = AnimeDataName(anime); // ten data anime

                // xoa du lieu trong Data

                if (File.Exists(appDir + "Data\\" + dataFile + ".xml")) // tim thay file Anime.xml
                {
                    File.Delete(appDir + "Data\\" + dataFile + ".xml"); // xoa file Anime.xml
                }

                if (File.Exists(appDir + "Data\\" + dataFile + ".jpg")) // tim thay file Anime.jpg
                {
                    File.Delete(appDir + "Data\\" + dataFile + ".jpg"); // xoa file Anime.jpg
                }
            }
        }

        private void frmMain_Load(object sender, EventArgs e) // khi khoi chay frmMain
        {
            loadAppExec();
            loadListAnime();
            loadAnimeInfo();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) // khi nhap txtSearch
        {
            // xoa danh sach anime
            lstAnime.Items.Clear();

            // che do tim kiem mo rong
            bool searchOption = false;

            // tu khoa
            string filter = "";

            // che do tim kiem
            if (Regex.IsMatch(txtSearch.Text, @"^[\[]")) // tim kiem mo rong "[{keyword}": title, season, status ...
            {
                searchOption = true; // cho phep tim kiem mo rong
                filter = Regex.Replace(txtSearch.Text, @"^[\[]", "").ToLower(); // loai bo "[" khoi tu khoa
            }
            else // tim kiem thong thuong "{title}"
            {
                filter = txtSearch.Text.ToLower();
            }

            // bat dau tim kiem
            foreach (string anime in listItemAnime) // duyet listItemAnime
            {
                if (searchOption) // che do tim kiem mo rong
                {
                    if (anime.ToLower().IndexOf(filter) != -1) // tim thay
                    {
                        lstAnime.Items.Add(anime); // them vao lstAnime
                    }
                }
                else // che do tim kiem thong thuong
                {
                    if (AnimeShortName(anime.ToLower()).IndexOf(filter) != -1) // tim thay
                    {
                        lstAnime.Items.Add(anime); // them vao lstAnime
                    }
                }
            }

            // hien thi thong tin anime
            loadAnimeInfo();
        }

        private void btScan_Click(object sender, EventArgs e) // khi nhan btScan
        {
            // tao hop thoai chon folder
            FolderBrowserDialog scanFolder = new FolderBrowserDialog();

            // thiet lap vi tri mac dinh khi mo folder

            if (Directory.Exists(recentScan)) // recentScan ton tai
            {
                scanFolder.SelectedPath = recentScan; // chon recentScan
            }
            else // khong ton tai
            {
                scanFolder.SelectedPath = Environment.SpecialFolder.Desktop.ToString(); // chon desktop
            }

            // xu li khi mo folder

            if (scanFolder.ShowDialog() == DialogResult.OK) // nhan OK
            {
                // tao danh sach subfolder co chua file

                string folderDir = scanFolder.SelectedPath; // doc duong dan duoc chon
                recentScan = scanFolder.SelectedPath; // luu duong dan da mo

                List<string> listDir = new List<string>(); // danh sach kiem duyet

                if (Regex.IsMatch(folderDir, @"^[a-zA-Z]:\\$")) // folder chon la drive
                {
                    // tao danh sach subfolder cap 1

                    List<string> subfolderDrive = new List<string>(); // danh sach subfolder cap 1

                    subfolderDrive.AddRange(Directory.GetDirectories(folderDir, "*", SearchOption.TopDirectoryOnly)); // lay danh sach subfolder cap 1
                    subfolderDrive.Remove(folderDir + "System Volume Information"); // bo qua folder "System Volume Information"
                    subfolderDrive.Remove(folderDir + "RECYCLER"); // bo qua folder "RECYCLER"

                    foreach (string subfolder in subfolderDrive) // duyet danh sach subfolder cap 1
                    {
                        listDir.Add(subfolder); // them subfolder cap 1 vao danh sach kiem duyet
                        listDir.AddRange(Directory.GetDirectories(subfolder, "*", SearchOption.AllDirectories)); // them tat ca subfolder cap > 1 vao danh sach kiem duyet
                    }
                }
                else // folder chon khong phai drive
                {
                    listDir.AddRange(Directory.GetDirectories(folderDir, "*", SearchOption.AllDirectories)); // them tat ca subfolder vao danh sach kiem duyet
                    folderDir = folderDir + "\\"; // them "\" vao duong dan da chon
                }

                // loai bo cac subfolder khong chua file ra khoi danh sach

                List<string> removeDir = new List<string>(); // danh sach xoa subfolder rong

                foreach (string subfolder in listDir) // duyet danh sach kiem duyet
                {
                    if (Directory.GetFiles(subfolder, "*.*", SearchOption.TopDirectoryOnly).Length == 0) // subfolder khong co file
                    {
                        removeDir.Add(subfolder); // them subfolder vao danh sach xoa
                    }
                }

                foreach (string subfolder in removeDir) // duyet danh sach xoa subfolder
                {
                    listDir.Remove(subfolder); // xoa subfolder khoi danh sach kiem duyet
                }

                // them anime vao data

                foreach (string subfolder in listDir) // duyet danh sach kiem duyet
                {
                    bool isAdd = true; // them anime

                    // thong tin co ban anime
                    string anime = subfolder.Replace(folderDir, ""); // ten anime
                    anime = anime.Replace("\\", " "); // thay the "\" => " "
                    string animeFolder = subfolder; // duong dan anime

                    // kiem tra anime da co trong data hay chua
                    foreach (string item in lstAnime.Items) // duyet danh sach lstAnime
                    {
                        // kiem tra ten anime
                        if (AnimeShortName(item) == anime) // tim thay
                        {
                            isAdd = false; // khong them anime
                            break; // dung kiem tra ten anime
                        }

                        string dataFile = AnimeDataName(item); // ten data anime

                        // kiem tra duong dan anime
                        if (File.Exists(appDir + "Data\\" + dataFile + ".xml")) // tim thay Anime.xml
                        {
                            string animeLink = ""; // duong dan anime

                            // doc thong tin duong dan anime: Anime.xml => <link>

                            XmlTextReader dataXml = new XmlTextReader(appDir + "Data\\" + dataFile + ".xml"); // mo file Anime.xml

                            while (dataXml.Read()) // doc noi dung file Anime.xml
                            {
                                if (dataXml.NodeType == XmlNodeType.Element) // doc the
                                {
                                    switch (dataXml.Name) // ten the
                                    {
                                        case "link": // doc the <link> => animeLink
                                            dataXml.Read();
                                            animeLink = (dataXml.Value == "\r\n  ") ? Properties.Resources.Link : dataXml.Value;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }

                            dataXml.Close(); // dong file Anime.xml

                            if (animeLink == animeFolder) // duong dan anime da co
                            {
                                isAdd = false; // khong them anime
                                break; // dung kiem tra duong dan anime
                            }
                        }
                    }

                    if (isAdd) // cho phep them anime vao data
                    {
                        // them vao danh sach anime
                        listItemAnime.Add(anime); // them anime vao listItemAnime

                        string dataFile = AnimeDataName(anime); // ten data anime

                        // luu thong tin anime: frmMain => Anime.xml

                        XmlTextWriter dataXml = new XmlTextWriter(appDir + "Data\\" + dataFile + ".xml", Encoding.UTF8); // mo file Anime.xml
                        dataXml.Formatting = Formatting.Indented; // thiet lap dinh dang
                        dataXml.WriteStartDocument(); // bat dau noi dung
                        dataXml.WriteStartElement("anime"); // mo the <anime>

                        // ghi the <title> <= anime
                        dataXml.WriteStartElement("title");
                        dataXml.WriteString(anime);
                        dataXml.WriteEndElement();

                        // ghi the <type> <= null
                        dataXml.WriteStartElement("type");
                        dataXml.WriteString("-1");
                        dataXml.WriteEndElement();

                        // ghi the <index> <= null
                        dataXml.WriteStartElement("index");
                        dataXml.WriteString("");
                        dataXml.WriteEndElement();

                        // ghi the <genre> <= null
                        dataXml.WriteStartElement("genre");
                        dataXml.WriteString("");
                        dataXml.WriteEndElement();

                        // ghi the <episode> <= null
                        dataXml.WriteStartElement("episode");
                        dataXml.WriteString("");
                        dataXml.WriteEndElement();

                        // ghi the <status> <= null
                        dataXml.WriteStartElement("status");
                        dataXml.WriteString("-1");
                        dataXml.WriteEndElement();

                        // ghi the <release> <= null
                        dataXml.WriteStartElement("release");
                        dataXml.WriteString("");
                        dataXml.WriteEndElement();

                        // ghi the <season> <= null
                        dataXml.WriteStartElement("season");
                        dataXml.WriteString("");
                        dataXml.WriteEndElement();

                        // ghi the <info> <= null
                        dataXml.WriteStartElement("info");
                        dataXml.WriteString("");
                        dataXml.WriteEndElement();

                        // ghi the <cover> <= dataFile
                        dataXml.WriteStartElement("cover");
                        dataXml.WriteString(dataFile);
                        dataXml.WriteEndElement();

                        // ghi the <link> <= folder
                        dataXml.WriteStartElement("link");
                        dataXml.WriteString(animeFolder);
                        dataXml.WriteEndElement();

                        dataXml.WriteEndElement(); // dong the <anime>
                        dataXml.WriteEndDocument(); // ket thuc noi dung
                        dataXml.Flush(); // giai phong bo nho
                        dataXml.Close(); // dong file Anime.xml

                        // luu anh bia anime: Resocure => Anime.jpg

                        EncoderParameters encoderParameters = new EncoderParameters(1); // khoi tao tham so
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L); // thiet lap chat luong anh

                        Bitmap image = new Bitmap(Properties.Resources.Empty); // khoi tao anh bia
                        image.Save(appDir + "Data\\" + dataFile + ".jpg", GetEncoder(ImageFormat.Jpeg), encoderParameters); // luu anh bia
                        image.Dispose(); // giai phong bo nho
                    }
                }

                saveListAnime(); // luu danh sach anime
                loadListAnime(); // hien thi danh sach anime
                loadAnimeInfo(); // hien thi thong tin anime
                txtMessage.Text = "Scan folder: success"; // hien thi thong bao
            }
        }

        private void lstAnime_DrawItem(object sender, DrawItemEventArgs e) // hien thi trang thai lstAnime
        {
            if (e.Index < 0) return; // bo qua neu khong co item

            // ve nen
            e.DrawBackground();

            // mau chu
            Brush myBrush = Brushes.Black;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) // item [select] => chu trang
            {
                myBrush = Brushes.White;
            }
            else
            {
                // item [Ongoing] => chu xanh la
                if (Regex.IsMatch(lstAnime.Items[e.Index].ToString(), @"\[Ongoing\]$"))
                {
                    myBrush = Brushes.Green;
                }

                // item [Dropping] => chu do
                if (Regex.IsMatch(lstAnime.Items[e.Index].ToString(), @"\[Dropping\]$"))
                {
                    myBrush = Brushes.Red;
                }

                // item [Complete] => chu xanh bien
                if (Regex.IsMatch(lstAnime.Items[e.Index].ToString(), @"\[Complete\]$"))
                {
                    myBrush = Brushes.Blue;
                }
            }

            // ve item
            e.Graphics.DrawString(AnimeShortName(lstAnime.Items[e.Index].ToString()), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

            // dong khung
            e.DrawFocusRectangle();

            // hien thi thanh cuon ngang

            int width = TextRenderer.MeasureText(AnimeShortName(lstAnime.Items[e.Index].ToString()), lstAnime.Font, lstAnime.ClientSize, TextFormatFlags.NoPrefix).Width + SystemInformation.VerticalScrollBarWidth; // chieu rong item

            if (lstAnime.HorizontalExtent < width) // chieu rong item > extent
            {
                lstAnime.HorizontalExtent = width; // lay chieu rong item lam chuan
            }
        }

        private void lstAnime_SelectedIndexChanged(object sender, EventArgs e) // khi lua chon anime
        {
            loadAnimeInfo(); // hien thi thong tin anime
        }

        private void btAdd_Click(object sender, EventArgs e) // khi nhan btAdd
        {
            // hien thi thong bao
            txtMessage.Text = "Add anime";

            // form danh sach anime

            // btAdd
            btAdd.Enabled = false; // an nut

            if (lstAnime.SelectedIndex == -1) // khong co anime nao duoc lua chon
            {
                // btEdit
                btEdit.Enabled = false; // an nut

                // btRemove
                btRemove.Enabled = false; // an nut
            }
            else
            {
                // btEdit
                btEdit.Enabled = true; // hien nut

                // btRemove
                btRemove.Enabled = true; // hien nut
            }

            // form thong tin anime

            // txturl
            txtUrl.ReadOnly = false; // cho nhep nhap
            txtUrl.Text = Properties.Resources.Url; // chua nhap

            // btGet
            btGet.Enabled = true; // hien nut

            // txtTitle
            txtTitle.ReadOnly = false; // cho phep nhap
            txtTitle.Text = Properties.Resources.Title; // chua nhap

            // cbType
            cbType.Enabled = true; // cho phep chon
            cbType.SelectedIndex = -1; // chua chon lua

            // txtType
            txtType.ReadOnly = true; // khong cho nhap
            txtType.Text = Properties.Resources.Type; // chua nhap

            // txtGenre
            txtGenre.ReadOnly = false; // cho phep nhap
            txtGenre.Text = Properties.Resources.Genre; // chua nhap

            // txtStatus
            txtStatus.ReadOnly = false; // cho phep nhap
            txtStatus.Text = Properties.Resources.Status; // chua nhap

            // cbStatus
            cbStatus.Enabled = true; // cho phep chon
            cbStatus.SelectedIndex = -1; // chua chon

            // txtRelease
            txtRelease.ReadOnly = false; // cho phep nhap
            txtRelease.Text = Properties.Resources.Release; // chua nhap

            // txtSeason
            txtSeason.ReadOnly = false; // cho phep nhap
            txtSeason.Text = Properties.Resources.Season; // chua nhap

            // txtInfo
            txtInfo.ReadOnly = false; // cho phep nhap
            txtInfo.Text = Properties.Resources.Info; // chua nhap

            // imgCover
            imgCover.Image = Properties.Resources.Empty; // xoa anh bia

            // txtCover
            txtCover.Text = Properties.Resources.Cover; // xoa duong dan

            // btOpenImg
            btOpenImg.Enabled = true; // hien nut

            // lstEpisode
            lstEpisode.Items.Clear(); // xoa danh sach

            // txtLink
            txtLink.Text = Properties.Resources.Link; // xoa duong dan

            // txtEpisode
            txtEpisode.Text = Properties.Resources.Episode; // xoa so luong tap tin

            // btOpenDir
            btOpenDir.Enabled = true; // hien nut

            // btSave
            btSave.Enabled = true; // hien nut
        }

        private void btEdit_Click(object sender, EventArgs e) // khi nhan btEdit
        {
            if (lstAnime.SelectedIndex == -1) // chua chon anime
            {
                txtMessage.Text = "No anime were selected!"; // hien thi thong bao
            }
            else // da chon anime
            {
                loadAnimeInfo(); // hien thi thong tin anime

                string anime = lstAnime.SelectedItem.ToString(); // ten anime

                txtMessage.Text = "Edit anime: " + AnimeShortName(anime); // hien thi thong bao

                // form danh sach anime

                // btEdit
                btEdit.Enabled = false; // an nut

                // form thong tin anime

                // txtUrl
                txtUrl.ReadOnly = false; // cho phep nhap
                txtUrl.Text = Properties.Resources.Url; // chua nhap

                // btGet
                btGet.Enabled = true; // hien nut

                // txtTitle
                txtTitle.ReadOnly = false; // cho phep nhap

                // cbType
                cbType.Enabled = true; // cho phep chon

                if (cbType.SelectedIndex != -1) // cbType da chon
                {
                    if (cbType.SelectedItem.ToString() == "TV Series") // cbType == "TV Series"
                    {
                        txtType.ReadOnly = true; // khong cho nhap
                    }
                    else
                    {
                        txtType.ReadOnly = false; // cho phep nhap
                    }
                }

                // txtGenre
                txtGenre.ReadOnly = false; // cho phep nhap

                // txtStatus
                txtStatus.ReadOnly = false; // cho phep nhap

                // cbStatus
                cbStatus.Enabled = true; // cho phep chon

                // txtRelease
                txtRelease.ReadOnly = false; // cho phep nhap

                // txtSeason
                txtSeason.ReadOnly = false; // cho phep nhap

                // txtInfo
                txtInfo.ReadOnly = false; // cho phep nhap

                // btOpenImg
                btOpenImg.Enabled = true; // hien nut

                // btOpenDir
                btOpenDir.Enabled = true; // hien nut

                // btSave
                btSave.Enabled = true; // hien nut
            }
        }

        private void btRemove_Click(object sender, EventArgs e) // khi nhan btRemove
        {
            if (lstAnime.SelectedIndex == -1) // chua chon anime
            {
                txtMessage.Text = "No anime were selected."; // hien thi thong bao
            }
            else // da chon anime
            {
                string anime = lstAnime.SelectedItem.ToString(); // ten anime

                // tao hop thoai xoa anime

                DialogResult dialogResult = MessageBox.Show("This anime " + AnimeShortName(anime) + " will be deleted from the database", "Remove anime", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes) // dong y xoa
                {
                    txtMessage.Text = "Remove Anime: " + AnimeShortName(anime); // hien thi thong bao

                    if (lstAnime.SelectedIndex > 0) // anime dang chon khong phai anime dau tien
                    {
                        recentAnime = lstAnime.Items[lstAnime.SelectedIndex - 1].ToString(); // nho anime truoc anime dang chon
                    }
                    else // anime dau tien
                    {
                        recentAnime = ""; // dat lai anime nho
                    }
                    removeAnimeInfo(); // xoa thong tin anime
                    loadListAnime(); // hien thi danh sach anime
                    loadAnimeInfo(); // hien thi thong tin anime
                }
            }
        }

        private void btGet_Click(object sender, EventArgs e) // khi nhan btGet
        {
            // kiem tra txtUrl
            if (txtUrl.Text == "") // txtUrl chua nhap
            {
                txtMessage.Text = "Get URL: invalid"; // hien thi thong bao
            }
            else // txtUrl da nhap
            {
                if (!Regex.IsMatch(txtUrl.Text, @"^https://myanimelist.net/anime/[0-9]{1,}/(.*)")) // dinh dang txtUrl khong hop le
                {
                    txtMessage.Text = "Get URL: not support format"; // hien thi thong bao
                }
                else // dinh dang txtUrl hop le
                {
                    string data = new StreamReader(StreamFrom(txtUrl.Text)).ReadToEnd(); // lay du lieu

                    // chuan hoa du lieu
                    data = Regex.Replace(data, @"\t", ""); // xoa tab
                    data = Regex.Replace(data, @"\r", ""); // xoa return
                    data = Regex.Replace(data, @"\n", " "); // xoa xuong dong
                    data = Regex.Replace(data, @"[ ]{1,}", " "); // xoa " " trung lap

                    // tach txtTitle
                    string title = txtUrl.Text; // lay url
                    title = Uri.UnescapeDataString(title); // giai ma url
                    title = Regex.Replace(title, @"^https://myanimelist.net/anime/[0-9]{1,}/", ""); // xoa phan dau url
                    title = Regex.Replace(title, @"\?q=(.*)", ""); // xoa phan method get
                    title = Regex.Replace(title, @"[^a-zA-Z0-9 \-_+\[\]\(\)!.,]{1,}", " "); // [a-z], [A-Z], [0-9], "[]()+-_!., "
                    title = Regex.Replace(title, "-", " "); // thay "-" => " "
                    title = Regex.Replace(title, "_", " "); // thay "_" => " "
                    title = Regex.Replace(title, @"[ ]{1,}", " "); // xoa " " trung lap
                    title = Regex.Replace(title, @"^[ ]{1,}", ""); // xoa " " dau chuoi
                    title = Regex.Replace(title, @"[ ]{1,}$", ""); // xoa " " cuoi chuoi
                    title = Regex.Replace(title, @"^[a-z]", m => m.ToString().ToUpper()); // viet hoa chua dau tien
                    txtTitle.Text = title; // dien txtTitle

                    // tach imgCover
                    string cover = Regex.Match(data, @"https://cdn\.myanimelist\.net/images/anime/[0-9]{1,}/[0-9]{1,}.jpg").Value; // tach link anh bia
                    imgCover.Image = ResizeImage(Image.FromStream(StreamFrom(cover)), 225, 334); // gui hinh anh vao imgCover

                    // tach cbType
                    string type = StringFrom(data, "<div( class=\"spaceit_pad\")?> <span class=\"dark_text\">Type:</span>", "</div>"); // tach thong tin cbType

                    switch (type) // chon cbType
                    {
                        case "TV": // TV => TV Series
                            cbType.SelectedIndex = 0;
                            break;

                        case "Movie": // Movie => Movies
                            cbType.SelectedIndex = 2;
                            break;

                        case "Special": // Special => Specicals
                            cbType.SelectedIndex = 3;
                            break;

                        case "OVA": // OVA => OVA
                            cbType.SelectedIndex = 4;
                            break;

                        case "ONA": // ONA => ONA
                            cbType.SelectedIndex = 5;
                            break;

                        default:
                            break;
                    }

                    // tach txtStatus
                    string status = StringFrom(data, "<div( class=\"spaceit_pad\")?> <span class=\"dark_text\">Episodes:</span>", "</div>"); // tach thong tin txtStatus

                    if (Regex.IsMatch(status, @"^[0-9]{1,}$")) // txtStatus la so
                    {
                        status = status + "/" + status + " EP"; // dinh dang [NUM]/[NUM] EP
                    }

                    if (Regex.IsMatch(status, @"^[\?]{1,}$")) // txtStatus la "?[?]"
                    {
                        status = Regex.Replace(status, @"^[\?]{1,}$", "??? EP"); // thay the "?[?]" => "??? EP"
                    }

                    txtStatus.Text = status; // dien txtStatus

                    // tach cbStatus
                    string progress = StringFrom(data, "<div( class=\"spaceit_pad\")?> <span class=\"dark_text\">Status:</span>", "</div>"); // tach thong tin cbStatus

                    switch (progress) // chon cbStatus
                    {
                        case "Currently Airing": // "Currently Airing" => Ongoing
                            cbStatus.SelectedIndex = 0;
                            break;

                        case "Finished Airing": // "Finished Airing" => Complete
                            cbStatus.SelectedIndex = 2;
                            break;

                        default:
                            break;
                    }

                    // tach txtRelease
                    string release = StringFrom(data, "<div( class=\"spaceit_pad\")?> <span class=\"dark_text\">Aired:</span>", "</div>"); // tach thong tin txtRelease
                    release = Regex.Replace(release, @"[\?]{1,}", "???"); // thay the "?[?]" => "???"
                    txtRelease.Text = release; // dien txtRelease

                    // tach txtSeason
                    txtSeason.Text = StringFrom(data, "<div( class=\"spaceit_pad\")?> <span class=\"dark_text\">Premiered:</span>", "</div>"); // dien txtSeason

                    // tach txtGenre
                    txtGenre.Text = StringFrom(data, "<div( class=\"spaceit_pad\")?> <span class=\"dark_text\">Genres:</span>", "</div>"); // dien txtGenre

                    // tach TxtInfo
                    string info = StringFrom(data, "Edit</a></div><h2>Synopsis</h2></div><p itemprop=\"description\">", "</p>"); // tach thong tin txtInfo
                    info = Regex.Replace(info, @"\&[a-zA-Z0-9#]{1,}\;", ""); // xoa ki tu html
                    info = Regex.Replace(info, @"[\ ]((\[Written\ by(.*)\])|(\(Source\:(.*)\)))$", ""); // xoa thong tin ghi chu
                    txtInfo.Text = info; // dien txtInfo

                    // chuyen doi thong tin txtRelease sang txtSeason
                    if (txtRelease.Text != "" && txtSeason.Text == "") // txtRelease da nhap ma txtSeason chua nhap
                    {
                        txtSeason.Text = SeasonFrom(txtRelease.Text); // chuyen doi txtRelease sang txtSeason
                    }

                    // txtUrl
                    txtUrl.Text = Properties.Resources.Url; // xoa url

                    // hien thi thong bao
                    txtMessage.Text = "Get Info: success";
                }
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e) // khi lua chon cbType
        {
            if (btSave.Enabled == true) // btSave hien nut
            {
                // txtType
                txtType.Text = Properties.Resources.Type; // chua nhap

                // cbType
                if (cbType.SelectedIndex == -1) // cbType chua chon
                {
                    txtType.ReadOnly = true; // khong cho nhap
                }
                else
                {
                    if (cbType.SelectedItem.ToString() == "TV Series") // cbType = "TV Series"
                    {
                        txtType.ReadOnly = true; // khong cho nhap
                    }
                    else
                    {
                        txtType.ReadOnly = false; // cho phep nhap
                    }
                }
            }
        }

        private void txtRelease_Leave(object sender, EventArgs e) // khi thoat khoi txtRelease
        {
            if (txtRelease.Text != "" && txtSeason.Text == "") // txtRelease da nhap ma txtSeason chua nhap
            {
                txtSeason.Text = SeasonFrom(txtRelease.Text); // chuyen doi thong tin txtRelease sang txtSeason
            }
        }

        private void btOpenImg_Click(object sender, EventArgs e) // khi nhan btOpenImg
        {
            OpenFileDialog fileImage = new OpenFileDialog(); // tao hop thoai mo file
            fileImage.Filter = "Image File|*.jpg"; // chi mo file jpg

            // thiet lap vi tri mac dinh khi mo folder

            if (Directory.Exists(recentImage)) // recentImage ton tai
            {
                fileImage.InitialDirectory = recentImage; // chon recentImage
            }
            else // khong ton tai
            {
                fileImage.InitialDirectory = Environment.SpecialFolder.Desktop.ToString(); // chon desktop
            }

            // xu li khi mo file

            if (fileImage.ShowDialog() == DialogResult.OK) // nhan OK
            {
                txtCover.Text = fileImage.FileName; // gui duong dan vao txtCover
                recentImage = fileImage.InitialDirectory; // luu folder da mo file
                FileStream fileStream = new FileStream(txtCover.Text, FileMode.Open); // mo file Anime.jpg
                Image image = Image.FromStream(fileStream); // doc file Anime.jpg
                fileStream.Close(); // dong file Anime.jpg
                imgCover.Image = ResizeImage(image, 225, 334); // thay doi kich thuoc phu hop, gui vao imgCover
            }
        }

        private void btOpenDir_Click(object sender, EventArgs e) // khi nhan btOpenDir
        {
            FolderBrowserDialog folderDir = new FolderBrowserDialog(); // tao hop thoai chon folder

            // thiet lap vi tri mac dinh khi mo folder

            if (Directory.Exists(txtLink.Text)) // txtLink ton tai
            {
                folderDir.SelectedPath = txtLink.Text; // chon txtLink
            }
            else // khong ton tai
            {
                if (Directory.Exists(recentFolder)) // recentFolder ton tai
                {
                    folderDir.SelectedPath = recentFolder; // chon recentFolder
                }
                else // khong ton tai
                {
                    folderDir.SelectedPath = Environment.SpecialFolder.Desktop.ToString(); // chon desktop
                }
            }

            // xu li khi mo folder

            if (folderDir.ShowDialog() == DialogResult.OK) // nhan OK
            {
                txtLink.Text = folderDir.SelectedPath; // gui duong dan vao txtLink
                recentFolder = folderDir.SelectedPath; // luu duong dan folder
                loadListEpisode(); // hien thi danh sach tap phim
            }
        }

        private void lstEpisode_SelectedIndexChanged(object sender, EventArgs e) // khi lua chon lstEpisode
        {
            if (lstEpisode.SelectedIndex != -1) // da chon tap phim
            {
                // cuon thanh cuon len vi tri duoc chon
                if (lstEpisode.SelectedIndex < 6) // tap phim duoc chon: item < 6
                {
                    lstEpisode.TopIndex = 0; // cuon thanh cuon len dau tien
                }
                else // tap phim duoc chon: 6 =< item < end
                {
                    if (lstEpisode.SelectedIndex < lstEpisode.Items.Count - 6) // tap phim duoc chon: 6 =< item < end - 6
                    {
                        lstEpisode.TopIndex = lstEpisode.SelectedIndex - 5; // cuon thanh cuon chinh giua
                    }
                    else // tap phim duoc chon: item > end - 6
                    {
                        lstEpisode.TopIndex = lstEpisode.Items.Count - 6; // cuon thanh cuon xuong cuoi cung
                    }
                }
            }
        }

        private void lstEpisode_MouseDoubleClick(object sender, MouseEventArgs e) // khi kich dup lstEpisode
        {
            if (!btSave.Enabled) // khong chinh sua
            {
                if (lstEpisode.SelectedIndex != -1) // da chon tap phim
                {
                    // mo tap phim dang chon
                    string epFile = txtLink.Text + "\\" + lstEpisode.SelectedItem.ToString(); // ten tap phim
                    Process.Start(epFile); // mo tap phim
                }
            }
        }

        private void btSave_Click(object sender, EventArgs e) // khi nhan btSave
        {
            saveAnimeInfo(); // luu thong tin anime
        }
    }
}
