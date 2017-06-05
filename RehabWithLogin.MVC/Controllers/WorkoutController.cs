﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RehabWithLogin.MVC.Data;
using RehabWithLogin.MVC.Models;

namespace RehabWithLogin.MVC.Controllers
{
    public class WorkoutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkoutController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int id)
        {
            var workout = _unitOfWork.WorkoutRepository.Get(x => x.Id == id, null, "WorkoutExercises.Exercise.Tool")
                .First();
            ViewBag.Tools = _unitOfWork.ToolRepository.Get();
            return View(workout);
        }

        [HttpPost]
        public IActionResult UpdateExerciseNotes(int id, string notes)
        {
            var workoutExercise = _unitOfWork.WorkoutExerciseRepository.Get(x => x.Id == id, null, "Workout").First();
            workoutExercise.Notes = notes;
            _unitOfWork.WorkoutExerciseRepository.Update(workoutExercise);
            _unitOfWork.Save();

            return RedirectToAction("Index", new {id = workoutExercise.Workout.Id});
        }

        [HttpPost]
        public IActionResult NewExercise(int workoutId, string name, string description, int? toolId,
            string toolName, string videoUrl, int reps, int sets, string resistance, string notes)
        {
            var tool = toolId != null ? _unitOfWork.ToolRepository.GetById(toolId) : new Tool {Name = toolName};
            var exercise = new Exercise
            {
                Name = name,
                Description = description,
                Tool = tool,
                VideoUrl = videoUrl,
                WorkoutExercises = new List<WorkoutExercise>()
            };

            var workout = _unitOfWork.WorkoutRepository.GetById(workoutId);

            exercise.WorkoutExercises.Add(new WorkoutExercise
            {
                Exercise = exercise,
                Workout = workout,
                Resistance = resistance,
                Reps = reps,
                Sets = sets,
                Notes = notes
            });
            _unitOfWork.ExerciseRepository.Insert(exercise);
            _unitOfWork.Save();
            return RedirectToAction("Index", new {id = workoutId});
        }

        [HttpPost]
        public IActionResult UpdateExercise(int workoutExerciseId, int exerciseId, string name, string description,
            int? toolId,
            string toolName, string videoUrl, int reps, int sets, string resistance, string notes)
        {
            var tool = toolId != null ? _unitOfWork.ToolRepository.GetById(toolId) : new Tool {Name = toolName};
            var exercise = _unitOfWork.ExerciseRepository.GetById(exerciseId);
            var workoutExercise = _unitOfWork.WorkoutExerciseRepository
                .Get(x => x.Id == workoutExerciseId, null, "Workout")
                .First();

            exercise.Description = description;
            exercise.Name = name;
            exercise.Tool = tool;
            exercise.VideoUrl = videoUrl;

            workoutExercise.Notes = notes;
            workoutExercise.Resistance = resistance;
            workoutExercise.Sets = sets;
            workoutExercise.Reps = reps;

            _unitOfWork.ExerciseRepository.Update(exercise);
            _unitOfWork.WorkoutExerciseRepository.Update(workoutExercise);
            _unitOfWork.Save();

            return RedirectToAction("Index", new {id = workoutExercise.Workout.Id});
        }

        [HttpPost]
        public IActionResult DeleteExercise(int workoutExerciseId, int workoutId)
        {
            _unitOfWork.WorkoutExerciseRepository.Delete(workoutExerciseId);
            _unitOfWork.Save();

            return RedirectToAction("Index", new {id = workoutId});
        }
    }
}