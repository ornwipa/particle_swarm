using System;

namespace PSO_csharp
{
    class Program
    {
        // Random object used as generator
        static Random ran = null; 

        static double ObjectiveFunction(double[] x) // Rastrigin function
        {
            double result = 0.0;
            for (int i = 0; i < x.Length; ++i)
            {
                result += x[i]*x[i] - 10*Math.Cos(2*Math.PI*x[i]) + 10;
            }
            return result;
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("\nBegin Particle Swarm Optimization Demo\n");
                
                // Instantiate the Random object with an arbitrary seed value of 0
                ran = new Random(0);
                
                // Initialize key PSO variables
                int numberParticles = 10;
                int Dim = 2; // dimensions
                double minX = -100.0;
                double maxX = 100.0;

                // Create an array of Particle objects
                Particle[] swarm = new Particle[numberParticles];
                
                // global best known position and corresponding fitness
                double[] bestGlobalPosition = new double[Dim];
                double bestGlobalFitness = double.MaxValue;

                // constraints for min and max values for a new velocity
                double minV = -1.0*maxX;
                double maxV = maxX;

                // Initialize all Particle objects and swarm
                for (int i = 0; i < swarm.Length; ++i)
                {
                    double[] randomPosition = new double[Dim];
                    for (int j = 0; j < randomPosition.Length; ++j)
                    {
                        double lo = minX;
                        double hi = maxX;
                        randomPosition[j] = lo + (hi-lo)*ran.NextDouble();
                    }

                    double fitness = ObjectiveFunction(randomPosition);

                    double[] randomVelocity = new double[Dim];
                    for (int j = 0; j < randomVelocity.Length; ++j)
                    {
                        double lo = -1.0*Math.Abs(maxX-minX);
                        double hi = Math.Abs(maxX-minX);
                        randomVelocity[j] = lo + (hi-lo)*ran.NextDouble();
                    }

                    swarm[i] = new Particle(randomPosition, fitness, randomVelocity,
                                            randomPosition, fitness); // best known is initial
                
                    if (swarm[i].fitness < bestGlobalFitness) // minimization problem
                    {
                        bestGlobalFitness = swarm[i].fitness;
                        swarm[i].position.CopyTo(bestGlobalPosition, 0);
                    }
                }

                double w = 0.729; // inertia weight
                double c1 = 1.49445; // cognitive weight
                double c2 = 1.49445; // social weight
                double r1, r2; // randomizers as PSO components to prevent local optima

                // main PSO processing loop
                for (int i = 0; i < swarm.Length; ++i)
                {
                    // Iterate through each Particle object in swarm array
                    Particle currP = swarm[i]; 

                    // Update each Particle's velocity
                    double[] newVelocity = new double[currP.velocity.Length];
                    for (int j = 0; j < currP.velocity.Length; ++j)
                    {
                        r1 = ran.NextDouble();
                        r2 = ran.NextDouble();
                        
                        newVelocity[j] = w*currP.velocity[j] +
                                            c1*r1*(currP.bestPosition[j] - currP.position[j]) +
                                            c2*r2*(bestGlobalPosition[j] - currP.position[j]);
                    
                        if (newVelocity[j] < minV)
                        {
                            newVelocity[j] = minV;
                        }
                        else if (newVelocity[j] > maxV)
                        {
                            newVelocity[j] = maxV;
                        }                        
                    }
                    newVelocity.CopyTo(currP.velocity, 0);

                    // Use new velocity to compute and update current Particle's position
                    double[] newPosition = new double[currP.position.Length];
                    for (int j = 0; j < currP.position.Length; ++j)
                    {                        
                        newPosition[j] = currP.position[j] + newVelocity[j];

                        if (newPosition[j] < minX)
                        {
                            newPosition[j] = minX;
                        }                            
                        else if (newPosition[j] > maxX)
                        {
                            newPosition[j] = maxX;
                        }
                    }
                    newPosition.CopyTo(currP.position, 0);

                    // Compute new fitness value and update the object's fitness field
                    double newFitness = ObjectiveFunction(newPosition);
                    currP.fitness = newFitness;
                    if (newFitness < currP.bestFitness)
                    {
                        newPosition.CopyTo(currP.bestPosition, 0);
                        currP.bestFitness = newFitness;
                    }
                    if (newFitness < bestGlobalFitness)
                    {
                        newPosition.CopyTo(bestGlobalPosition, 0);
                        bestGlobalFitness = newFitness;
                    }
                }

                Console.Write("Final best fitness = ");
                Console.WriteLine(bestGlobalFitness.ToString());
                Console.WriteLine("Best position:");
                for (int i = 0; i < bestGlobalPosition.Length; ++i)
                {
                    Console.Write("x" + i + " = ");
                    Console.WriteLine(bestGlobalPosition[i].ToString());
                }

                Console.WriteLine("\nEnd Particle Swarm Optimization Demo\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error: " + ex.Message);
            }
        }
    }
}
