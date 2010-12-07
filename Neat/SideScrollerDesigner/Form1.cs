using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Neat.Mathematics;
using Microsoft.Xna.Framework;
namespace SideScrollerDesigner
{

    public partial class Form1 : Form
    {
        #region Fields
        Vector size;
        string filename = "untitled.rbw";
        Bitmap bitmap;
        Graphics graphics;
        Pen SelectPen;
        Pen NormalPen;
        Pen HoverPen;
        Font NormalFont;
        Brush NormalBrush;
        System.Drawing.Point MousePos;
        bool lastMouseLeft, lastMouseRight, lastMouseMiddle;
        bool currentMouseLeft, currentMouseRight, currentMouseMiddle;
        bool savePropertiesState = false;
        public ObjectList objects;
        Vector point1 = new Vector();
        GameObject currentObject;
        bool mouseHover = false;
        GameObject hoverObject;
        #endregion

        #region Code
        void Initialize()
        {
            SelectPen = new Pen(System.Drawing.Color.Red, 1);
            NormalPen = new Pen(System.Drawing.Color.Black, 2);
            HoverPen = new Pen(System.Drawing.Color.Blue, 2);
            NormalFont = new System.Drawing.Font("Segoe UI", 9, FontStyle.Regular, GraphicsUnit.Point);
            NormalBrush = Brushes.Black;
            size = new Vector();
            size.X = 640;
            size.Y = 480;
            NewWorld();
        }

        void NewWorld()
        {
            objects = new ObjectList();
            CreateBitmap();
            LoadProperties();
        }

        void NewObject()
        {
            for (int i = 0; i < objects.Count; i++)
                if (objects[i].body.Mesh.Vertices.Count < 3) objects.RemoveAt(i);
            currentObject = new GameObject();
            currentObject.name = "undefined";
            objects.Add(currentObject);
            RefreshObjectsList();
        }

        void Compile()
        {
            try
            {
                saveFileDialog1.ShowDialog();
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false);
                sw.WriteLine("rem RedBulb World Maker Auto Generated Code");
                foreach (var item in objects)
                {
                    sw.WriteLine("\n:obj_" + item.name.Trim());
                    sw.WriteLine("e_create " + item.name.Trim());
                    sw.WriteLine("e_select " + item.name.Trim());
                    sw.WriteLine("e_texture " + item.texture.Trim());
                    sw.Write("e_reshape ");
                    foreach (Vector2 vertex in item.body.Mesh.Vertices)
                        sw.Write(vertex.X + "," + vertex.Y + " ");
                    sw.WriteLine();
                    sw.WriteLine("e_mass " + item.body.Mass.ToString());
                    sw.WriteLine("e_speed " + item.speed.ToString());
                    sw.WriteLine("e_free " + item.body.IsFree.ToString());
                    sw.WriteLine("e_pushable " + item.body.Pushable.ToString());
                    sw.WriteLine("e_fallable " + item.body.AttachToGravity.ToString());
                    sw.WriteLine("e_addtosim");
                }
                sw.Close();
            }
            catch
            {
                MessageBox.Show("Error saving file.");
            }
        }

