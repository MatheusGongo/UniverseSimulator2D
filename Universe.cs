using System;
using System.Collections.Generic;
using System.Text;


abstract class Universe
{
    public abstract void RenderColision(Body bodyA, Body bodyB);
    public abstract void GravitationalForceBodies(Body bodyA, Body bodyB);
    public abstract void ForceCalculate(Body body);
    public abstract void ForcesResets(List<Body> bodies);
    public abstract void InteractionForceBodies(List<Body> bodies);
    public abstract void InteractionColisionsBodies(List<Body> bodies);
    public abstract void SaveOutputFile(List<string> filename);
    public abstract void RenderUniverse();

}