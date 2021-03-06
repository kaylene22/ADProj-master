﻿using ADProj.DB;
using ADProj.Models;
using ADProj.ModelsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
namespace ADProj.Services
{
    public class DisbursementAPIService
    //AUTHOR: CHONG HENG TAT

    {
        private ADProjContext dbcontext;
        private Emailservice ems;

        public DisbursementAPIService(ADProjContext dbcontext, Emailservice ems)
        {
            this.dbcontext = dbcontext;
            this.ems = ems;
        }

        public static DisbursementDetailAPIModel DisbursementdetailmodelConverttoAPImodel(DisbursementDetails dd)
        {
            DisbursementDetailAPIModel ddAPI = new DisbursementDetailAPIModel()
            {
                Id = dd.Id,
                QtyNeeded = dd.QtyNeeded,
                QtyReceived = dd.QtyReceived,
                DisbursementId = dd.DisbursementId,
                InventoryItemId = dd.InventoryItemId,
                ItemDescription = dd.InventoryItem.Description,

            };

            return ddAPI;
        }

        public DisbursementDetails APImodelconvertoDisbursementDetailmodel(DisbursementDetailAPIModel apimodel)
        {
            DisbursementDetails dd = new DisbursementDetails()
            {
                Id = apimodel.Id,
                QtyNeeded = apimodel.QtyNeeded,
                QtyReceived = apimodel.QtyReceived,
                InventoryItemId = apimodel.InventoryItemId,
                DisbursementId = apimodel.DisbursementId
            };
            return dd;
        }

        public static DisbursementAPIModel DisbursementConverttoDisbursementAPIModel(Disbursement d)
        {
            DisbursementAPIModel apimodel = new DisbursementAPIModel
            {
                Id = d.Id,
                CollectionPointId = d.Department.CollectionPointId,
                DateRequested = d.DateRequested,
                DepartmentName = d.Department.Name,
                DisbursedDate = d.DisbursedDate
            };
            return apimodel;
        }

        //find all disbursementid (clerk)
        public List<DisbursementAPIModel> findalldisbursementid()
        {
            List<DisbursementAPIModel> Listofapimodels = new List<DisbursementAPIModel>();
            List<Disbursement> Listofdisbursements = dbcontext.Disbursements.ToList();
            foreach (Disbursement d in Listofdisbursements)
            {
                DisbursementAPIModel APIModel = DisbursementConverttoDisbursementAPIModel(d);
                Listofapimodels.Add(APIModel);
            }
            return Listofapimodels;
        }


        //to find disbursementdetail given a disbursementId
        public List<DisbursementDetailAPIModel> findalldisbursementdetail(int id)
        {
            List<DisbursementDetails> Listofdd = dbcontext.DisbursementDetails.Where(x => x.DisbursementId == id).ToList();
            List<DisbursementDetailAPIModel> ListofddAPI = new List<DisbursementDetailAPIModel>();
            foreach (DisbursementDetails i in Listofdd)
            {
                ListofddAPI.Add(DisbursementdetailmodelConverttoAPImodel(i));
            }
            return ListofddAPI;
        }

        //to find all disbursement which have not been disbursed belonging to department using employee id
        public List<DisbursementAPIModel> getdisbursementbyemployeeid(int id)
        {
            string Departmentid = dbcontext.Employees.Find(id).DepartmentId;
            List<DisbursementAPIModel> Listofapimodels = new List<DisbursementAPIModel>();
            List<Disbursement> Listofdisbursements = dbcontext.Disbursements.Where(x => x.DepartmentId == Departmentid & x.DisbursementStatus == Enums.DisbursementStatus.NOTCOLLECTED).ToList();
            foreach (Disbursement d in Listofdisbursements)
            {
                DisbursementAPIModel APIModel = DisbursementConverttoDisbursementAPIModel(d);
                Listofapimodels.Add(APIModel);
            }
            return Listofapimodels;
        }

        public void ChangeDisbursementStatustocollectbyDisbursementDetail(DisbursementDetails dd)
        {
            Disbursement d = dbcontext.Disbursements.Where(x => x.Id == dd.DisbursementId).FirstOrDefault();
            if (d.DisbursementStatus != Enums.DisbursementStatus.COLLECTED)
            {
                d.DisbursementStatus = Enums.DisbursementStatus.COLLECTED;
            }
            // update associated request status to completed
            List<Request> requests = dbcontext.Requests.Where(x => x.DisbursementId == d.Id).ToList();
            foreach (Request r in requests)
            {
                r.Status = Enums.Status.Completed;
                dbcontext.Update(r);
                ems.sendCompletedRequestToEmployeeEmail(r);
            }
            dbcontext.Update(d);
            dbcontext.SaveChanges();
        }
    }
}
