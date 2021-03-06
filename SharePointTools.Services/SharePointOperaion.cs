﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SharePoint.Client;
using SharePointTools.Domain;
using SharePointTools.Services.Interface;
using SharePointTools.Utility;


namespace SharePointTools.Services
{
    public class SharePointOperaion : ISharePointOperation
    {
        private SharePointInfo info = new SharePointInfo();
        private Dictionary<string, string> dictionarys = DicOfName.GetDictionarys();

        public List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();
            var web = info.GetWeb1();
            return employees;
        }

        public Employee GetEmployee(string condition)
        {
            var web = info.GetWeb1();
            var list = web.Lists.GetByTitle("China Employees List");
            info.ChinaAdministrationContext.Load(list);
            info.ChinaAdministrationContext.ExecuteQuery();
            var items = list.GetItems(new CamlQuery());
            info.ChinaAdministrationContext.Load(items);
            info.ChinaAdministrationContext.ExecuteQuery();

            foreach (var item in items)
            {
                if (item[dictionarys[Constant.Name]] != null)
                {
                    if (condition.Contains(item[dictionarys[Constant.Name]].ToString()) ||
                        condition.Contains(item[dictionarys[Constant.EnglishName]].ToString()))
                    {

                        return GetEmployeeInfo(item);
                    }
                }
                else if (condition.Contains(item[dictionarys[Constant.EnglishName]].ToString()))
                {

                    return GetEmployeeInfo(item);
                }
            }
            return null;
        }

        public List<Employee> GetEmployees()
        {
            var web = info.GetWeb1();
            var list = web.Lists.GetByTitle("China Employees List");
            info.ChinaAdministrationContext.Load(list);
            info.ChinaAdministrationContext.ExecuteQuery();
            var items = list.GetItems(new CamlQuery());
            info.ChinaAdministrationContext.Load(items);
            info.ChinaAdministrationContext.ExecuteQuery();
            return items.Select(item => GetEmployeeInfo(item)).ToList();
        }


        public List<Employee> GetEmployees(string condition)
        {
            var employees = new List<Employee>();
            var web = info.GetWeb1();
            var list = web.Lists.GetByTitle("China Employees List");
            info.ChinaAdministrationContext.Load(list);
            info.ChinaAdministrationContext.ExecuteQuery();
            var items = list.GetItems(new CamlQuery());
            info.ChinaAdministrationContext.Load(items);
            info.ChinaAdministrationContext.ExecuteQuery();

            foreach (var item in items)
            {
                if (item[dictionarys[Constant.Name]] != null)
                {
                    if (condition.Contains(item[dictionarys[Constant.Name]].ToString()) ||
                        condition.Contains(item[dictionarys[Constant.EnglishName]].ToString()))
                    {

                        employees.Add(GetEmployeeInfo(item));
                    }
                }
                else if (condition.Contains(item[dictionarys[Constant.EnglishName]].ToString()))
                {

                    employees.Add(GetEmployeeInfo(item));
                }
            }
            return employees;
        }

        public List<Employee> GetIsLeaveEmployees(string condition, Dimission isLeave = Dimission.All)
        {
            var employees = new List<Employee>();
            var web = info.GetWeb1();
            var list = web.Lists.GetByTitle("China Employees List");
            info.ChinaAdministrationContext.Load(list);
            info.ChinaAdministrationContext.ExecuteQuery();
            var items = list.GetItems(new CamlQuery());
            info.ChinaAdministrationContext.Load(items);
            info.ChinaAdministrationContext.ExecuteQuery();

            foreach (var item in items)
            {
                if (item[dictionarys[Constant.Name]] != null)
                {
                    if (condition.Contains(item[dictionarys[Constant.Name]].ToString()) ||
                        condition.Contains(item[dictionarys[Constant.EnglishName]].ToString()))
                    {
                        employees.Add(GetOneDimissionEmployee(item, isLeave));
                    }
                }
                else if (condition.Contains(item[dictionarys[Constant.EnglishName]].ToString()))
                {

                    employees.Add(GetOneDimissionEmployee(item, isLeave));
                }
            }
            return employees;
        }

        public Employee GetOneDimissionEmployee(ListItem item, Dimission isLeave = Dimission.All)
        {
            if (isLeave.Equals(Dimission.NotLeave) && item[dictionarys[Constant.IsLeave]].ToString().Equals("否"))
            {
                return GetEmployeeInfo(item);
            }
            else if (isLeave.Equals(Dimission.HavedLeave) && item[dictionarys[Constant.IsLeave]].ToString().Equals("是"))
            {
                return GetEmployeeInfo(item);
            }
            else if (isLeave.Equals(Dimission.All))
            {
                return GetEmployeeInfo(item);
            }
            else
            {
                return null;
            }
        }

