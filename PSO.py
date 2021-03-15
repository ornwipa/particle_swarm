#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Mon Mar 15 10:08:54 2021

@author: ornwipa
"""

import random

# define cost function to be optimized (minimized) in PSO
def cost_func(x):
    total = 0
    for i in range(len(x)):
        total += x[i]**2
    return total

class Particle:
    def __init__(self, x0):
        self.position_i = []
        self.velocity_i = []
        self.pos_best_i = []
        self.err_best_i = -1
        self.err_i = -1
        
        for i in range(num_dimensions):
            self.velocity_i.append(random.uniform(-1, 1))
            self.position_i.append(x0[i])
    
    # evaluate current fitness
    def evaluate(self, cost_func):
        self.err_i = cost_func(self.position_i)
        
        # check if current position is individual best
        if self.err_i < self.err_best_i or self.err_best_i == -1:
            self.err_best_i = self.err_i
            self.pos_best_i = self.position_i
            
    # update new particle velocity
    def update_velocity(self, pos_best_g):
        w = 0.5 # constant inertial weight (of previous velocity)
        c1 = 1 # cognitive constant weight (of distance from individual particle's best known position)
        c2 = 2 # social constant weight (of distance from swarm best know position)
        
        for i in range(num_dimensions):
            r1 = random.random()
            r2 = random.random()
            vel_cognitive = c1*r1*(self.pos_best_i[i] - self.position_i[i])
            vel_social = c2*r2*(pos_best_g[i] - self.position_i[i])
            self.velocity_i[i] = w*self.velocity_i[i] + vel_cognitive + vel_social
    
    # update particle position based off new velocity updates
    def update_position(self, bounds):
        for i in range(num_dimensions):
            self.position_i[i] += self.velocity_i[i]
            
            # adjust minimum position if needed
            if self.position_i[i] < bounds[i][0]:
                self.position_i[i] = bounds[i][0]
            
            # adjust maximum position if needed
            if self.position_i[i] > bounds[i][1]:
                self.position_i[i] = bounds[i][1]

class PSO():
    def __init__(self, cost_func, x0, bounds, num_particles, maxiter):
        global num_dimensions
        num_dimensions = len(x0)
        err_best_g = -1
        pos_best_g = []
        
        # establish swarm
        swarm = []
        for i in range(num_particles):
            swarm.append(Particle(x0))
            
        # begin optimization loop
        i = 0
        while i < maxiter:
            
            # cycle through particles in swarm and evaluate fitness
            for j in range(num_particles):
                swarm[j].evaluate(cost_func)
                
                # determine if current particle is the best globally
                if swarm[j].err_i < err_best_g or err_best_g == -1:
                    err_best_g = float(swarm[j].err_i)
                    pos_best_g = list(swarm[j].position_i)
                    
            # cycle through swarm and update velocities and positions
            for j in range(num_particles):
                swarm[j].update_velocity(pos_best_g)
                swarm[j].update_position(bounds)
                
            i += 1
        
        print(pos_best_g)
        print(err_best_g)
            
# define main entry point of program
def main():
    initial=[5,5]               # initial starting location [x1,x2...]
    bounds=[(-10,10),(-10,10)]  # input bounds [(x1_min,x1_max),(x2_min,x2_max)...]
    PSO(cost_func, initial, bounds, num_particles=15, maxiter=30)

if __name__ == "__PSO__":
    main()
    