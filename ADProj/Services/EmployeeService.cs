﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADProj.DB;
using ADProj.Enums;
using ADProj.Models;

namespace ADProj.Services
//AUTHOR: LENG CHUNG HIANG, GUO JIEYI, NGUI KAI LIN

{
    public class EmployeeService
    {
        protected ADProjContext dbcontext;

        public EmployeeService(ADProjContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public List<Employee> FindAllEmployeesByDepartment(string DeptId)
        {
            List<Employee> employees = dbcontext.Employees.Where(x => x.DepartmentId == DeptId && x.Role != EmployeeRole.DEPTHEAD).ToList();
            return employees;
        }

        public Employee GetDeptRepByDeptId(string DeptId)
        {
            Employee deptrep = dbcontext.Employees.Where(x => x.DepartmentId == DeptId && x.Role == EmployeeRole.DEPTREP).FirstOrDefault();
            return deptrep;
        }

        public List<Employee> GetAllClerks()
        {
            return dbcontext.Employees.Where(x => x.Role == EmployeeRole.STORECLERK).ToList();
        }

        public Employee GetEmployee(string email)
        {
            Employee employee = dbcontext.Employees.Where(x => x.Email == email).FirstOrDefault();
            return employee;
        }

        public Employee GetEmployeeById(int employeeId)
        {
            Employee employee = dbcontext.Employees.Where(x => x.Id == employeeId).FirstOrDefault();
            return employee;
        }

        public ActingDepartmentHead GetActingDepartmentHead(string email)
        {
            DateTime currentDate = DateTime.Today;
 
            ActingDepartmentHead actingDepartmentHead = dbcontext.ActingDepartmentHeads.Where(x => x.Employee.Email == email && x.StartDate <= currentDate && x.EndDate >= currentDate).FirstOrDefault();
            return actingDepartmentHead;
        }

        public List<ActingDepartmentHead> GetFutureActingDepartmentHeads(Employee employee)
        {
            DateTime currentDate = DateTime.Today;

            List<ActingDepartmentHead> actingDepartmentHeads = dbcontext.ActingDepartmentHeads.Where(x => x.Employee.Department == employee.Department && x.EndDate >= currentDate).ToList();
            return actingDepartmentHeads;
        }

        public bool OverlappingAppointment(int employeeId, DateTime startDate, DateTime endDate)
        {
            bool overlap = false;
            Employee employee = GetEmployeeById(employeeId);
            List<ActingDepartmentHead> actingDepartmentHeads = dbcontext.ActingDepartmentHeads.Where(x => x.Employee.Department == employee.Department).ToList();
            if (actingDepartmentHeads.Count() > 0)
            {
                foreach (ActingDepartmentHead head in actingDepartmentHeads)
                {
                    if (head.EndDate > startDate && head.StartDate < endDate || head.StartDate == startDate)
                    {
                        overlap = true;
                    }
                }
            }
            return overlap;
        }

        public bool PasswordCheck(Employee employee, string hashPassword)
        {
            return (employee.Password == hashPassword);
        }

        public void ChangePassword(Employee employee, string NewPassword)
        {
            employee.Password = Crypto.Sha256(NewPassword);
            dbcontext.SaveChanges();
        }

        public bool PasswordDoubleCheck(string NewPassword, string ConfirmPassword)
        {
            return (NewPassword == ConfirmPassword);
        }

        public ActingDepartmentHead CurrentDelegate(Employee employee)
        {
            DateTime currentDate = DateTime.Today;
            ActingDepartmentHead actingDepartmentHead = dbcontext.ActingDepartmentHeads.Where(x => x.Employee.DepartmentId == employee.DepartmentId && x.StartDate <= currentDate && x.EndDate >= currentDate).FirstOrDefault();
            return actingDepartmentHead;
        }

        public void DeleteActingDepartmentHead(int id)
        {
            ActingDepartmentHead actingDepartmentHead = dbcontext.ActingDepartmentHeads.Where(x => x.Id == id).FirstOrDefault();
            if (actingDepartmentHead != null)
                dbcontext.ActingDepartmentHeads.Remove(actingDepartmentHead);
            dbcontext.SaveChanges();
        }

        public void AddActingDepartmentHead(int employeeId, DateTime startDate, DateTime endDate)
        {
            ActingDepartmentHead actingDepartmentHead = new ActingDepartmentHead()
            {
                EmployeeId = employeeId,
                StartDate = startDate,
                EndDate = endDate
            };
            dbcontext.Add(actingDepartmentHead);
            dbcontext.SaveChanges();
        }

        public List<Employee> DepartmentEmployeeList(Employee employee)
        {
            return dbcontext.Employees.Where(x => x.DepartmentId == employee.DepartmentId && x.Role == "Employee").ToList();
        }

        public ActingDepartmentHead GetActingDepartmentHeadByDepartment(Department dept)
        {
            ActingDepartmentHead actingDepartmentHead = dbcontext.ActingDepartmentHeads.Where(x => x.Employee.Department == dept).FirstOrDefault();
            return actingDepartmentHead;
        }
    }
}