        public Employee GetEmployeeInfo(ListItem item)
        {
            FieldUrlValue fileUrl = null;
            Picture picture = null;
            if (item[dictionarys[Constant.Picture]] != null)
            {
                fileUrl = item[dictionarys[Constant.Picture]] as FieldUrlValue;
            }
            if (fileUrl != null)
            {
                if (!Directory.Exists(Constant.PictureSaveDirectory))
                {
                    Directory.CreateDirectory(Constant.PictureSaveDirectory);
                }
                picture = WebDownload.GetPictureContent(fileUrl.Url, Constant.PictureSaveDirectory+ "/" + fileUrl.Url.Substring(fileUrl.Url.LastIndexOf('/') + 1));
            }


            var employee = new Employee()
            {
                NO = item[dictionarys[Constant.NO]] == null ? "" : item[dictionarys[Constant.NO]].ToString(),
                MediumPicture = fileUrl == null ? "" : picture.Content,
                MediumPicturePath = fileUrl == null ? "" : fileUrl.Url,
                ID = item[dictionarys[Constant.ID]] == null ? "" : item[dictionarys[Constant.ID]].ToString(),
                Name = item[dictionarys[Constant.Name]] == null ? "" : item[dictionarys[Constant.Name]].ToString(),
                Sex = item[dictionarys[Constant.Sex]] == null ? "" : item[dictionarys[Constant.Sex]].ToString(),
                EnglishName = item[dictionarys[Constant.EnglishName]] == null ? "" : item[dictionarys[Constant.EnglishName]].ToString(),
                LoginDomainName = item[dictionarys[Constant.LoginDomainName]] == null ? "" : item[dictionarys[Constant.LoginDomainName]].ToString(),
                SkypeId = item[dictionarys[Constant.SkypeID]] == null ? "" : item[dictionarys[Constant.SkypeID]].ToString(),
                LyncId = item[dictionarys[Constant.LyncID]] == null ? "" : item[dictionarys[Constant.LyncID]].ToString(),
                Office = item[dictionarys[Constant.Office]] == null ? "" : item[dictionarys[Constant.Office]].ToString(),
                Department = item[dictionarys[Constant.Department]] == null ? "" : item[dictionarys[Constant.Department]].ToString(),
                Band = item[dictionarys[Constant.Band]] == null ? "" : item[dictionarys[Constant.Band]].ToString(),
                Province = item[dictionarys[Constant.Province]] == null ? "" : item[dictionarys[Constant.Province]].ToString(),
                PostCode = item[dictionarys[Constant.PostCode]] == null ? "" : item[dictionarys[Constant.PostCode]].ToString(),
                EducationBackgroud = item[dictionarys[Constant.EducationBackgroud]] == null ? "" : item[dictionarys[Constant.EducationBackgroud]].ToString(),
                Position = item[dictionarys[Constant.Position]] == null ? "" : item[dictionarys[Constant.Position]].ToString(),
                DirectLeader = item[dictionarys[Constant.DirectLeader]] == null ? "" : item[dictionarys[Constant.DirectLeader]].ToString(),
                ArrivalDate = item[dictionarys[Constant.ArrivalDate]] == null ? "" : item[dictionarys[Constant.ArrivalDate]].ToString(),
                LaborContractStartDate = item[dictionarys[Constant.LaborContractStartDate]] == null ? "" : item[dictionarys[Constant.LaborContractStartDate]].ToString(),
                ConversionDate = item[dictionarys[Constant.ConversionDate]] == null ? "" : item[dictionarys[Constant.ConversionDate]].ToString(),
                LaborContractEndDate = item[dictionarys[Constant.LaborContractEndDate]] == null ? "" : item[dictionarys[Constant.LaborContractEndDate]].ToString(),
                LaborContractUtilDate = item[dictionarys[Constant.LaborContractUtilDate]] == null ? "" : item[dictionarys[Constant.LaborContractUtilDate]].ToString(),
                Birthday = item[dictionarys[Constant.Birthday]] == null ? "" : item[dictionarys[Constant.Birthday]].ToString(),
                IsIntern = item[dictionarys[Constant.IsIntern]] == null ? "" : item[dictionarys[Constant.IsIntern]].ToString(),
                GraduateSchool = item[dictionarys[Constant.GraduateSchool]] == null ? "" : item[dictionarys[Constant.GraduateSchool]].ToString(),
                GraduateTime = item[dictionarys[Constant.GraduateTime]] == null ? "" : item[dictionarys[Constant.GraduateTime]].ToString(),
                HouseAddress = item[dictionarys[Constant.HouseAddress]] == null ? "" : item[dictionarys[Constant.HouseAddress]].ToString(),
                Mobile = item[dictionarys[Constant.Mobile]] == null ? "" : item[dictionarys[Constant.Mobile]].ToString(),
                IsLeave = item[dictionarys[Constant.IsLeave]] == null ? "" : item[dictionarys[Constant.IsLeave]].ToString(),
                LeaveDate = item[dictionarys[Constant.LeaveDate]] == null ? "" : item[dictionarys[Constant.LeaveDate]].ToString(),
                EditInHRIS = item[dictionarys[Constant.EditInHRIS]] == null ? "" : item[dictionarys[Constant.EditInHRIS]].ToString(),
                IDNumber = item[dictionarys[Constant.IDNumber]] == null ? "" : item[dictionarys[Constant.IDNumber]].ToString()
            };
            return employee;
        }
    }
}
