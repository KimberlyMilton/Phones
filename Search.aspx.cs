using Phones.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Data.Entity.Core.Common.CommandTrees;

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
                e.Row.Attributes.Add("onclick", "showPhoneInfo("+e.Row.RowIndex+");");
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
                List<cellPhone> phoneInventory = importData();

                //take the text entered in the boxes, and create a string list for the comma seperated values
                
                string[] brandsEntered =  this.brand.Text.ToLower().Replace(" ","").Trim().TrimEnd(',').TrimStart(',').Split(',');
                string[] colorsEntered = this.color.Text.ToLower().Replace(" ", "").Trim().TrimEnd(',').TrimStart(',').Split(',');
                string[] modelsEntered = this.model.Text.ToLower().Replace(" ", "").Trim().TrimEnd(',').TrimStart(',').Split(',');
                string[] storagesEntered = this.storage.Text.ToLower().Replace(" ", "").Trim().TrimEnd(',').TrimStart(',').Split(',');

                //Build the Query using the lists for the variables. Price will compare based on the min and max boxes entered
                //also have to check the string length of the textbox because a string array will always have a length of 1/
                var query = from cellPhoneItem in phoneInventory 
                     where
                      ((this.brand.Text.Trim().Length > 0 && brandsEntered.Length > 0) ? (brandsEntered.Any(wordInListEntered => cellPhoneItem.brand.ToLower().Trim().Replace(" ", "").Contains(wordInListEntered))) : true ) &&
                      ((this.model.Text.Trim().Length > 0 && modelsEntered.Length > 0) ? (modelsEntered.Any(wordInListEntered => cellPhoneItem.model.ToLower().Trim().Replace(" ", "").Contains(wordInListEntered))) : true) &&
                      ((this.storage.Text.Trim().Length > 0 && storagesEntered.Length > 0 ) ? (storagesEntered.Any(wordInListEntered => cellPhoneItem.storage.ToLower().Trim().Replace(" ", "").Contains(wordInListEntered))) : true) &&
                      ((this.color.Text.Trim().Length > 0 && colorsEntered.Length > 0 ) ? (colorsEntered.Any(wordInListEntered => cellPhoneItem.color.ToLower().Trim().Replace(" ", "").Contains(wordInListEntered))) : true) &&
                     ((this.minPrice.Text.Length > 0 && this.maxPrice.Text.Length < 1) ? cellPhoneItem.price >= Convert.ToDouble(this.minPrice.Text.Replace("$", "").Trim()) : true) && //minprioce but not max price
                     ((this.minPrice.Text.Length > 0 && this.maxPrice.Text.Length > 0) ? cellPhoneItem.price >= Convert.ToDouble(this.minPrice.Text.Replace("$", "").Trim()) && cellPhoneItem.price <= Convert.ToDouble(this.maxPrice.Text.Replace("$", "").Trim()) : true) && //min and max price
                     ((this.minPrice.Text.Length < 1 && this.maxPrice.Text.Length > 0) ? cellPhoneItem.price <= Convert.ToDouble(this.maxPrice.Text.Replace("$", "").Trim()) : true)  //max price but not min price
                    
                    orderby cellPhoneItem.brand, cellPhoneItem.model, cellPhoneItem.storage, cellPhoneItem.color, cellPhoneItem.price ascending
                    select cellPhoneItem;

              
                //RunQuery
                List<cellPhone> filteredPhoneList = query.ToList();

                //BindTODataGrid
                this.searchGrd.DataSource = filteredPhoneList; 
                this.searchGrd.DataBind();
                ViewState["PhoneInventorySort"] = filteredPhoneList;


                this.searchGrd.Visible = true;
                this.phoneCount.Text = searchGrd.Rows.Count.ToString() + " Phones found out of "+phoneInventory.Count.ToString()+" total phones carried." ;
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