        void SaveMap()
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename, false);
                foreach (var item in objects)
                {
                    sw.WriteLine("~object");
                    sw.WriteLine(item.name);
                    sw.WriteLine(item.texture);
                    foreach (Vector2 vertex in item.body.Mesh.Vertices)
                        sw.Write(vertex.X + "," + vertex.Y + " ");
                    sw.WriteLine("");
                    sw.WriteLine(item.speed.ToString());
                    sw.WriteLine(item.body.IsFree.ToString());
                    sw.WriteLine(item.body.Pushable.ToString());
                    sw.WriteLine(item.body.AttachToGravity.ToString());
                    sw.WriteLine(item.body.Mass.ToString());
                }
                sw.WriteLine("~script");
                sw.Write(textBox1.Text);
                sw.Close();
            }
            catch
            {
                MessageBox.Show("Error in save");
            }
        }

        void LoadMap()
        {
            NewWorld();
            try
            {
                StreamReader sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    string data = sr.ReadLine().Trim();
                    if (data == "~object")
                    {
                        currentObject = new GameObject();
                        currentObject.name = sr.ReadLine().Trim();
                        currentObject.texture = sr.ReadLine().Trim();
                        Polygon p = new Polygon();
                        var vertexdata = sr.ReadLine().Trim();
                        string vs = "";
                        for (int i = 0; i < vertexdata.Length; i++)
                        {
                            if (vertexdata[i] == ' ')
                            {
                                p.AddVertex(Neat.Mathematics.GeometryHelper.String2Vector(vs));
                                vs = "";
                            }
                            else vs += vertexdata[i];
                        }
                        currentObject.speed = Vector.FromString(sr.ReadLine());
                        currentObject.body.IsFree = bool.Parse(sr.ReadLine());
                        currentObject.body.Pushable = bool.Parse(sr.ReadLine());
                        currentObject.body.AttachToGravity = bool.Parse(sr.ReadLine());
                        currentObject.body.Mass = float.Parse(sr.ReadLine());
                        objects.Add(currentObject);
                    }
                    else if (data == "~script")
                    {
                        textBox1.Text = sr.ReadToEnd();
                    }
                    else
                    {
                        MessageBox.Show("File is damaged.");
                        return;
                    }
                }
                sr.Close();
                RefreshObjectsList();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        
        void DeleteObject()
        {
            if (objects.Count <= 0) return;
            objects.Remove(currentObject);
            if (objects.Count > 0) currentObject = objects[objects.Count - 1];
            RefreshObjectsList();
        }

        void RefreshObjectsList()
        {
            listObjects.Items.Clear();
            foreach (var item in objects)
            {
                listObjects.Items.Add(item.name);
            }
            Render();
        }

        void CreateBitmap()
        {
            bitmap = new Bitmap(size.X, size.Y, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(bitmap);
            pictureBox1.Image = bitmap;
            Render();
        }

        void Render()
        {
            graphics.Clear(System.Drawing.Color.White);
            if (objects.Count > 0)
            {
                foreach (var item in objects)
                {
                    DrawObject(item, NormalPen);
                }
                DrawObject(currentObject, SelectPen);
                if (mouseHover)
                {
                    DrawObject(hoverObject, HoverPen);
                }
            }
            graphics.Flush();
            pictureBox1.Refresh();
            UpdateStatusBar();
        }

        void DrawObject(GameObject o, Pen pen)
        {
            var vs = o.body.Mesh.Vertices;
            for (int p = vs.Count - 1, q = 0; q < vs.Count; p = q++)
            {
                graphics.DrawLine(pen, vs[p].X, vs[p].Y, vs[q].X, vs[q].Y);
                graphics.DrawString(p.ToString(), NormalFont, NormalBrush, vs[p].X, vs[p].Y);

            }
            var pos = o.body.Mesh.GetPosition();
            graphics.DrawString(
                o.name+"\nTexture:"+o.texture,
            NormalFont, NormalBrush, pos.X, pos.Y);
        }

        void UpdateStatusBar()
        {
            System.Drawing.Point p = MousePos;
            MousePositionLabel.Text = p.X.ToString() + "," + p.Y.ToString();
            FilenameLabel.Text = filename;
        }

        void HandleMouse(MouseEventArgs e)
        {
            MousePos = e.Location;
            currentMouseLeft = (e.Button == System.Windows.Forms.MouseButtons.Left);
            currentMouseMiddle = (e.Button == System.Windows.Forms.MouseButtons.Middle);
            currentMouseRight = (e.Button == System.Windows.Forms.MouseButtons.Right);

            mouseHover = false;
            //if (objects.Count > 0)
            {
                if (currentMouseLeft && !lastMouseLeft)
                {
                    if (currentObject == null || objects.Count == 0)
                    {
                        currentObject = new GameObject();
                        objects.Add(currentObject);
                    }
                    currentObject.body.Mesh.AddVertex(MousePos.X, MousePos.Y);
                    LoadProperties();
                }
                if (currentMouseLeft)
                {
                    LoadProperties();
                }

                for (int i = objects.Count - 1; i >= 0; i--)
                {
                    var item = objects[ i ];
                    if (item != currentObject)
                    {
                        if (item.body.Mesh.Triangles == null) item.body.Mesh.Triangulate();
                        if (item.body.Mesh.IsInside(new Vector2(MousePos.X, MousePos.Y)))
                        {
                            mouseHover = true;
                            hoverObject = item;
                            break;
                        }
                    }
                }

                if (currentMouseRight && !lastMouseRight && mouseHover)
                {
                    mouseHover = false;
                    currentObject = hoverObject;
                    LoadProperties();
                }

                if (currentMouseMiddle)
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i].body.Mesh.Vertices.Count < 3) objects.RemoveAt(i);
                    }
                    currentObject.body.Mesh.AutoTriangulate = true;
                    currentObject.body.Mesh.Triangulate();
                    currentObject = new GameObject();
                    objects.Add(currentObject);
                }
            }
            Render();

            lastMouseLeft = currentMouseLeft;
            lastMouseRight = currentMouseRight;
            lastMouseMiddle = currentMouseMiddle;
        }

        void SaveProperties()
        {
            if (savePropertiesState) return;
            currentObject.name = textName.Text.Trim();
            currentObject.texture = textTexture.Text.Trim();
            try
            {
                currentObject.body.Mass = float.Parse(textMass.Text.Trim());
            }
            catch
            {
                currentObject.body.Mass = 0f;
            }
            currentObject.speed = Vector.FromString(textSpeed.Text.Trim());
            currentObject.body.IsFree = checkFree.Checked;
            currentObject.body.Pushable= checkPushable.Checked;
            currentObject.body.AttachToGravity= checkFallable.Checked;
            //currentObject.background = checkBackground.Checked;
            Render();
        }

        void LoadProperties()
        {
            try
            {
                savePropertiesState = true;
                textName.Text = currentObject.name;
                textTexture.Text = currentObject.texture;
                //textPosition.Text = currentObject.position.ToString();
                //textSize.Text = currentObject.size.ToString();
                textMass.Text = currentObject.body.Mass.ToString();
                textSpeed.Text = currentObject.speed.ToString();
                checkFree.Checked = currentObject.body.IsFree;
                checkPushable.Checked = currentObject.body.Pushable;
                checkFallable.Checked = currentObject.body.AttachToGravity;
                //checkBackground.Checked = currentObject.background;
                savePropertiesState = false;
                RefreshObjectsList();
            }
            catch
            {
                currentObject = new GameObject();
            }
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        #region PictureBox1 Events
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            HandleMouse(e);
        }

        private void pictureBox1_Move(object sender, EventArgs e)
        {
            Render();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            HandleMouse(e);
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            Render();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            HandleMouse(e);
        }
        #endregion

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            
        }
        #region Properties
        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            Vector v = Vector.FromString(toolStripTextBox1.Text);
            size = v;
            if (v.X <= 0 || v.Y <= 0) return;
            CreateBitmap();
        }

        private void textName_TextChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void textTexture_TextChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void textPosition_TextChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void textSize_TextChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void textMass_TextChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void textSpeed_TextChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void checkFree_CheckedChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void checkPushable_CheckedChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void checkFallable_CheckedChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }

        private void checkBackground_CheckedChanged(object sender, EventArgs e)
        {
            SaveProperties();
        }
