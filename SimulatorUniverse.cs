using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class SimulatorUniverse : Universe
{
    double G = 6.6740184 * Math.Pow(10, -11);

    private int nBodies;
    private int nIterations;
    private int time;

    public SimulatorUniverse() { }

    public List<Body> ReadBodies()
    {
        List<Body> celestialBodies = new List<Body>();
        string file = "bodies.txt";

        if (!File.Exists(file))
        {
            Console.WriteLine(String.Format("Arquivo {0} não existe", file));
            return celestialBodies;
        }

        string[] lines = File.ReadAllLines(file);
        int counter = 0;

        foreach (var line in lines)
        {
            if (counter == 0)
            {
                string[] data = line.Split(";");

                nBodies = int.Parse(data[0]);
                nIterations = int.Parse(data[1]);
                time = int.Parse(data[1]);
            }

            if (counter > 0 && counter <= nBodies)
            {
                Body newCelestialBody = new Body(line);
                celestialBodies.Add(newCelestialBody);
            }

            counter++;
        }

        return celestialBodies;
    }

    public override void SaveOutputFile(List<string> output)
    {
        string file = "output.txt";

        FileStream myFile = new FileStream(file, FileMode.Open, FileAccess.Write);
        StreamWriter sw = new StreamWriter(myFile, Encoding.UTF8);

        foreach (var item in output)
        {
            sw.WriteLine(item);
        }

        sw.Close();
        myFile.Close();

    }




    public override void GravitationalForceBodies(Body bodyA, Body bodyB)
    {

        double r = CalculateDistance(bodyA, bodyB);
        double F = (G * bodyA.getMass() * bodyB.getMass()) / Math.Pow(r, 2);

        double rx = (bodyA.getPosX() - bodyB.getPosX());
        double ry = (bodyA.getPosY() - bodyB.getPosY());


        double Fx = F * (rx / r);
        double Fy = F * (ry / r);

        bodyA.setF(bodyA.getF() + (F * (-1)));
        bodyA.setFx(bodyA.getFx() + (Fx * (-1)));
        bodyA.setFy(bodyA.getFy() + (Fy * (-1)));

        bodyB.setF(bodyB.getF() + (F * (1)));
        bodyB.setFx(bodyB.getFx() + (Fx * (1)));
        bodyB.setFy(bodyB.getFy() + (Fy * (1)));
    }


    public override void RenderColision(Body bodyA, Body bodyB)
    {
        double distance = CalculateDistance(bodyA, bodyB);

        if (distance < (bodyA.getRadius() + bodyB.getRadius()))
        {
            double body_1_Vx = bodyA.getVelX();
            double body_1_Vy = bodyA.getVelY();
            double body_2_Vx = bodyA.getVelX();
            double body_2_Vy = bodyA.getVelY();

            // Realiza as colisões entre os bodies A e B
            bodyA.setVelX(body_2_Vx * (-1));
            bodyA.setVelY(body_2_Vy * (-1));
            bodyB.setVelX(body_1_Vx * (-1));
            bodyB.setVelY(body_1_Vy * (-1));

            ForceCalculate(bodyA);
            ForceCalculate(bodyB);
        }
    }


    public override void ForceCalculate(Body body)
    {
        double Ax = body.getFx() / body.getMass();
        double Ay = body.getFy() / body.getMass();
        double Vx = body.getVelX() + (Ax * time);
        double Vy = body.getVelY() + (Ay * time);
        double Sx = body.getPosX() + (body.getVelX() * time) + ((Ax / 2) * Math.Pow(time, 2));
        double Sy = body.getPosY() + (body.getVelY() * time) + ((Ay / 2) * Math.Pow(time, 2));

        // Seta a posição X,Y e Velociade em X,Y do corpo
        body.setPosX(Sx);
        body.setPosY(Sy);
        body.setVelX(Vx);
        body.setVelY(Vy);
    }


    public override void ForcesResets(List<Body> bodies)
    {
        foreach (var body in bodies)
        {
            body.setF(0.0f);
            body.setFx(0.0f);
            body.setFy(0.0f);
        }
    }


    public override void InteractionForceBodies(List<Body> bodies)
    {
        foreach (var body in bodies)
        {
            ForceCalculate(body);
        }
    }

    public override void InteractionColisionsBodies(List<Body> bodies)
    {

        for (var i = 0; i < bodies.Count; ++i)
        {
            for (var j = i + 1; j < bodies.Count; ++j)
            {
                RenderColision(bodies[i], bodies[j]);
            }
        }
    }


    public List<string> WriteIterationBodies(List<string> output, List<Body> bodies)
    {
        foreach (var body in bodies)
        {
            output.Add(body.formatOutputFile());
        }

        return output;
    }


    public double CalculateDistance(Body bodyA, Body bodyB)
    {
        return Math.Sqrt(Math.Pow((bodyA.getPosX() - bodyB.getPosX()), 2) + Math.Pow((bodyA.getPosY() - bodyB.getPosY()), 2));
    }


    public override void RenderUniverse()
    {
        List<Body> celestialBodies = ReadBodies();
        List<string> output = new List<string>();

        output.Add(String.Format("{0};{1}", celestialBodies.Count, nIterations));

        if (celestialBodies.Count > 1)
        {
            for (int iteration = 0; iteration < nIterations; iteration++)
            {
                output.Add(String.Format("** Interacao {0} ************", iteration));

                for (var i = 0; i < celestialBodies.Count; ++i)
                {
                    for (var j = i + 1; j < celestialBodies.Count; ++j)
                    {
                        GravitationalForceBodies(celestialBodies[i], celestialBodies[j]);
                    }
                }

                InteractionForceBodies(celestialBodies);
                output = WriteIterationBodies(output, celestialBodies);
                InteractionColisionsBodies(celestialBodies);
                ForcesResets(celestialBodies);

            }

            SaveOutputFile(output);
        }
    }
}