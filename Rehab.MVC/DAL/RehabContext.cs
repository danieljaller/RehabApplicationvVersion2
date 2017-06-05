﻿using Microsoft.EntityFrameworkCore;
using Rehab.MVC.Models;

namespace Rehab.MVC.DAL
{
    public class RehabContext : DbContext
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<Tool> Tools { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server = (localdb)\\mssqllocaldb; Database = Rehab; Trusted_Connection = True; ");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkoutPlanWorkout>().HasIndex(x => new { x.WorkoutId, x.WorkoutPlanId, x.ScheduledTime}).IsUnique(true);

            modelBuilder.Entity<WorkoutPlanWorkout>()
                .HasOne(wpw => wpw.WorkoutPlan)
                .WithMany(w => w.WorkoutPlanWorkouts)
                .HasForeignKey(wpw => wpw.WorkoutPlanId);

            modelBuilder.Entity<WorkoutPlanWorkout>()
                .HasOne(wpw => wpw.Workout)
                .WithMany(w => w.WorkoutPlanWorkouts)
                .HasForeignKey(wpw => wpw.WorkoutId);
        }
    }
}