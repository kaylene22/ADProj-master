﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ADProj.DB;
using ADProj.Enums;
using ADProj.Models;
using ADProj.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADProj.Controllers
{
    public class CollectionPointController : Controller
    {
        private CollectionPointService cps;
        private EmployeeService es;

        public CollectionPointController(CollectionPointService cps, EmployeeService es)
        {
            this.cps = cps;
            this.es = es;
        }

        /*public IActionResult Index()
        {
            List<CollectionPoint> cplist = cps.ListCollectionPoints();
            ViewData["cplist"] = cplist;
            if (TempData["alertMsg"] != null)
            {
                ViewData["alertMsg"] = TempData["alertMsg"];
            }
            return View();
        }


        public IActionResult AddCollectionPoint()
        {
            List<Employee> clerkList = es.GetAllClerks();
            ViewData["clerkList"] = clerkList;
            if (TempData["alertMsg"] != null)
            {
                ViewData["alertMsg"] = TempData["alertMsg"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult SaveCollectionPoint(string name, string time, int clerkId)
        {
            if (!(name != null && time != null && clerkId != null))
            {
                TempData["alertMsg"] = "Please enter all information";
                return RedirectToAction("AddCollectionPoint");
            }
            cps.CreateCollectionPoint(name, time, clerkId);
            return RedirectToAction("Index");
        }

        public IActionResult EditCollectionPoint(int cpId)
        {
            List<Employee> clerkList = es.GetAllClerks();
            ViewData["clerkList"] = clerkList;
            CollectionPoint cpToEdit = cps.GetCollectionPointById(cpId);
            ViewData["cpToEdit"] = cpToEdit;

            return View("UpdateCollectionPoint");
        }

        public IActionResult UpdateCollectionPoint(int cpId, string name, string time, int clerkId)
        {

            if (HttpContext.Session.GetString("role") == EmployeeRole.STORECLERK)
            {
            }


            if (!(cpId != null && name != null && time != null && clerkId != null))
            {
                TempData["alertMsg"] = "Please enter all information";
                return RedirectToAction("Index");
            }
            cps.UpdateCollectionPointById(cpId, name, time, clerkId);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteCollectionPoint(int cpId)
        {
            CollectionPoint cpToDelete = cps.GetCollectionPointById(cpId);
            ViewData["cpToDelete"] = cpToDelete;
            cps.DeleteCollectionPointById(cpId);
            return RedirectToAction("Index");
        }
*/

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("role") == EmployeeRole.STORECLERK || HttpContext.Session.GetString("role") == EmployeeRole.STORESUPERVISOR || HttpContext.Session.GetString("role") == EmployeeRole.STOREMANAGER)
            {
                List<CollectionPoint> cplist = cps.ListCollectionPoints();
                ViewData["cplist"] = cplist;
                if (TempData["alertMsg"] != null)
                {
                    ViewData["alertMsg"] = TempData["alertMsg"];
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AddCollectionPoint()
        {
            if (HttpContext.Session.GetString("role") == EmployeeRole.STORECLERK || HttpContext.Session.GetString("role") == EmployeeRole.STORESUPERVISOR || HttpContext.Session.GetString("role") == EmployeeRole.STOREMANAGER)
            {
                List<Employee> clerkList = es.GetAllClerks();
                ViewData["clerkList"] = clerkList;
                if (TempData["alertMsg"] != null)
                {
                    ViewData["alertMsg"] = TempData["alertMsg"];
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        //[HttpPost]
        public IActionResult SaveCollectionPoint(string name, string time, int clerkId)
        {
            int empId = Convert.ToInt32(HttpContext.Session.GetString("id"));

            if (HttpContext.Session.GetString("role") == EmployeeRole.STORECLERK || HttpContext.Session.GetString("role") == EmployeeRole.STORESUPERVISOR || HttpContext.Session.GetString("role") == EmployeeRole.STOREMANAGER)
            {
                if (!(name != null && time != null))
                {
                    TempData["alertMsg"] = "Please enter all information";
                    return RedirectToAction("AddCollectionPoint");
                }
                CollectionPoint cp = new CollectionPoint();
                cp.Name = name;
                cp.Time = time;
                cp.EmployeeId = clerkId;
                cps.AddCollectionPoint(empId, cp);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }


        public IActionResult EditCollectionPoint(int cpId)
        {
            if (HttpContext.Session.GetString("role") == EmployeeRole.STORECLERK || HttpContext.Session.GetString("role") == EmployeeRole.STORESUPERVISOR || HttpContext.Session.GetString("role") == EmployeeRole.STOREMANAGER)
            {
                List<Employee> clerkList = es.GetAllClerks();
                ViewData["clerkList"] = clerkList;
                CollectionPoint cpToEdit = cps.GetCollectionPointById(cpId);
                ViewData["cpToEdit"] = cpToEdit;

                return View("UpdateCollectionPoint");
            }
            return RedirectToAction("Index", "Home");
        }


        public IActionResult UpdateCollectionPoint(int cpId, string name, string time, int clerkId)
        {
            int empId = Convert.ToInt32(HttpContext.Session.GetString("id"));
            CollectionPoint cp = null;
            if (HttpContext.Session.GetString("role") == EmployeeRole.STORECLERK || HttpContext.Session.GetString("role") == EmployeeRole.STORESUPERVISOR || HttpContext.Session.GetString("role") == EmployeeRole.STOREMANAGER)
            {
                if (!(name != null && time != null))
                {
                    TempData["alertMsg"] = "Please enter all information";
                    return RedirectToAction("Index");
                }
                cp = cps.GetCollectionPointById(cpId);
                
                cp.Name = name;
                cp.Time = time;
                cp.EmployeeId = clerkId;
                cps.UpdateCollectionPoint(empId, cp);
                TempData["alertMsg"] = "Updated successfully!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteCollectionPoint(int cpId)
        {
            int empId = Convert.ToInt32(HttpContext.Session.GetString("id"));
            if (HttpContext.Session.GetString("role") == EmployeeRole.STORECLERK || HttpContext.Session.GetString("role") == EmployeeRole.STORESUPERVISOR || HttpContext.Session.GetString("role") == EmployeeRole.STOREMANAGER)
            {
                CollectionPoint cpToDelete = cps.GetCollectionPointById(cpId);
                cps.DeleteCollectionPoint(empId, cpToDelete);
                TempData["alertMsg"] = "Deleted successfully!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }

    }
}