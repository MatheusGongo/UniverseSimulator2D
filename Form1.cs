using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        SimulatorUniverse universe = new SimulatorUniverse();
        List<Body> bodies = new List<Body>();


        public Form1()
        {
            InitializeComponent();
            this.Paint += Form1_Load;
        }

        async private void Form1_Load(object sender, EventArgs e)
        {

            bodies = universe.ReadBodies();
            List<string> output = new List<string>();
            Graphics graph = this.CreateGraphics();
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen pen = new Pen(Color.Green, 10);

            if (bodies.Count > 1)
            {
                for (int it = 0; it < 10000; it++)
                {
                    for (int i = 0; i < bodies.Count; ++i)
                    {
                        for (var j = i + 1; j < bodies.Count; ++j)
                        {
                            universe.GravitationalForceBodies(bodies[i], bodies[j]);
                        }

                        RectangleF rect = new RectangleF(new PointF((float)bodies[i].getPosX(), (float)bodies[i].getPosY()), new Size(Convert.ToInt32(bodies[i].getRadius()), (int)bodies[i].getRadius()));
                        graph.FillEllipse(Brushes.Yellow, rect);
                        graph.DrawEllipse(pen, rect);
                    }
                    universe.InteractionForceBodies(bodies);
                    output = universe.WriteIterationBodies(output, bodies);
                    universe.InteractionColisionsBodies(bodies);
                    universe.ForcesResets(bodies);
                    await Task.Delay(1000);

                    graph.Clear(SystemColors.Control);
                }
            }
        }

    }
}
