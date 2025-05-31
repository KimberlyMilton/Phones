<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Phones.Search" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



     <div class="container">
        <link  REL="StyleSheet" href="Content/site.css" TYPE="text/css" /> 

       

        <script>
            $.noConflict();

            $(document.ready(function () {
            }));

            function showPhoneInfo(id) {
                window.location.href = "PhoneInfo?id=" + id;
            }

            function validateCurrency() {
                alert(this.value);
            }
        </script>
            <h1 class="searchHeader">Search Cell Phone Inventory</h1>
               <form role="form" class="form-inline" >
                <div style="height:30px;"></div>
                   <div class="row">
                     <div class="col-sm-4">
                        <div class="form-group">
                            <label for="brand" >Brand:&nbsp; </label>
                                <asp:TextBox ID="brand" runat="server" class="form-control"></asp:TextBox>
                            </div>
                      </div>
                      <div class="col-sm-4">
                       <div class="form-group">
                           <label for="model">Model:&nbsp; </label>
                               <asp:TextBox ID="model" runat="server" class="form-control"></asp:TextBox>
                           </div>
                      </div>
                         <div class="col-sm-4">
                           <div class="form-group">
                               <label for="storage">Storage:&nbsp; </label>
                                   <asp:TextBox ID="storage" runat="server" class="form-control"></asp:TextBox>
                               </div>
                          </div>
                    </div>
                    <div class="row">
                          <div class="col-sm-4">
                           <div class="form-group">
                               <label for="color">Color:&nbsp; </label>
                                   <asp:TextBox ID="color" runat="server" class="form-control"></asp:TextBox>
                               </div>
                          </div>
                           
                     <div class="col-sm-2">

                        <div class="form-group">
                            <label for="minPrice">Price Between: </label>
                            <div class="input-group">
                                          <div class="input-group-prepend">
                                            <span class="input-group-text">$</span>
                                          </div>
                                           <asp:TextBox ID="minPrice" runat="server" CssClass="form-control" placeholder="min Price" ></asp:TextBox> 
                                            <asp:RegularExpressionValidator runat="server"  ID="minPriceValidator" ControlToValidate="minPrice" Display="Dynamic" ValidationExpression="^\$?[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]{2})?$"  ForeColor="Red" ErrorMessage="Please enter valid price"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                         </div>
                     <div class="col-sm-2">
                         <div class="form-group">
                              <label for="maxPrice" style="color:white">to</label>

                     <div class="input-group">
                           <div class="input-group-prepend">
                             <span class="input-group-text">$</span>
                           </div>
                            <asp:TextBox ID="maxPrice" runat="server" CssClass="form-control" placeholder="Max Price" ></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server"  ID="maxPriceValidator" ControlToValidate="maxPrice" Display="Dynamic" ValidationExpression="^\$?[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]{2})?$"  ForeColor="Red" ErrorMessage="Please enter valid price"></asp:RegularExpressionValidator>
                    </div>
                         

                    </div>
                </div>
             </div>

        <div class="priceMinMaxError">  
              <asp:CompareValidator ID="PriceCompareValidator" runat="server"
                        ControlToValidate="minPrice"
                        ControlToCompare="maxPrice"
                        Operator="LessThan"
                        Type="Double"
                        ErrorMessage="Minimum price must be less than Maximum Price."
                        SetFocusOnError="true" ForeColor="Red" Display="Dynamic"
                />
         </div>     
              

        <div style="text-align:center; padding-top:20px;">  
            <asp:Button BorderStyle="None" class="btn btn-success searchBtn" ID="searchBtn" runat="server" OnClick="searchBtn_Click" Text="Search Inventory"  />
            <br /><br /> 
        </div>

               </form>
        </div>
    <div class="container-fluid" style="padding-right:50px; padding-left:50px;">
        <div style="padding:10px; width:100%; text-align:center; "><asp:Label ID="phoneCount" runat="server" Visible="false"></asp:Label></div>
         <asp:GridView runat="server" ID="searchGrd" 
                DataKeyNames="brand,model,storage,color,price" 
                AutoGenerateColumns="false" 
                UseAccessibleHeader="true" 
                ShowHeaderWhenEmpty="true"
                OnRowDataBound="searchGrd_RowDataBound"
                 AllowSorting="true"
                OnSorting="searchGrd_Sorting"
                HeaderStyle-CssClass="SgridTableHeaderStyle"  
                 CssClass="SgridTable table table-responsive" 
                 EmptyDataRowStyle-CssClass="SgridTableEmptyRow"
                 AllowPaging="false" Visible="false" ShowFooter="false"
                >
                <Columns>
                    <asp:BoundField DataField="brand"  HeaderText="Brand"  Visible="true"  HeaderStyle-CssClass="SgridTableHeaderStyle" ItemStyle-CssClass="SgridTableItemStyle" SortExpression="brand" />
                    <asp:BoundField DataField="model" HeaderText="Model"  HeaderStyle-CssClass="SgridTableHeaderStyle" ItemStyle-CssClass="SgridTableItemStyle" SortExpression="model"/>
                    <asp:BoundField DataField="storage" HeaderText="Storage"  HeaderStyle-CssClass="SgridTableHeaderStyle" ItemStyle-CssClass="SgridTableItemStyle" SortExpression="storage"/> 
                    <asp:BoundField DataField="color" HeaderText="Color"  HeaderStyle-CssClass="SgridTableHeaderStyle" ItemStyle-CssClass="SgridTableItemStyle" SortExpression="color"/>  
                    <asp:BoundField DataField="price" HeaderText="Price" DataFormatString="{0:C}"  HeaderStyle-CssClass="SgridTableHeaderStyle" ItemStyle-CssClass="SgridTableItemStyle" SortExpression="price"/>  
               </Columns>
                <rowstyle backcolor="White" forecolor="Black"/>
                <alternatingrowstyle backcolor="#F9F9F9"  forecolor="Black"  />
                <EmptyDataTemplate ><div style="padding:10px;">No Phones meeting the critera entered have been found.</div></EmptyDataTemplate>
                   <PagerStyle HorizontalAlign="Center" CssClass="GridPager" />
            </asp:GridView>

        </div>
</asp:Content>