#endregion

        private void newObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewObject();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteObject();
        }

        private void listObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentObject = objects[listObjects.SelectedIndex];
            LoadProperties();
            Render();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            objects.MoveUp(objects.Search(currentObject.name));
            LoadProperties();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            objects.MoveDown(objects.Search(currentObject.name));
            LoadProperties();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewWorld();
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt ="rbw";
            saveFileDialog1.ShowDialog();
            filename = saveFileDialog1.FileName;
            SaveMap();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "rbw";
            openFileDialog1.ShowDialog();
            filename = openFileDialog1.FileName;
            LoadMap();
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Compile();
        }

        private void tabControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show("");
        }
    }
    #region Other Classes
        public class Vector
        {
            public int X;
            public int Y;
            public Vector()
            {
                X = 0;
                Y = 0;
            }
            public Vector(int _X, int _Y)
            {
                X = _X;
                Y = _Y;
            }
            new public string ToString()
            {
                return X.ToString() + "," + Y.ToString();
            }

            public static Vector FromVector(Vector v)
            {
                Vector result = new Vector();
                result.X = v.X;
                result.Y = v.Y;
                return result;
            }

            public static Vector FromString(string s)
            {
                Vector result = new Vector();
                try
                {
                    string number = "";
                    char d = '0';
                    int i = 0;
                    for (; d != ',' && i < s.Length; i++)
                    {
                        d = s[i];
                        if (d != ',') number += d;
                    }
                    result.X = int.Parse(number.Trim());

                    number = "";
                    for (; i < s.Length; i++) number += s[i];
                    result.Y = int.Parse(number);
                }
                catch
                {
                }
                return result;
            }
        }

        public class GameObject
        {
            public string name = "undefined";
            public string texture = "undefined";
            public Body body = new Body();
            public Vector speed = new Vector(10,10);

            public GameObject()
            {
                body.Mesh = new Polygon();
            }
        }

        public class ObjectList : List<GameObject>
        {
            public int Search(string name)
            {
                int result = -1;
                for (int i = 0; i < Count; i++)
                {
                    var item = this[i];
                    if (item.name == name)
                    {
                        result = i;
                        break;
                    }
                }
                return result;
            }

            public int MoveUp(int i)
            {
                if (Count <= i || Count < 2 || i == 0) return i;
                var temp = this[i - 1];
                this[i - 1] = this[i];
                this[i] = temp;
                return i - 1;
            }

            public int MoveDown(int i)
            {
                if (Count <= i || Count < 2 || i == Count - 1) return i;
                var temp = this[i + 1];
                this[i + 1] = this[i];
                this[i] = temp;
                return i + 1;
            }
        }
        #endregion
}
