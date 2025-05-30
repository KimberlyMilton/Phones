using Phones.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace Phones
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //loadSearchGrid();
            }
        }

        protected void searchGrd_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
              //  e.Row.Attributes.Add("onclick", "showPhoneInfo(" + searchGrd.DataKeys[e.Row.RowIndex].Values["PhoneID"].ToString() + ");");
                e.Row.Attributes.Add("onmouseover", "this.style.cursor=\'pointer\'; this.originalstyle=this.style.backgroundColor; this.style.backgroundColor='#CEF6F5'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor = this.originalstyle");
            }
        }

        protected void searchBtn_Click(object sender, EventArgs e)
        {
            loadSearchGrid();
        }

        protected List<cellPhone> importData()
        {
            try
            {
                List<cellPhone> Inventory = null;

                string path = Server.MapPath("./App_Data/cellphonedata.txt");
                Inventory = ProcessCellInventoryFile(path);
                string numRows = Inventory.Count.ToString();
                return Inventory;
            }

            catch(Exception ex)
            {
                string exyy = ex.ToString();
                return null;
            }
     
        }

        protected List<cellPhone> ProcessCellInventoryFile(string dataFileName)
        {
            return File.ReadAllLines(dataFileName)
                //.Skip(1) //if there is a header row, this would skip it
                .Where(row => row.Length > 0)
                .Select(cellPhone.ParseRow).ToList();
        }

        protected void loadSearchGrid()
        {
            try
            {
                // ***add` price field validator **** /
                //** add in multiplpe values seperated by a comma **//
                List<cellPhone> phoneInventory = importData();

            var query = from cellPhone in phoneInventory
                        
                     where 
                    (this.brand.Text.Length > 0 ? cellPhone.brand.ToLower().Replace(" ", "").Contains(this.brand.Text.ToLower().Trim().Replace(" ", "").ToString()) : true ) &&
                    (this.model.Text.Length > 0 ? cellPhone.model.ToLower().Replace(" ", "").Contains(this.model.Text.ToLower().Trim().Replace(" ", "").ToString()) : true) &&
                    (this.storage.Text.Length > 0 ? cellPhone.storage.ToLower().Replace(" ", "").Contains(this.storage.Text.ToLower().Replace(" ", "").Trim().ToString()) : true) &&
                    (this.color.Text.Length > 0 ? cellPhone.color.ToLower().Replace(" ", "").Contains(this.color.Text.ToLower().Trim().Replace(" ", "").ToString()) : true) &&
                    ( (this.minPrice.Text.Length > 0 && this.maxPrice.Text.Length < 1) ? cellPhone.price >= Convert.ToDouble(this.minPrice.Text.Replace("$", "").Trim()) : true) && //minprioce but not max price
                    ( (this.minPrice.Text.Length > 0 && this.maxPrice.Text.Length > 0) ? cellPhone.price >= Convert.ToDouble(this.minPrice.Text.Replace("$", "").Trim()) && cellPhone.price <= Convert.ToDouble(this.maxPrice.Text.Replace("$", "").Trim()) : true) && //min and max price
                    ( (this.minPrice.Text.Length < 1 && this.maxPrice.Text.Length > 0) ? cellPhone.price <= Convert.ToDouble(this.maxPrice.Text.Replace("$", "").Trim()) : true)  //max price but not min price
                    
                    orderby cellPhone.brand, cellPhone.model, cellPhone.storage, cellPhone.color, cellPhone.price ascending

                // (this.brand.Text.Length > 0 ? cellPhone.brand.ToLower() == this.brand.Text.ToLower().Trim().ToString() : true ) &&
                // (this.model.Text.Length > 0 ? cellPhone.model.ToLower() == this.model.Text.ToLower().Trim().ToString() : true ) &&
                //(this.storage.Text.Length > 0 ? cellPhone.storage.ToLower() == this.storage.Text.ToLower().Trim().ToString() : true) &&
                //(this.color.Text.Length > 0 ? cellPhone.color.ToLower() == this.color.Text.ToLower().Trim().ToString() : true) 

                select cellPhone;

                List<cellPhone> filteredPhoneList = query.ToList();

                this.searchGrd.DataSource = filteredPhoneList; // phoneInventory; 
                this.searchGrd.DataBind();
                ViewState["PhoneInventorySort"] = filteredPhoneList;


                this.searchGrd.Visible = true;
                this.phoneCount.Text = query.Count().ToString() + " Phones found out of "+phoneInventory.Count.ToString()+" Phone Types in Inventory." ;
                this.phoneCount.Visible = true;

            }
            catch(Exception ex)
            {
                string trt = ex.ToString();
            }
          
        }

        protected void searchGrd_Sorting(object sender, GridViewSortEventArgs e)
        {

            if (ViewState["PhoneInventorySort"] != null)
            {
                List<cellPhone> inventoryList = (List<cellPhone>)ViewState["PhoneInventorySort"];

                string sortExpression = e.SortExpression;

                if (GetSortDirection(e.SortExpression) == "ASC")
                {
                    inventoryList = inventoryList.OrderBy(obj => {   //ascending is default
                        var propertyInfo = typeof(cellPhone).GetProperty(sortExpression);
                        return propertyInfo.GetValue(obj);
                    }).ToList();
                }
                else
                {
                    inventoryList = inventoryList.OrderByDescending(obj => {
                        var propertyInfo = typeof(cellPhone).GetProperty(sortExpression);
                        return propertyInfo.GetValue(obj);
                    }).ToList();
                }

                this.searchGrd.DataSource = inventoryList;
                this.searchGrd.DataBind();
                ViewState["PhoneInventorySort"] = inventoryList;
            }
        }

        private string GetSortDirection(string column)
        {

            // By default, set the sort direction to ascending.
            string sortDirection = "ASC";

            // Retrieve the last column that was sorted.
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            // Save new values in ViewState.
            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }



    }

}